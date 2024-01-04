namespace MachineLoadProject.Equipment.Machine;

public class Machine : BaseEquipment
{
    private readonly uint _operationExecutionTime;
    public delegate void OperationFinishedEventHandler(object sender, OperationFinishedEventArgs args);
    public event OperationFinishedEventHandler? OperationFinished;
    
    public Machine(uint operationExecutionTime, uint maintenanceTime)
    {
        _operationExecutionTime = operationExecutionTime;
        MaintenanceTime = maintenanceTime;
        PrepareForNextOperation();
    }

    public uint MaintenanceTime { get; }
    public override bool IsBusy => CurrentTaskRemainingTime != 0;
    
    public override void ProcessCurrentTask(uint time)
    {
        var prevState = State;
        
        AddElapsedTime(time);

        if (State == EquipmentState.Idle && prevState == EquipmentState.Working)
        {
            OnOperationFinished(new OperationFinishedEventArgs(machineId: Id));
        }
    }

    protected virtual void OnOperationFinished(OperationFinishedEventArgs args)
    {
        var operationFinished = OperationFinished;

        operationFinished?.Invoke(this, args);
    }

    public void PrepareForNextOperation() => CurrentTaskRemainingTime = _operationExecutionTime;
}