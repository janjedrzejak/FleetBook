namespace FleetBook.Models;

public enum ReservationStatus
{
    Pending = 0,      // Wniosek czeka na akceptację (user)
    Approved = 1,     // Zatwierdzony (manager/admin)
    Rejected = 2,     // Odrzucony (manager/admin)
    Completed = 3,    // Zakończony (rezerwacja minęła)
    Cancelled = 4     // Anulowany (przez user)
}