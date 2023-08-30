using System;

namespace JoyFusionInitializer.Models;

public struct StepTimeServiceConfig
{
    public DateTime StartTimePosition { get; set; }
    public uint MaxMinuteTimeStep { get; set; }
}