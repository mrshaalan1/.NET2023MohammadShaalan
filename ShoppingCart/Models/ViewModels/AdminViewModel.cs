namespace ShoppingCart.Models.ViewModels
{
        public class AdminViewModel
        {
                public Product WinningProduct { get; set; }

                public int NumberOfProductSold { get; set; }

                public decimal TotalRevenue { get; set; }

                public AppUser LoyalCustomer { get; set; }

                public decimal MaxSpent { get; set; }
        }
}
