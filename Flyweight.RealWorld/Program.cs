// EN: Real World Example for the Flyweight Design Pattern
//
// Need: Represent a map of a city with tons of cars and trucks, each with a
// 3D model
//
// Solution: Having a pool of shared 3D vehicle and building models

using System.Diagnostics;

namespace RefactoringGuru.DesignPatterns.Flyweight.RealWorld;

// EN: The VehicleFlyweight class stores only the shared portion of the state
class VehicleFlyweight
{
    public List<int> Shared3DModel;
    private readonly VehicleType _vehicleType;

    public VehicleFlyweight(VehicleType vehicleType)
    {
        Shared3DModel = vehicleType switch
        {
            VehicleType.Car => ReadFile("mediumCar.3d"),
            VehicleType.Truck => ReadFile("largeTruck.3d"),
            _ => ReadFile("smallMotorbike.3d")
        };

        _vehicleType = vehicleType;
    }

    private static List<int> ReadFile(string fileName)
    {
        if (fileName.StartsWith("large"))
        {
            return Enumerable.Range(0, 1024 * 1024).Select(x => Random.Shared.Next()).ToList();
        }

        if (fileName.StartsWith("medium"))
        {
            return Enumerable.Range(0, 1024 * 256).Select(x => Random.Shared.Next()).ToList();
        }

        return Enumerable.Range(0, 1024 * 16).Select(x => Random.Shared.Next()).ToList();
    }

    public void Render(int x, int y, Direction direction)
    {
        Console.WriteLine($"Rendered {_vehicleType} in position {x}, {y} with direction {direction}");
    }
}

enum VehicleType
{
    Car,
    Truck,
    Motorbike
}

enum Direction
{
    North = 0,
    NorthEast = 45,
    East = 90,
    SouthEast = 135,
    South = 180,
    SouthWest = 225,
    West = 270,
    NorthWest = 315,
}

// EN: The Vehicle class contains the extrinsic state and a reference to the
// shared state.

class Vehicle
{
    private readonly int _x;
    private readonly int _y;
    private readonly Direction _direction;
    private readonly VehicleFlyweight _vehicleFlyweight;

    public Vehicle(
        int x,
        int y,
        Direction direction,
        VehicleFlyweight vehicleFlyweight)

    {
        _x = x;
        _y = y;
        _direction = direction;
        _vehicleFlyweight = vehicleFlyweight;
    }

    public void Render()
    {
        _vehicleFlyweight.Render(_x, _y, _direction);
    }
}

// EN: The Vehicle factory internally manages all the Flyweight objects

class VehicleFactory
{
    private static readonly Dictionary<VehicleType, VehicleFlyweight> VehicleFlyweights = new();

    public static Vehicle GetCar(int x, int y, Direction direction)
    {
        return GetVehicle(VehicleType.Car, x, y, direction);
    }

    public static Vehicle GetTruck(int x, int y, Direction direction)
    {
        return GetVehicle(VehicleType.Truck, x, y, direction);
    }

    public static Vehicle GetMotorbike(int x, int y, Direction direction)
    {
        return GetVehicle(VehicleType.Motorbike, x, y, direction);
    }

    // EN: Checks if the external state exists in the cache, otherwise it
    // creates a new one and stores it for reusing in the future
    private static Vehicle GetVehicle(VehicleType vehicleType, int x, int y, Direction direction)
    {
        if (!VehicleFlyweights.TryGetValue(vehicleType, out var value))
        {
            value = new VehicleFlyweight(vehicleType);
            VehicleFlyweights.Add(vehicleType, value);
        }

        return new Vehicle(x, y, direction, value);
    }
}

class Program
{
    static void Main(string[] args)
    {
        // EN: The client code is not aware of the internal representation, so no
        // reference to Flyweight objects are present.

        Console.WriteLine("Initially the application takes:");

        var memoryUsage = Process.GetCurrentProcess().PeakWorkingSet64;
        Console.WriteLine($"{memoryUsage / (1024 * 1024)}MB of PeakWorkingSet64");

        List<Vehicle> vehicles = [];

        for (var i = 0; i < 1000; i++)
        {
            var x = Random.Shared.Next(0, 1000);
            var y = Random.Shared.Next(0, 1000);
            var direction = i % 2 == 0 ? Direction.North : Direction.South;
            vehicles.Add(VehicleFactory.GetCar(x, y, direction));
        }

        for (var i = 0; i < 1000; i++)
        {
            var x = Random.Shared.Next(0, 1000);
            var y = Random.Shared.Next(0, 1000);
            var direction = i % 2 == 0 ? Direction.East : Direction.West;
            vehicles.Add(VehicleFactory.GetTruck(x, y, direction));
        }

        for (var i = 0; i < 1000; i++)
        {
            var x = Random.Shared.Next(0, 1000);
            var y = Random.Shared.Next(0, 1000);
            var direction = i % 2 == 0 ? Direction.SouthEast : Direction.NorthWest;
            vehicles.Add(VehicleFactory.GetMotorbike(x, y, direction));
        }

        Console.WriteLine($"After creating {vehicles.Count} vehicles, the application takes:");

        memoryUsage = Process.GetCurrentProcess().PeakWorkingSet64;
        Console.WriteLine($"{memoryUsage / (1024 * 1024)}MB of PeakWorkingSet64");


        Console.WriteLine("Let's create some vehicles flyweights directly to see what happens");

        List<VehicleFlyweight> flyweights = [];

        for (var i = 0; i < 1000; i++)
        {
            flyweights.Add(new VehicleFlyweight(VehicleType.Truck));
        }

        Console.WriteLine($"After creating {flyweights.Count} flyweights, finally the application takes:");

        memoryUsage = Process.GetCurrentProcess().PeakWorkingSet64;
        Console.WriteLine($"{memoryUsage / (1024 * 1024)}MB of PeakWorkingSet64");
    }
}