// EN: Real World Example for the Template Method design pattern
//  
// Need: To format an invoice for different outputs
//  
// Solution: All components should have the ability to clone themselves

using System.Text;

namespace RefactoringGuru.DesignPatterns.TemplateMethod.RealWorld;

// EN: Some support records and classes

record Country(string Name, decimal Vat);

record Customer(string Id, string Name, Country Country);

record InvoiceLine(string Product, int Quantity, decimal UnitPrice);

class Invoice
{
    public readonly List<InvoiceLine> _invoiceLines = [];
    public string Notes { get; set; }
    public Customer Customer { get; }

    public Invoice(Customer customer)
    {
        Customer = customer;
    }

    public void AddLine(InvoiceLine line)
    {
        _invoiceLines.Add(line);
    }

    public decimal Taxes => SubTotal * Customer.Country.Vat;

    public decimal SubTotal => _invoiceLines.Sum(line => line.Quantity * line.UnitPrice);

    public decimal Total => SubTotal + Taxes;
}

// EN: The abstract formatter holds the template method and the hook methods
// for each step

abstract class InvoiceFormatter
{
    protected Invoice Invoice { get; }

    public InvoiceFormatter(Invoice invoice)
    {
        Invoice = invoice;
    }


    public string Format()
    {
        var result = new StringBuilder();
        result.Append(FormatHeader());
        result.Append(HookFormatSubHeader());
        result.Append(FormatCustomer());
        result.Append(FormatInvoiceLines());
        result.Append(FormatTaxes());
        result.Append(FormatTotal());
        result.Append(FormatFooter());
        result.Append(HookFormatSubFooter());
        return result.ToString();
    }

    protected abstract string FormatHeader();
    protected abstract string FormatCustomer();
    protected abstract string FormatInvoiceLines();
    protected abstract string FormatTaxes();
    protected abstract string FormatFooter();

    // These operations have already a default implementation
    protected virtual string HookFormatSubHeader() => "";
    protected virtual string FormatTotal() => $"Total: {Invoice.Total}";
    protected virtual string HookFormatSubFooter() => "";
}

// Concrete classes implement all abstract operations of the base
// class and also override some of the operations with default implementation

class HtmlInvoiceFormatter : InvoiceFormatter
{
    public HtmlInvoiceFormatter(Invoice invoice) : base(invoice)
    {
    }

    protected override string FormatHeader() => "<h1>ACME S.L. Invoice</h1>";
    protected override string HookFormatSubHeader() => "</hr>";
    protected override string FormatCustomer() => $"<div id='customer'>Customer: {Invoice.Customer.Name}, id: {Invoice.Customer.Id}</div>";
    protected override string FormatInvoiceLines() => $"<ul>\n{string.Join("\n", Invoice._invoiceLines.Select(l => $"  <li>{l.Product}, units={l.Quantity}, amount={l.Quantity * l.UnitPrice}€</li>"))}\n</ul>";
    protected override string FormatTaxes() => $"<div>Taxes: {Invoice.Taxes}€</div>";
    protected override string FormatFooter() => $"<footer>Center Avenue, 42, Rockland  -  {DateTime.Now.Year}</footer>";
}

class MarkdownInvoiceFormatter : InvoiceFormatter
{
    public MarkdownInvoiceFormatter(Invoice invoice) : base(invoice)
    {
    }

    protected override string FormatHeader() => "#ACME S.L. Invoice";
    protected override string HookFormatSubHeader() => "---";
    protected override string FormatCustomer() => $"Customer: **{Invoice.Customer.Name}**, id: {Invoice.Customer.Id}";
    protected override string FormatInvoiceLines() => string.Join("\n", Invoice._invoiceLines.Select(l => $"  - {l.Product}, units={l.Quantity}, amount={l.Quantity * l.UnitPrice}€"));
    protected override string FormatTaxes() => $"Taxes: {Invoice.Taxes}\u20ac";
    protected override string FormatFooter() => $"Center Avenue, 42, Rockland  -  {DateTime.Now.Year}";
    protected override string FormatTotal() => $"**Total**: {Invoice.Total}";
    protected override string HookFormatSubFooter() => "---";
}

// EN: The client code defines some constants and use the concrete
// classes calling the template method
class Program
{
    static void Main(string[] args)
    {
        var invoice = new Invoice(new Customer("amazon-spain", "Amazon Spain", new Country("Spain", 0.21m)));
        invoice.AddLine(new InvoiceLine("Apple", 10, 1.5m));
        invoice.AddLine(new InvoiceLine("Banana", 5, 0.5m));
        invoice.Notes = "Thanks for your purchase!";

        var htmlFormatter = new HtmlInvoiceFormatter(invoice);
        var markdownFormatter = new MarkdownInvoiceFormatter(invoice);

        Console.WriteLine("HTML Invoice:");
        Console.WriteLine(htmlFormatter.Format());
        Console.WriteLine("\nMarkdown Invoice:");
        Console.WriteLine(markdownFormatter.Format());
    }
}