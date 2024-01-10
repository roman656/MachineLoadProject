namespace MachineLoadProject.Equipment;

public interface IEquipment
{
    public string Id { get; }
    public bool IsBusy { get; }
    public uint CurrentTaskRemainingTime { get; }
    public uint CompletedTasksAmount { get; }
    public Statistics Statistics { get; }
    
    public void Start();
    public void Stop();
    public void ProcessCurrentTask(uint time);
}