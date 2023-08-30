namespace BLL.Services.TimeService;

public class StandartTimeService : ITimeService
{
    public DateTime GetCurrentDateTime()
    {
        return DateTime.UtcNow;
    }
}