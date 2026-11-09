namespace Orders.Application.Orders.Models;

public class OrderUpsertModel
{
    public string CustomerName { get; set; } = "";

    public string CustomerEmail { get; set; } = "";

    public decimal TotalAmount { get; set; }
}