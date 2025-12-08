namespace FleetBook.Models;

public class Car
{
    public int Id { get; set; }
    public string Marka { get; set; } = "";
    public string Model { get; set; } = "";
    public string Rejestracja { get; set; } = "";
    public int Rok { get; set; }
    public bool Dostepny { get; set; } = true;
    
    public List<Reservation> Reservations { get; set; } = new();
}