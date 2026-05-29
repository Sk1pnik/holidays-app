namespace HolidayApp.Database;

public class Holiday
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LocalName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
}