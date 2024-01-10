namespace MachineLoadProject.Equipment.Machine;

public class OperationFinishedEventArgs : EventArgs
{
    public readonly string MachineId;
    
    public OperationFinishedEventArgs(string machineId)
    {
        MachineId = machineId;
    }
}