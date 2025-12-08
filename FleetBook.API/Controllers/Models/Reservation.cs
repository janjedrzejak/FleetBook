namespace FleetBook.Models;

public class Reservation
{
    public int Id { get; set; }
    
    public int CarId { get; set; }
    public Car Car { get; set; } = null!;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DateTime DataOd { get; set; }
    public DateTime DataDo { get; set; }
    
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    
    public string Notatki { get; set; } = "";
    public string NotatkiAkceptujacego { get; set; } = "";
    
    public int? ApprovedByUserId { get; set; }
    public User? ApprovedByUser { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
}