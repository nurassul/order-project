namespace Orders.Application.Orders.Models;

public class OrderViewModel
{
    public Guid OrderId { get; init; }

    public required string CustomerName { get; init; }
    
    public required string CustomerEmail { get; init; }
    
    public decimal TotalAmount { get; init; }
    
    public DateTime CreatedAt { get; init; }

}