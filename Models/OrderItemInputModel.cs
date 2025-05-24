
using System.ComponentModel.DataAnnotations;


namespace InvoiceApp.Models
{
    public class OrderItemInputModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quntity { get; set; }
    }

    public class OrderCreateViewModel
    {
        [Required]
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        public List<OrderItemInputModel> OrderItems { get; set; } = new();

        public List<Product> Products { get; set; } = new();
    }
}