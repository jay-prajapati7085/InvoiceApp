using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceApp.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Invoice Date is required.")]
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; }

        //[Required(ErrorMessage = "Customer Name is required.")]
        //[Display(Name = "Customer Name")]
        //public string CustomerName { get; set; }

        [Required(ErrorMessage = "Total Amount is required.")]
        [Display(Name = "Total Amount")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
