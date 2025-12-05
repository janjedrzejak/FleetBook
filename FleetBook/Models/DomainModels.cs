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