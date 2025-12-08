namespace FleetBook.Models;


public class CarDto
{
    public int Id { get; set; }
    public string Marka { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Rejestracja { get; set; } = string.Empty;
    public int Rok { get; set; }
    public bool Dostepny { get; set; }
}


public class UserDto
{
    public int Id { get; set; }
    public string Imie { get; set; } = "";
    public string Nazwisko { get; set; } = "";
    public string Email { get; set; } = "";
    public string NumerTelefonu { get; set; } = "";
    public int Uprawniony { get; set; }
}


public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}


public class ReservationDto
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public CarDto? Car { get; set; }
    public int UserId { get; set; }
    public UserDto? User { get; set; }
    public DateTime DataOd { get; set; }
    public DateTime DataDo { get; set; }
    public int Status { get; set; }
    public string Notatki { get; set; } = "";
    public string NotatkiAkceptujacego { get; set; } = "";
    public int? ApprovedByUserId { get; set; }
    public UserDto? ApprovedByUser { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}


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