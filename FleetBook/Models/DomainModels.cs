public class CarDto
{
    public int Id { get; set; }
    public string Marka { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Rejestracja { get; set; } = string.Empty;
    public int Rok { get; set; }
    public bool Dostepny { get; set; }
}