using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FleetBook.API.Data;
using FleetBook.Models;
using FleetBook.Services;

namespace FleetBook.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarsController : ControllerBase
{
    private readonly CarService _carService;
    private readonly ILogger<CarsController> _logger;

    public CarsController(CarService carService, ILogger<CarsController> logger)
    {
        _carService = carService;
        _logger = logger;
    }

    /// <summary>
    /// Get all cars
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Car>>> GetCars()
    {
        _logger.LogInformation("🚗 GET api/cars called");
        var cars = await _carService.GetCarsAsync();
        return Ok(cars);
    }

    /// <summary>
    /// Get car by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Car>> GetCarById(int id)
    {
        _logger.LogInformation($"🚗 GET api/cars/{id} called");
        var car = await _carService.GetCarByIdAsync(id);
        if (car == null)
        {
            return NotFound(new { message = "Car not found" });
        }
        return Ok(car);
    }

    /// <summary>
    /// Get cars sorted by specified field
    /// </summary>
    [HttpGet("sorted/{sortBy}")]
    public async Task<ActionResult<List<Car>>> GetCarsSorted(string sortBy, bool ascending = true)
    {
        _logger.LogInformation($"🚗 GET api/cars/sorted/{sortBy}?ascending={ascending} called");
        var cars = await _carService.GetCarsSortedAsync(sortBy, ascending);
        return Ok(cars);
    }

    /// <summary>
    /// Create a new car
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Car>> CreateCar([FromBody] Car car)
    {
        _logger.LogInformation($"🚗 POST api/cars called - Creating car: {car.Marka} {car.Model}");
        
        if (string.IsNullOrWhiteSpace(car.Marka) || string.IsNullOrWhiteSpace(car.Model))
        {
            return BadRequest(new { message = "Marka and Model are required" });
        }

        await _carService.AddCarAsync(car);
        return CreatedAtAction(nameof(GetCarById), new { id = car.Id }, car);
    }

    /// <summary>
    /// Update an existing car
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCar(int id, [FromBody] Car car)
    {
        _logger.LogInformation($"🚗 PUT api/cars/{id} called - Updating car");
        
        if (id != car.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        if (string.IsNullOrWhiteSpace(car.Marka) || string.IsNullOrWhiteSpace(car.Model))
        {
            return BadRequest(new { message = "Marka and Model are required" });
        }

        var existingCar = await _carService.GetCarByIdAsync(id);
        if (existingCar == null)
        {
            return NotFound(new { message = "Car not found" });
        }

        await _carService.UpdateCarAsync(car);
        return NoContent();
    }

    /// <summary>
    /// Delete a car
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCar(int id)
    {
        _logger.LogInformation($"🚗 DELETE api/cars/{id} called");
        
        var car = await _carService.GetCarByIdAsync(id);
        if (car == null)
        {
            return NotFound(new { message = "Car not found" });
        }

        await _carService.DeleteCarAsync(id);
        return NoContent();
    }
}