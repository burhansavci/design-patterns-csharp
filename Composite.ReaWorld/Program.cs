// EN: Real World Example for the Composite Design Pattern
// 
// Need: Calculate the total price of a shipment of packages that can contain
// other packages
// 
// Solution: Create a common interface for the package that contains only
// products (leafs) and the package that contains other packages
namespace RefactoringGuru.DesignPatterns.Composite.RealWorld;

// EN: The base Package (Component) class declares the common operations.
// Removed the reference to the parent as in this example is not needed.
public abstract class PackageComponent
{
    protected PackageComponent(string title)
    {
    }

    public abstract void Add(PackageComponent packageComponent);
    public abstract void Remove(PackageComponent packageComponent);
    public abstract decimal GetPrice();

    public virtual bool IsComposite()
    {
        return false;
    }
}

// EN: The Product (Leaf) class only has the getPrice implementation
class ProductLeaf : PackageComponent
{
    private readonly decimal _price;

    public ProductLeaf(string title, decimal price) : base(title)
    {
        _price = price;
    }

    public override void Add(PackageComponent packageComponent)
    {
        throw new InvalidOperationException("Cannot add to a leaf");
    }

    public override void Remove(PackageComponent packageComponent)
    {
        throw new InvalidOperationException("Cannot remove from a leaf");
    }

    public override decimal GetPrice()
    {
        return _price;
    }
}

// EN: The MultiPackage (Composite) class represents a complex package that
// contains other packages
class MultiPackageComposite : PackageComponent
{
    private readonly List<PackageComponent> _packageComponents;

    public MultiPackageComposite(string title) : base(title)
    {
        _packageComponents = new List<PackageComponent>();
    }

    public override void Add(PackageComponent packageComponent)
    {
        _packageComponents.Add(packageComponent);
    }

    public override void Remove(PackageComponent packageComponent)
    {
        _packageComponents.Remove(packageComponent);
    }

    public override decimal GetPrice()
    {
        return _packageComponents.Sum(packageComponent => packageComponent.GetPrice());
    }

    public override bool IsComposite()
    {
        return true;
    }
}

public class Program
{
    public static void Main()
    {
        // EN: The client code always works with base Package components
        var galaxyS68Pack = GetGalaxyS68Pack();
        var canonM50Pack = GetCanonM50Pack();
        var headphones = GetHeadphones();

        var shipment = new MultiPackageComposite("Main Shipment");
        shipment.Add(galaxyS68Pack);
        shipment.Add(canonM50Pack);
        shipment.Add(headphones);
        
        Console.WriteLine($"Total shipment cost: {shipment.GetPrice()}");
    }

    // EN: Helper (builder) functions hide there are 2 concrete package components
    private static PackageComponent GetGalaxyS68Pack()
    {
        var complexMobilePackage = new MultiPackageComposite("Galaxy S68 pack");
        complexMobilePackage.Add(new ProductLeaf("Galaxy S68", 900));
        complexMobilePackage.Add(new ProductLeaf("Charger", 25));
        complexMobilePackage.Add(new ProductLeaf("Earphones", 15));
        return complexMobilePackage;
    }

    private static PackageComponent GetCanonM50Pack()
    {
        var complexCameraPackage = new MultiPackageComposite("Canon M50 pack");
        complexCameraPackage.Add(new ProductLeaf("Canon M50", 600));
        complexCameraPackage.Add(new ProductLeaf("A50 1.8 Lends", 250));
        complexCameraPackage.Add(new ProductLeaf("128 Gb Micro SD", 40));
        complexCameraPackage.Add(new ProductLeaf("Canon Generic Case", 150));
        return complexCameraPackage;
    }

    private static PackageComponent GetHeadphones()
    {
        return new ProductLeaf("HyperX Cloud Flight", 150);
    }
}