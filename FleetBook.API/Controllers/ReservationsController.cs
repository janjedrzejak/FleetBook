using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FleetBook.Models;
using FleetBook.API.Data;

namespace FleetBook.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<ReservationsController> _logger;

    public ReservationsController(ApplicationDbContext db, ILogger<ReservationsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Get all reservations
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<Reservation>>> GetReservations()
    {
        _logger.LogInformation("📅 GET api/reservations called");
        var reservations = await _db.Reservations
            .Include(r => r.Car)
            .Include(r => r.User)
            .Include(r => r.ApprovedByUser)
            .ToListAsync();
        return Ok(reservations);
    }

    /// <summary>
    /// Get user's reservations
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<Reservation>>> GetUserReservations(int userId)
    {
        _logger.LogInformation($"📅 GET api/reservations/user/{userId} called");
        var reservations = await _db.Reservations
            .Where(r => r.UserId == userId)
            .Include(r => r.Car)
            .Include(r => r.User)
            .Include(r => r.ApprovedByUser)
            .ToListAsync();
        return Ok(reservations);
    }

    /// <summary>
    /// Get pending reservations (for Manager approval)
    /// </summary>
    [HttpGet("pending")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<List<Reservation>>> GetPendingReservations()
    {
        _logger.LogInformation("📅 GET api/reservations/pending called");
        var reservations = await _db.Reservations
            .Where(r => r.Status == ReservationStatus.Pending)
            .Include(r => r.Car)
            .Include(r => r.User)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
        return Ok(reservations);
    }

    /// <summary>
    /// Get reservation by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Reservation>> GetReservationById(int id)
    {
        _logger.LogInformation($"📅 GET api/reservations/{id} called");
        var reservation = await _db.Reservations
            .Include(r => r.Car)
            .Include(r => r.User)
            .Include(r => r.ApprovedByUser)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
        {
            return NotFound(new { message = "Rezerwacja nie znaleziona" });
        }
        return Ok(reservation);
    }

    /// <summary>
    /// Create reservation (Admin - direct approval, User - pending approval)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateReservation([FromBody] CreateReservationRequest request, [FromHeader] string userId)
    {
        _logger.LogInformation($"📅 POST api/reservations called");

        if (request.DataOd >= request.DataDo)
        {
            return BadRequest(new { message = "Data od musi być przed datą do" });
        }

        // Check if car is available
        var conflict = await _db.Reservations.AnyAsync(r =>
            r.CarId == request.CarId &&
            r.Status == ReservationStatus.Approved &&
            r.DataOd < request.DataDo &&
            r.DataDo > request.DataOd);

        if (conflict)
        {
            return BadRequest(new { message = "Samochód nie jest dostępny w tym terminie" });
        }

        var userIdInt = int.Parse(userId);
        var user = await _db.Users.FindAsync(request.UserId ?? userIdInt);
        if (user == null)
        {
            return BadRequest(new { message = "Użytkownik nie znaleziony" });
        }

        if (user.Uprawniony != 1)
        {
            return BadRequest(new { message = "Użytkownik nie ma uprawnień do rezerwacji" });
        }

        var car = await _db.Cars.FindAsync(request.CarId);
        if (car == null)
        {
            return BadRequest(new { message = "Samochód nie znaleziony" });
        }

        var reservation = new Reservation
        {
            CarId = request.CarId,
            UserId = request.UserId ?? userIdInt,
            DataOd = request.DataOd,
            DataDo = request.DataDo,
            Notatki = request.Notatki ?? "",
            Status = request.IsAdmin ? ReservationStatus.Approved : ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ApprovedByUserId = request.IsAdmin ? userIdInt : null,
            ApprovedAt = request.IsAdmin ? DateTime.UtcNow : null
        };

        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);
    }

    /// <summary>
    /// Approve reservation (Manager only)
    /// </summary>
    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult> ApproveReservation(int id, [FromBody] ApproveReservationRequest request, [FromHeader] string userId)
    {
        _logger.LogInformation($"📅 PUT api/reservations/{id}/approve called");

        var reservation = await _db.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return NotFound(new { message = "Rezerwacja nie znaleziona" });
        }

        if (reservation.Status != ReservationStatus.Pending)
        {
            return BadRequest(new { message = "Rezerwacja musi być w statusie Pending" });
        }

        reservation.Status = ReservationStatus.Approved;
        reservation.ApprovedByUserId = int.Parse(userId);
        reservation.ApprovedAt = DateTime.UtcNow;
        reservation.NotatkiAkceptujacego = request.Notatki ?? "";

        _db.Reservations.Update(reservation);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Reject reservation (Manager only)
    /// </summary>
    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult> RejectReservation(int id, [FromBody] RejectReservationRequest request, [FromHeader] string userId)
    {
        _logger.LogInformation($"📅 PUT api/reservations/{id}/reject called");

        var reservation = await _db.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return NotFound(new { message = "Rezerwacja nie znaleziona" });
        }

        if (reservation.Status != ReservationStatus.Pending)
        {
            return BadRequest(new { message = "Rezerwacja musi być w statusie Pending" });
        }

        reservation.Status = ReservationStatus.Rejected;
        reservation.NotatkiAkceptujacego = request.Powod ?? "";

        _db.Reservations.Update(reservation);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Cancel reservation (User can cancel pending, Admin can cancel any)
    /// </summary>
    [HttpPut("{id}/cancel")]
    public async Task<ActionResult> CancelReservation(int id, [FromHeader] string userId)
    {
        _logger.LogInformation($"📅 PUT api/reservations/{id}/cancel called");

        var reservation = await _db.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return NotFound(new { message = "Rezerwacja nie znaleziona" });
        }

        var userIdInt = int.Parse(userId);
        var isAdmin = User.IsInRole("Admin");

        // User can only cancel own pending reservations
        if (!isAdmin && (reservation.UserId != userIdInt || reservation.Status != ReservationStatus.Pending))
        {
            return Forbid();
        }

        reservation.Status = ReservationStatus.Cancelled;
        _db.Reservations.Update(reservation);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Delete reservation (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteReservation(int id)
    {
        _logger.LogInformation($"📅 DELETE api/reservations/{id} called");

        var reservation = await _db.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return NotFound(new { message = "Rezerwacja nie znaleziona" });
        }

        _db.Reservations.Remove(reservation);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}

// Request/Response DTOs
public class CreateReservationRequest
{
    public int CarId { get; set; }
    public int? UserId { get; set; }
    public DateTime DataOd { get; set; }
    public DateTime DataDo { get; set; }
    public string? Notatki { get; set; }
    public bool IsAdmin { get; set; }
}

public class ApproveReservationRequest
{
    public string? Notatki { get; set; }
}

public class RejectReservationRequest
{
    public string? Powod { get; set; }
}