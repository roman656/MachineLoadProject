using MachineLoadProject.Equipment.Machine;
using MachineLoadProject.Equipment.Robot;

namespace MachineLoadProject;

public class Factory
{
    private const uint MachinesAmount = 4;
    private const uint StopTime = 20 * 60;
    private readonly uint[] _machinesOperationExecutionTime = { 18 * 60, 10 * 60, 3 * 60, 20 * 60 };
    private readonly uint[] _machinesMaintenanceTime = { 60, 60, 60, 60 };
    private readonly float[] _distanceToMachines = { 5.0f, 5.0f, 5.0f, 5.0f };
    private readonly Robot _robot = new (speed: 1.0f, clockTableInteractionTime: 30);
    private readonly Machine[] _machines = new Machine[MachinesAmount];
    private readonly Queue<MaintenanceRequest> _maintenanceRequestQueue = new ();
    private uint _globalTime;
    private uint _nearestEventTime;

    public Factory()
    {
        Initialize();
    }

    private void Initialize()
    {
        for (var index = 0; index < MachinesAmount; index++)
        {
            _machines[index] = new Machine(_machinesOperationExecutionTime[index], _machinesMaintenanceTime[index]);
            _machines[index].OperationFinished += OnMachineOperationFinished;
        }
    }

    public void Start()
    {
        foreach (var machine in _machines)
        {
            machine.Start();
        }

        _nearestEventTime = FindNearestEventTime();
    }

    private uint FindNearestEventTime()
    {
        var nearestEventTime = _robot.CurrentTaskRemainingTime;
        
        foreach (var machine in _machines)
        {
            if (nearestEventTime == 0 || nearestEventTime > machine.CurrentTaskRemainingTime)
            {
                nearestEventTime = machine.CurrentTaskRemainingTime;
            }
        }

        nearestEventTime += _globalTime;

        if (nearestEventTime > StopTime)
        {
            nearestEventTime = StopTime;
        }

        return nearestEventTime;
    }

    private void MoveToTime(uint time)
    {
        _robot.ProcessCurrentTask(time - _globalTime);
        
        foreach (var machine in _machines)
        {
            machine.ProcessCurrentTask(time - _globalTime);
        }

        _globalTime = time;
    }
    
    public void Process()
    {
        while (_globalTime < StopTime)
        {
            MoveToTime(_nearestEventTime);

            if (!_robot.IsBusy && _maintenanceRequestQueue.Count != 0)
            {
                _robot.RegisterMaintenanceRequest(_maintenanceRequestQueue.Dequeue());
                _robot.Start();
            }

            _nearestEventTime = FindNearestEventTime();
        }
    }
    
    public void PrintResults()
    {
        for (var index = 0; index < MachinesAmount; index++)
        {
            Console.WriteLine($"Коэффициент загрузки {index + 1}-го станка: {_machines[index].Statistics.LoadFactor}.\tРаботал {_machines[index].Statistics.WorkingTime} из {_machines[index].Statistics.TotalTime} секунд");
        }
    
        Console.WriteLine($"Коэффициент загрузки робота: {_robot.Statistics.LoadFactor}. Работал {_robot.Statistics.WorkingTime} из {_robot.Statistics.TotalTime} секунд");
    }

    private void OnMachineOperationFinished(object sender, OperationFinishedEventArgs args)
    {
        if (sender is Machine machine)
        {
            var distanceToMachine = GetDistanceToMachine(machine.Id) ?? 0.0f;
            _maintenanceRequestQueue.Enqueue(new MaintenanceRequest(machine, distanceToMachine));
        }
    }
    
    private float? GetDistanceToMachine(string machineId)
    {
        for (var index = 0; index < MachinesAmount; index++)
        {
            if (_machines[index].Id == machineId)
            {
                return _distanceToMachines[index];
            }
        }

        return null;
    }
}