using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace ShoppingCart.Models;

public class Review
{
    public long Id { get; set; }

    public string body { get; set; }

    public string UserId { get; set; }

    public long ProductId { get; set; }

    public Product Product { get; set; }

    public AppUser AppUser { get; set; }
}