namespace MachineLoadProject.Equipment.Robot;

public enum MaintenanceState : byte
{
    MovingToMachine,
    MachineMaintenance,
    MovingToClockTable,
    ClockTableInteraction
}