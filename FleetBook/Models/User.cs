namespace FleetBook.Models;
public class User
{
    public int Id { get; set; }
    public string Imie { get; set; } = "";
    public string Nazwisko { get; set; } = "";
    public string Email { get; set; } = "";
    public string NumerTelefonu { get; set; } = "";
    public bool Uprawniony { get; set; } = false; 
}