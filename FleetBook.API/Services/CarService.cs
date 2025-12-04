using Microsoft.EntityFrameworkCore;
using FleetBook.API.Data;
using FleetBook.Models;

namespace FleetBook.Services;

public class CarService
{
    private readonly ApplicationDbContext _db;
    public CarService(ApplicationDbContext db) => _db = db;

    public async Task<List<Car>> GetCarsAsync()
        => await _db.Cars.ToListAsync();
    
    public async Task DeleteCarAsync(int id)
    {
        var car = await _db.Cars.FindAsync(id);
        if (car != null)
        {
            _db.Cars.Remove(car);
            await _db.SaveChangesAsync();
        }
    }
    
    public async Task UpdateCarAsync(Car car)
    {
        var existingCar = await _db.Cars.FindAsync(car.Id);
        if (existingCar != null)
        {
            existingCar.Marka = car.Marka;
            existingCar.Model = car.Model;
            existingCar.Rejestracja = car.Rejestracja;
            existingCar.Rok = car.Rok;
            existingCar.Dostepny = car.Dostepny;
            
            await _db.SaveChangesAsync();
        }
    }
    
    public async Task AddCarAsync(Car car)
    {
        _db.Cars.Add(car);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Car>> GetCarsSortedAsync(string sortBy = "id", bool ascending = true)
    {
        var query = _db.Cars.AsQueryable();

        query = sortBy.ToLower() switch
        {
            "marka" => ascending ? query.OrderBy(c => c.Marka) : query.OrderByDescending(c => c.Marka),
            "model" => ascending ? query.OrderBy(c => c.Model) : query.OrderByDescending(c => c.Model),
            "rejestracja" => ascending ? query.OrderBy(c => c.Rejestracja) : query.OrderByDescending(c => c.Rejestracja),
            "dostepny" => ascending ? query.OrderBy(c => c.Dostepny) : query.OrderByDescending(c => c.Dostepny),
            _ => ascending ? query.OrderBy(c => c.Id) : query.OrderByDescending(c => c.Id),
        };

        return await query.ToListAsync();
    }
}
