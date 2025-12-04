using Microsoft.EntityFrameworkCore;
using FleetBook.API.Data;
using FleetBook.Models;

namespace FleetBook.Services;

public class ReservationService
{
    private readonly ApplicationDbContext _db;
    public ReservationService(ApplicationDbContext db) => _db = db;
    
    public async Task<List<Reservation>> GetReservationsAsync() 
        => await _db.Reservations
            .Include(r => r.Car)
            .Include(r => r.User)
            .ToListAsync();

    public async Task AddReservationAsync(Reservation reservation)
    {
        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteReservationAsync(int id)
    {
        var reservation = await _db.Reservations.FindAsync(id);
        if (reservation != null)
        {
            _db.Reservations.Remove(reservation);
            await _db.SaveChangesAsync();
        }
    }

    public async Task UpdateReservationAsync(Reservation reservation)
    {
        var existing = await _db.Reservations.FindAsync(reservation.Id);
        if (existing != null)
        {
            existing.DataZwrotu = reservation.DataZwrotu;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<List<Car>> GetAvailableCarsAsync()
        => await _db.Cars
            .Where(c => !_db.Reservations.Any(r => r.CarId == c.Id && r.DataZwrotu == null))
            .ToListAsync();

    public async Task<List<User>> GetAuthorizedUsersAsync()
        => await _db.Users
            .Where(u => u.Uprawniony == 1)
            .ToListAsync();
}
