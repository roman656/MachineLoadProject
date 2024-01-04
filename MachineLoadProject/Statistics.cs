namespace MachineLoadProject;

public class Statistics
{
    public uint IdleTime { get; set; }
    public uint WorkingTime { get; set; }
    public uint TotalTime => IdleTime + WorkingTime;
    public float LoadFactor => WorkingTime / (float)TotalTime;

    public Statistics(uint workingTime = 0, uint idleTime = 0)
    {
        IdleTime = idleTime;
        WorkingTime = workingTime;
    }
    
    public void AddIdleTime(uint time) => IdleTime += time;
    public void AddWorkingTime(uint time) => WorkingTime += time;
}