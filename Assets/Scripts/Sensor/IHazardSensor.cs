using System;

public interface IHazardSensor
{
    event Action OnHazardTriggered;
}
