namespace Orders.Domain.Orders;

public class Order
{
    
    public Guid OrderId { get; set; }

    public required string CustomerName { get; set; }

    public required string CustomerEmail { get; set; }
    
    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }
}