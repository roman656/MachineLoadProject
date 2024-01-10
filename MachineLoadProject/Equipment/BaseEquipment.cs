namespace MachineLoadProject.Equipment;

public abstract class BaseEquipment : IEquipment
{
    protected EquipmentState State = EquipmentState.Idle;
    
    public string Id { get; } = Ulid.NewUlid().ToString()!;
    public abstract bool IsBusy { get; }
    public uint CurrentTaskRemainingTime { get; protected set; }
    public uint CompletedTasksAmount { get; protected set; }
    public Statistics Statistics { get; } = new ();

    public void Start() => State = EquipmentState.Working;
    public void Stop() => State = EquipmentState.Idle;
    public abstract void ProcessCurrentTask(uint time);
    
    protected void AddElapsedTime(uint time)
    {
        if (State == EquipmentState.Idle)
        {
            Statistics.AddIdleTime(time);
        }
        else
        {
            if (time <= CurrentTaskRemainingTime)
            {
                Statistics.AddWorkingTime(time);
                CurrentTaskRemainingTime -= time;
            }
            else
            {
                Statistics.AddWorkingTime(CurrentTaskRemainingTime);
                Statistics.AddIdleTime(time - CurrentTaskRemainingTime);
                CurrentTaskRemainingTime = 0;
            }
            
            if (CurrentTaskRemainingTime == 0)
            {
                State = EquipmentState.Idle;
            }
        }
    }
}