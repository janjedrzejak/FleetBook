using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FleetBook.Models;
using FleetBook.Services;
using FleetBook.API.Data; 
using Microsoft.EntityFrameworkCore; 

namespace FleetBook.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<CarsController> _logger;

    public CarsController(ApplicationDbContext db, ILogger<CarsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Get all cars
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Car>>> GetCars()
    {
        _logger.LogInformation("🚗 GET api/cars called");
        var cars = await _db.Cars.ToListAsync();
        return Ok(cars);
    }

    /// <summary>
    /// Get car by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Car>> GetCarById(int id)
    {
        _logger.LogInformation($"🚗 GET api/cars/{id} called");
        var car = await _db.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound(new { message = "Samochód nie znaleziony" });
        }
        return Ok(car);
    }

    /// <summary>
    /// Get available cars for date range
    /// </summary>
    [HttpGet("available")]
    public async Task<ActionResult<List<Car>>> GetAvailableCars([FromQuery] DateTime dataOd, [FromQuery] DateTime dataDo)
    {
        _logger.LogInformation($"🚗 GET api/cars/available from {dataOd} to {dataDo}");
        
        var availableCars = await _db.Cars
            .Where(c => !_db.Reservations.Any(r =>
                r.CarId == c.Id &&
                r.Status == ReservationStatus.Approved &&
                r.DataOd < dataDo &&
                r.DataDo > dataOd))
            .ToListAsync();

        return Ok(availableCars);
    }

    /// <summary>
    /// Create a new car
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CreateCar([FromBody] Car car)
    {
        _logger.LogInformation($"🚗 POST api/cars called - Creating car: {car.Marka} {car.Model}");

        if (string.IsNullOrWhiteSpace(car.Marka) || string.IsNullOrWhiteSpace(car.Model))
        {
            return BadRequest(new { message = "Marka i Model są wymagane" });
        }

        _db.Cars.Add(car);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCarById), new { id = car.Id }, car);
    }

    /// <summary>
    /// Update a car
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateCar(int id, [FromBody] Car car)
    {
        _logger.LogInformation($"🚗 PUT api/cars/{id} called");

        if (id != car.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        var existingCar = await _db.Cars.FindAsync(id);
        if (existingCar == null)
        {
            return NotFound(new { message = "Samochód nie znaleziony" });
        }

        existingCar.Marka = car.Marka;
        existingCar.Model = car.Model;
        existingCar.Rejestracja = car.Rejestracja;
        existingCar.Rok = car.Rok;
        existingCar.Dostepny = car.Dostepny;

        _db.Cars.Update(existingCar);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Delete a car
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCar(int id)
    {
        _logger.LogInformation($"🚗 DELETE api/cars/{id} called");

        var car = await _db.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound(new { message = "Samochód nie znaleziony" });
        }

        _db.Cars.Remove(car);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}