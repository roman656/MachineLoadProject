namespace MachineLoadProject.Equipment.Machine;

public class MaintenanceRequest
{
    public readonly Machine Machine;
    public readonly float DistanceToMachine;

    public MaintenanceRequest(Machine machine, float distanceToMachine)
    {
        Machine = machine;
        DistanceToMachine = distanceToMachine;
    }
}