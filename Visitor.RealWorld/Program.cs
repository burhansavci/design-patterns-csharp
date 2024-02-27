// EN: Visitor Design Pattern
//
// Intent: Lets you separate algorithms from the objects on which they operate.
//
// Example: In this example, the Visitor pattern helps to introduce a reporting
// feature into an existing class hierarchy:
//
// Company > Department > Employee
//
// Once the Visitor is added to the app, you can easily add other similar
// behaviors to app, without changing the existing classes.

namespace RefactoringGuru.DesignPatterns.Visitor.RealWorld;

// EN: The Component interface declares a method of accepting visitor objects.
//
// In this method, a Concrete Component must call a specific Visitor's method
// that has the same parameter type as that component.

interface IEntity
{
    public string Accept(IVisitor visitor);
}

// EN: The Company Concrete Component.
class Company : IEntity
{
    public Company(string name, List<Department> departments)
    {
        Name = name;
        Departments = departments;
    }

    public string Name { get; }
    public List<Department> Departments { get; }

    public string Accept(IVisitor visitor)
    {
        // EN: See, the Company component must call the VisitCompany method. The
        // same principle applies to all components.
        return visitor.VisitCompany(this);
    }
}

// EN: The Department Concrete Component.
class Department : IEntity
{
    public Department(string name, List<Employee> employees)
    {
        Name = name;
        Employees = employees;
    }

    public string Name { get; }
    public List<Employee> Employees { get; }

    public int GetCost()
    {
        return Employees.Sum(e => e.Salary);
    }

    public string Accept(IVisitor visitor)
    {
        return visitor.VisitDepartment(this);
    }
}

// EN: The Employee Concrete Component.
class Employee : IEntity
{
    public Employee(string name, string position, int salary)
    {
        Name = name;
        Position = position;
        Salary = salary;
    }

    public string Name { get; }
    public string Position { get; }
    public int Salary { get; }

    public string Accept(IVisitor visitor)
    {
        return visitor.VisitEmployee(this);
    }
}

// EN: The Visitor interface declares a set of visiting methods for each of the 
// Concrete Component classes.
interface IVisitor
{
    public string VisitCompany(Company company);
    public string VisitDepartment(Department department);
    public string VisitEmployee(Employee employee);
}

// EN: The Concrete Visitor must provide implementations for every single class
// of the Concrete Components.
class SalaryReport : IVisitor
{
    public string VisitCompany(Company company)
    {
        string output = "";
        int total = 0;
        foreach (var department in company.Departments)
        {
            total += department.GetCost();
            output += "\n--" + department.Accept(this);
        }

        return company.Name + " (" + total + ")\n" + output;
    }

    public string VisitDepartment(Department department)
    {
        string output = "";
        foreach (var employee in department.Employees)
        {
            output += "   " + VisitEmployee(employee);
        }

        return department.Name + " (" + department.GetCost() + ")\n\n" + output;
    }

    public string VisitEmployee(Employee employee)
    {
        return $"{employee.Salary} {employee.Name} ({employee.Position})\n";
    }
}

// EN: The client code.
class Program
{
    static void Main(string[] args)
    {
        var mobileDev = new Department("Mobile Development", new List<Employee>
        {
            new Employee("Albert Falmore", "designer", 100000),
            new Employee("Ali Halabay", "programmer", 100000),
            new Employee("Sarah Konor", "programmer", 90000),
            new Employee("Monica Ronaldino", "QA engineer", 31000),
            new Employee("James Smith", "QA engineer", 30000),
        });

        var techSupport = new Department("Tech Support", new List<Employee>
        {
            new Employee("Larry Ulbrecht", "supervisor", 70000),
            new Employee("Elton Pale", "operator", 30000),
            new Employee("Rajeet Kumar", "operator", 30000),
            new Employee("John Burnovsky", "operator", 34000),
            new Employee("Sergey Korolev", "operator", 35000),
        });

        var company = new Company("SuperStarDevelopment", [mobileDev, techSupport]);

        var report = new SalaryReport();

        Console.WriteLine("Client: I can print a report for a whole company:");
        Console.WriteLine(company.Accept(report));

        Console.WriteLine("Client: ...or for different entities: such as an employee, a department, or a company.");
        var someEmployee = new Employee("Some employee", "operator", 35000);
        var differentEntities = new List<IEntity> { someEmployee, techSupport, company };
        foreach (var entity in differentEntities)
        {
            Console.WriteLine(entity.Accept(report));
        }
    }
}