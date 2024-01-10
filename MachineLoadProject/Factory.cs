using MachineLoadProject.Equipment.Machine;
using MachineLoadProject.Equipment.Robot;

namespace MachineLoadProject;

public class Factory
{
    private const uint MachinesAmount = 4;
    private const uint StopTime = 60 * 60 * 8;
    private const uint ClockTableInteractionTime = 10;
    private const float RobotMovementSpeed = 0.4f;
    private readonly uint[] _machinesOperationExecutionTime = { 3 * 60, 5 * 60, 7 * 60, 14 * 60 };
    private readonly uint[] _machinesMaintenanceTime = { 30, 60, 30, 60 };
    private readonly float[] _distanceToMachines = { 5.0f, 7.0f, 9.0f, 11.0f };
    private readonly Robot _robot = new (RobotMovementSpeed, ClockTableInteractionTime);
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

        if (nearestEventTime == 0)
        {
            nearestEventTime = _machines[0].CurrentTaskRemainingTime;
            
            foreach (var machine in _machines)
            {
                if (nearestEventTime > machine.CurrentTaskRemainingTime)
                {
                    nearestEventTime = machine.CurrentTaskRemainingTime;
                }
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
        var timeDelta = time - _globalTime;
        
        foreach (var machine in _machines)
        {
            machine.ProcessCurrentTask(timeDelta);
        }
        
        _robot.ProcessCurrentTask(timeDelta);

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
    
    public void PrintReport()
    {
        Console.WriteLine($"Количество станков: {MachinesAmount}");
        Console.WriteLine($"Время смены: {StopTime}");
        
        Console.WriteLine("\nРобот:");
        Console.WriteLine($"  Время взаимодействия с тактовым столом: {ClockTableInteractionTime}");
        Console.WriteLine($"  Скорость движения: {RobotMovementSpeed}");
        Console.WriteLine($"  Работал: {_robot.Statistics.WorkingTime}");
        Console.WriteLine($"  Простаивал: {_robot.Statistics.IdleTime}");
        Console.WriteLine($"  Коэффициент загрузки: {_robot.Statistics.LoadFactor}");
        Console.WriteLine($"  Количество обслуживаний станков: {_robot.CompletedTasksAmount}");
        
        for (var index = 0; index < MachinesAmount; index++)
        {
            Console.WriteLine($"\nСтанок №{index + 1}:");
            Console.WriteLine($"  Расстояние до станка: {_distanceToMachines[index]}");
            Console.WriteLine($"  Время выполнения технологической операции: {_machinesOperationExecutionTime[index]}");
            Console.WriteLine($"  Время обслуживания: {_machinesMaintenanceTime[index]}");
            Console.WriteLine($"  Работал: {_machines[index].Statistics.WorkingTime}");
            Console.WriteLine($"  Простаивал: {_machines[index].Statistics.IdleTime}");
            Console.WriteLine($"  Коэффициент загрузки: {_machines[index].Statistics.LoadFactor}");
            Console.WriteLine($"  Количество произведенных деталей: {_machines[index].CompletedTasksAmount}");
        }
    }
    
    public void PrintShortReport()
    {
        Console.WriteLine($"Время смены: {StopTime}");
        Console.Write($"Коэффициент загрузки робота:      {_robot.Statistics.LoadFactor:F}");
        Console.Write($"  Работал / Простаивал: {_robot.Statistics.WorkingTime} / {_robot.Statistics.IdleTime}");
        Console.WriteLine($"  Количество обслуживаний станков:  {_robot.CompletedTasksAmount}");
        
        for (var index = 0; index < MachinesAmount; index++)
        {
            Console.Write($"Коэффициент загрузки {index + 1}-го станка: {_machines[index].Statistics.LoadFactor:F}");
            Console.Write($"  Работал / Простаивал: {_machines[index].Statistics.WorkingTime} / {_machines[index].Statistics.IdleTime}");
            Console.WriteLine($"  Количество произведенных деталей: {_machines[index].CompletedTasksAmount}");
        }
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