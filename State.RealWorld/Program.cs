// EN: Real World Example for the State design pattern
//
// Need: To implement a controller for a multi-product vending machine
//
// Solution: To create a finite state machine that controls the possible states and transitions.

namespace RefactoringGuru.DesignPatterns.State.RealWorld;

// EN: Some auxiliary classws

class Coin(string name, decimal value)
{
    public string Name { get; } = name;
    public decimal Value { get; } = value;
}

record Product(string Name, decimal Value);

class InventoryItem(Product product, int quantity)
{
    public Product Product { get; } = product;
    public int Quantity { get; set; } = quantity;
}

class Inventory(InventoryItem[] items)
{
    public InventoryItem[] Items { get; } = items;
}

// EN: The Context defines the interface of interest to clients. It also
// maintains a reference to an instance of a State subclass, which represents
// the current state of the Context.

class VendingMachineContext
{
    // EN: A reference to the current state
    private State _state;
    private decimal _credit;

    private readonly Inventory _inventory = new([
        new InventoryItem(new Product("Soda", 1.5m), 2),
        new InventoryItem(new Product("Chips", 2.5m), 0)
    ]);

    public VendingMachineContext(State state)
    {
        TransitionTo(state);
    }

    // EN: Some context public methods that the state will interact with
    public void AddCredit(decimal amount)
    {
        _credit += amount;
        Console.WriteLine($"Credit is now {_credit}");
    }

    public void ResetCredit()
    {
        _credit = 0;
        Console.WriteLine("Credit has been reset");
    }

    public bool HasStockOf(Product product)
    {
        return _inventory.Items.Any(i => i.Product == product && i.Quantity > 0);
    }

    public bool IsOutOfStock()
    {
        return _inventory.Items.All(i => i.Quantity <= 0);
    }

    public void DispenseProduct(Product product)
    {
        if (product.Value > _credit)
        {
            Console.WriteLine($"You are trying to buy a product with price {product.Value} but your credit is only {_credit}");
            return;
        }

        if (!HasStockOf(product))
        {
            Console.WriteLine($"Sorry, {product.Name} is out of stock, please select another product");
            return;
        }

        var item = _inventory.Items.First(i => i.Product == product);
        item.Quantity--;
        Console.WriteLine($"Product {product.Name} has been dispensed. Inventory is now: {item.Quantity}");
        _credit -= product.Value;
    }

    // EN: The Context allows changing the State object at runtime.
    public void TransitionTo(State state)
    {
        Console.WriteLine($"Context: Transition to {state.GetType().Name}.");
        _state = state;
        _state.SetContext(this);
    }

    // EN: The Context delegates part of its behavior to the current State
    public void InsertCoin(Coin coin)
    {
        _state.InsertCoin(coin);
    }

    public void SelectProduct(Product product)
    {
        _state.SelectProduct(product);
    }
}

// EN: The base State class declares methods that all Concrete State should
// implement and also provides a backreference to the Context object, associated
// with the State. This backreference can be used by States to transition the
// Context to another State.
abstract class State
{
    protected VendingMachineContext Context;

    public void SetContext(VendingMachineContext context)
    {
        Context = context;
    }

    public abstract void InsertCoin(Coin coin);
    public abstract void SelectProduct(Product product);
}

// EN: We will implement only 3 states. States are only responsible for the
// state transitions. We will delegate all the actions to the context,
// following the 'tell don't ask' principle.
class InitialReadyState : State
{
    public override void InsertCoin(Coin coin)
    {
        Context.AddCredit(coin.Value);
        Context.TransitionTo(new TransactionStarted());
    }

    public override void SelectProduct(Product product)
    {
        Console.WriteLine("You should insert coins before selecting the product");
    }
}

class TransactionStarted : State
{
    public override void InsertCoin(Coin coin)
    {
        Context.AddCredit(coin.Value);
    }

    public override void SelectProduct(Product product)
    {
        Context.DispenseProduct(product);
        if (Context.IsOutOfStock())
        {
            Context.TransitionTo(new OutOfStock());
        }
        else
        {
            Context.TransitionTo(new InitialReadyState());
        }
    }
}

class OutOfStock : State
{
    public override void InsertCoin(Coin coin)
    {
        Console.WriteLine("Stop inserting coins, we completely run out of stock");
    }

    public override void SelectProduct(Product product)
    {
        Console.WriteLine("Stop selecting products, we completely run of stock");
    }
}

// EN: The client code should handle edge cases and errors, in this case, only
// to log them to the console output
class Program
{
    static void Main()
    {
        var context = new VendingMachineContext(new InitialReadyState());

        context.SelectProduct(new Product("Soda", 1.5m));
        
        context.InsertCoin(new Coin("One and Half coin", 1.5m));
        context.SelectProduct(new Product("Soda", 1.5m));
        
        context.InsertCoin(new Coin("One coin", 1m));
        context.SelectProduct(new Product("Soda", 1.5m));
        context.SelectProduct(new Product("Chips", 2.5m));
        context.InsertCoin(new Coin("One coin", 1m));
        context.InsertCoin(new Coin("One coin", 1m));
        context.SelectProduct(new Product("Chips", 2.5m));
    }
}