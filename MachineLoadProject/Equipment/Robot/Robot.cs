using MachineLoadProject.Equipment.Machine;

namespace MachineLoadProject.Equipment.Robot;

public class Robot : BaseEquipment
{
    private readonly float _speed;
    private readonly uint _clockTableInteractionTime;
    private MaintenanceState _maintenanceState;
    private MaintenanceRequest? _maintenanceRequest;

    public Robot(float speed, uint clockTableInteractionTime)
    {
        _speed = speed;
        _clockTableInteractionTime = clockTableInteractionTime;
    }
    
    private uint GetMovingTime(float distance) => (uint)Math.Ceiling(distance / _speed);

    public override bool IsBusy => _maintenanceRequest is not null;
    
    public override void ProcessCurrentTask(uint time)
    {
        if (!IsBusy || (IsBusy && (time < CurrentTaskRemainingTime)))
        {
            AddElapsedTime(time);
            return;
        }
        
        var remainingTime = time - CurrentTaskRemainingTime;    // Отрицательных значений не будет из-за проверки выше
        
        AddElapsedTime(CurrentTaskRemainingTime);
        SwitchToNextState();

        if (IsBusy)
        {
            Start();
        }
                
        if (remainingTime > 0)
        {
            ProcessCurrentTask(remainingTime);
        }
    }

    private void MaintainCurrentMachine()
    {
        _maintenanceRequest?.Machine.PrepareForNextOperation();
        _maintenanceRequest?.Machine.Start();
    }

    private void SwitchToNextState()
    {
        switch (_maintenanceState)
        {
            case MaintenanceState.MovingToMachine:
            {
                _maintenanceState = MaintenanceState.MachineMaintenance;
                CurrentTaskRemainingTime = _maintenanceRequest!.Machine.MaintenanceTime;

                break;
            }
            case MaintenanceState.MachineMaintenance:
            {
                MaintainCurrentMachine();
                
                _maintenanceState = MaintenanceState.MovingToClockTable;
                CurrentTaskRemainingTime = GetMovingTime(_maintenanceRequest!.DistanceToMachine);

                break;
            }
            case MaintenanceState.MovingToClockTable:
            {
                _maintenanceState = MaintenanceState.ClockTableInteraction;
                CurrentTaskRemainingTime = _clockTableInteractionTime;
                    
                break;
            }
            case MaintenanceState.ClockTableInteraction:
            {
                _maintenanceRequest = null;
                CompletedTasksAmount++;

                break;
            }
        }
    }

    public void RegisterMaintenanceRequest(MaintenanceRequest maintenanceRequest)
    {
        _maintenanceRequest = maintenanceRequest;
        _maintenanceState = MaintenanceState.MovingToMachine;
        CurrentTaskRemainingTime = GetMovingTime(_maintenanceRequest.DistanceToMachine);
    }
}