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
        if (IsBusy)
        {
            switch (_maintenanceState)
            {
                case MaintenanceState.MovingToMachine:
                {
                    if (time < CurrentTaskRemainingTime)
                    {
                        AddElapsedTime(time);
                    }
                    else if (time == CurrentTaskRemainingTime)
                    {
                        AddElapsedTime(time);
                        _maintenanceState = MaintenanceState.MachineMaintenance;
                        CurrentTaskRemainingTime = _maintenanceRequest!.Machine.MaintenanceTime;
                        Start();
                    }
                    else
                    {
                        var remainingTime = time - CurrentTaskRemainingTime;
                        AddElapsedTime(CurrentTaskRemainingTime);
                        _maintenanceState = MaintenanceState.MachineMaintenance;
                        CurrentTaskRemainingTime = _maintenanceRequest!.Machine.MaintenanceTime;
                        Start();
                        ProcessCurrentTask(remainingTime);
                    }

                    break;
                }
                case MaintenanceState.MachineMaintenance:
                {
                    if (time < CurrentTaskRemainingTime)
                    {
                        AddElapsedTime(time);
                    }
                    else if (time == CurrentTaskRemainingTime)
                    {
                        AddElapsedTime(time);
                        _maintenanceRequest!.Machine.PrepareForNextOperation();
                        _maintenanceRequest.Machine.Start();
                        _maintenanceState = MaintenanceState.MovingToClockTable;
                        CurrentTaskRemainingTime = GetMovingTime(_maintenanceRequest.DistanceToMachine);
                        Start();
                    }
                    else
                    {
                        var remainingTime = time - CurrentTaskRemainingTime;
                        AddElapsedTime(CurrentTaskRemainingTime);
                        _maintenanceRequest!.Machine.PrepareForNextOperation();
                        _maintenanceRequest.Machine.Start();
                        _maintenanceState = MaintenanceState.MovingToClockTable;
                        CurrentTaskRemainingTime = GetMovingTime(_maintenanceRequest.DistanceToMachine);
                        Start();
                        ProcessCurrentTask(remainingTime);
                    }
                    
                    break;
                }
                case MaintenanceState.MovingToClockTable:
                {
                    if (time < CurrentTaskRemainingTime)
                    {
                        AddElapsedTime(time);
                    }
                    else if (time == CurrentTaskRemainingTime)
                    {
                        AddElapsedTime(time);
                        _maintenanceState = MaintenanceState.ClockTableInteraction;
                        CurrentTaskRemainingTime = _clockTableInteractionTime;
                        Start();
                    }
                    else
                    {
                        var remainingTime = time - CurrentTaskRemainingTime;
                        AddElapsedTime(CurrentTaskRemainingTime);
                        _maintenanceState = MaintenanceState.ClockTableInteraction;
                        CurrentTaskRemainingTime = _clockTableInteractionTime;
                        Start();
                        ProcessCurrentTask(remainingTime);
                    }
                    
                    break;
                }
                case MaintenanceState.ClockTableInteraction:
                {
                    if (time < CurrentTaskRemainingTime)
                    {
                        AddElapsedTime(time);
                    }
                    else if (time == CurrentTaskRemainingTime)
                    {
                        AddElapsedTime(time);
                        _maintenanceRequest = null;
                    }
                    else
                    {
                        var remainingTime = time - CurrentTaskRemainingTime;
                        AddElapsedTime(CurrentTaskRemainingTime);
                        _maintenanceRequest = null;
                        ProcessCurrentTask(remainingTime);
                        // запрос новой таски
                    }
                    
                    break;
                }
            }
        }
        else
        {
            AddElapsedTime(time);
        }
    }

    public void RegisterMaintenanceRequest(MaintenanceRequest maintenanceRequest)
    {
        _maintenanceRequest = maintenanceRequest;
        _maintenanceState = MaintenanceState.MovingToMachine;
        CurrentTaskRemainingTime = GetMovingTime(_maintenanceRequest.DistanceToMachine);
    }
}