using System.ComponentModel.DataAnnotations;

namespace OrderService.Contracts;

public class CreateOrderRequest
{
    [Required] 
    [StringLength(100)] 
    public string CustomerName { get; set; } = "";

    [Required] 
    [StringLength(100)] 
    [EmailAddress]
    public string CustomerEmail { get; set; } = "";

    [Range(typeof(decimal),
        "0.01",
        "999999999.99",
        ParseLimitsInInvariantCulture = true)
    ]
    public decimal TotalAmount { get; set; }
}