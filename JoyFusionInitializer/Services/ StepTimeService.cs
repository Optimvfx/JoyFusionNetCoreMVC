using System;
using System.Text.Json.Serialization;
using BLL.Services.TimeService;

namespace JoyFusionInitializer.Services;

public class StepTimeService : ITimeService
{
    private DateTime _currentDateTime;
    
    public readonly DateTime StartTimePosition;
    public readonly Func<TimeSpan> GetTimeStep;

    public StepTimeService(DateTime startTimePosition, Func<TimeSpan> getTimeStep)
    {
        StartTimePosition = startTimePosition;
        GetTimeStep = getTimeStep;
        _currentDateTime = StartTimePosition;
    }

    public DateTime GetCurrentDateTime()
    {
        var timeStep = GetTimeStep();

        if (timeStep.Ticks < 0)
            throw new ArgumentOutOfRangeException();

        _currentDateTime += timeStep;
        
        return _currentDateTime;
    }
}