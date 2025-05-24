using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace InvoiceApp.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Order Date is required.")]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public Product Product { get; set; }


        [DefaultValue(0)]
        public int Quntity { get; set; }

        [Display(Name = "Total Selling Cost")]
        [Column(TypeName = "decimal(18, 2)")]
        [ReadOnly(true)]
        public decimal TotalSellingCost { get; set; }
    }
}
