using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models;

public class Order
{
    
    [Key] public long Id { get; set; }
    [Microsoft.Build.Framework.Required] 
    public string FirstName { get; set; }
    
    [Microsoft.Build.Framework.Required]
    public string LastName { get; set; }
    
    public string userId { get; set; }
    
    [Microsoft.Build.Framework.Required]
    [DataType(DataType.MultilineText)]
    public string ShippingAddress { get; set; }
    
    [DataType(DataType.MultilineText)]
    public string OrderNotes { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a valid positive price.")]
    public decimal TotalAmount { get; set; }

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.Now;
    
    [Required] public string PaymentMethod { get; set; }
    public List<CartItem> OrderItemsIds { get; set; }
}