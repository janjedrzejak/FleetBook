using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FleetBook.Services;
using FleetBook.Models;

namespace FleetBook.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly CarService _carService;

    public CarsController(CarService carService)
    {
        _carService = carService;
    }

    // GET: /api/cars
    [HttpGet]
    [Authorize] // jeśli chcesz, aby tylko zalogowani widzieli listę
    public async Task<ActionResult<List<Car>>> GetCars()
    {
        var cars = await _carService.GetCarsAsync();
        return Ok(cars);
    }
}
