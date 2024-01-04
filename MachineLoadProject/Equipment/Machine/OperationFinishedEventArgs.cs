namespace MachineLoadProject.Equipment.Machine;

public class OperationFinishedEventArgs : EventArgs
{
    public string MachineId { get; private set; }
    
    public OperationFinishedEventArgs(string machineId)
    {
        MachineId = machineId;
    }
}