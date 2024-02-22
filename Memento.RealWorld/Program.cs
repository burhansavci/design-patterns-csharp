//EN: Real World Example for the Memento design pattern

//Need: To save all versions of an employee throughout their lifecycle

//Solution: Employee can save and restore their mementos, stored in a history
//class as caretaker

namespace RefactoringGuru.DesignPatterns.Memento.RealWorld;

class EmployeeOriginator
{
    private string Name;
    private decimal Salary;
    private decimal MonthlyExpensesLimit;

    public EmployeeOriginator(string name, decimal salary)
    {
        Name = name;
        Salary = salary;
        MonthlyExpensesLimit = 0;
    }


    public void RaiseSalaryTo(decimal newSalary)
    {
        Salary = newSalary;
        Console.WriteLine($"Employee: My salary has been raised to: {Salary}");
    }

    public void RaiseLimitTo(decimal newLimit)
    {
        MonthlyExpensesLimit = newLimit;
        Console.WriteLine($"Employee: My monthly expenses limit has been raised to: {MonthlyExpensesLimit}");
    }

    public Memento<EmployeeState> SaveSnapshot()
    {
        return new EmployeeMemento(new EmployeeState(Salary, MonthlyExpensesLimit));
    }

    public void RestoreSnapshot(Memento<EmployeeState> memento)
    {
        Salary = memento.State.Salary;
        MonthlyExpensesLimit = memento.State.MonthlyExpensesLimit;
        Console.WriteLine($"Employee: My state has been restored to: {memento.Name}");
    }

    public void PrintState()
    {
        Console.WriteLine($"Employee: State for {Name} is Salary={Salary} and MonthlyExpensesLimit={MonthlyExpensesLimit}");
    }
}

// EN: The Memento interface provides some metadata, apart from the
// originator state. We created the state public, but we can hide this
// implementation detail from the clients by setting the salary and
// monthlyExpensesLimit in the memento constructor and moving the restore
// method to the memento class instead of the originator
interface Memento<T>
{
    public T State { get; }
    public string Name { get; }
    public DateOnly Date { get; }
}

// EN: We also declare an interface for the employee (originator) state. Not
// all the employee fields should be in the state.
class EmployeeState
{
    public EmployeeState(decimal salary, decimal monthlyExpensesLimit)
    {
        Salary = salary;
        MonthlyExpensesLimit = monthlyExpensesLimit;
    }

    public decimal Salary { get; }
    public decimal MonthlyExpensesLimit { get; }
}

class EmployeeMemento : Memento<EmployeeState>
{
    public EmployeeMemento(EmployeeState state)
    {
        State = state;
        Date = DateOnly.FromDateTime(DateTime.Now);
        Name = $"date: {Date}, salary: {State.Salary}, limit: {State.MonthlyExpensesLimit}";
    }

    public EmployeeState State { get; }

    public string Name { get; }

    public DateOnly Date { get; }
}

// EN: The Caretaker doesn't need to access the state of the originator.
class EmployeeCareTaker
{
    private List<Memento<EmployeeState>> _mementos = new List<Memento<EmployeeState>>();
    private EmployeeOriginator _originator;

    public EmployeeCareTaker(EmployeeOriginator originator)
    {
        _originator = originator;
    }

    public void Backup()
    {
        Console.WriteLine("Caretaker: Saving Originator (Employee)'s state...");
        _mementos.Add(_originator.SaveSnapshot());
    }

    public void Undo()
    {
        if (_mementos.Count == 0)
        {
            return;
        }

        var memento = _mementos.Last();
        Console.WriteLine("Caretaker: Restoring state to: " + memento.Name);
        _mementos.Remove(memento);
        _originator.RestoreSnapshot(memento);
    }

    public void ShowHistory()
    {
        Console.WriteLine("Caretaker: Here's the list of mementos:");

        if (_mementos.Count == 0)
        {
            Console.WriteLine("Caretaker: No mementos to show");
        }

        foreach (var memento in _mementos)
        {
            Console.WriteLine(memento.Name);
        }
    }
}

// EN: The client code only interacts with mementos via the caretaker.
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Client: Creating employee originator and caretaker...");
        
        var originator = new EmployeeOriginator("John Doe", 1000);
        var careTaker = new EmployeeCareTaker(originator);

        Console.WriteLine("Client: Let's change states saving state before each change...");
        originator.PrintState();
        careTaker.Backup();
        originator.RaiseSalaryTo(2000);
        careTaker.Backup();
        originator.RaiseLimitTo(100);
        careTaker.Backup();
        originator.RaiseSalaryTo(3000);

        Console.WriteLine("Client: This is the history of mementos and the state of the originator:");
        careTaker.ShowHistory();
        originator.PrintState();

        Console.WriteLine("Client: Changed state up to 3 times, let's rollback to the initial state...");
        careTaker.Undo();
        originator.PrintState();
        careTaker.Undo();
        originator.PrintState();
        careTaker.Undo();
        originator.PrintState();

        Console.WriteLine("Client: Now the history of mementos should be empty...");
        careTaker.ShowHistory();

        Console.WriteLine("Client: A new undo will leave the employee untouched...");
        careTaker.Undo();
        originator.PrintState();
    }
}