namespace FleetBook.Models;
public class Reservation
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; } = null!;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DateTime DataRezerwacji { get; set; } = DateTime.Now;
    public DateTime? DataZwrotu { get; set; }
}
