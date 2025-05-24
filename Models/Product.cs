using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace InvoiceApp.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Basic Rate is required.")]
        [Display(Name = "Basic Rate")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BasicRate { get; set; }

        [Display(Name = "Is GST Applicable?")]
        public bool IsGSTAplicable { get; set; } = false;

        [Display(Name = "Buying Cost")]
        [Column(TypeName = "decimal(18, 2)")]
        [ReadOnly(true)] // Calculated, so read-only on the form
        public decimal BuyingCost { get; set; }

        [Required(ErrorMessage = "MRP is required.")]
        [Display(Name = "MRP")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal MRP { get; set; }

        [Required(ErrorMessage = "Selling Price is required.")]
        [Display(Name = "Selling Price")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SellingPrice { get; set; }

        [Display(Name = "CGST Percentage (%)")]
        [Range(0, 100, ErrorMessage = "CGST Percentage must be between 0 and 100.")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal CGSTPercentage { get; set; } = 2.5m;

        [Display(Name = "SGST Percentage (%)")]
        [Range(0, 100, ErrorMessage = "SGST Percentage must be between 0 and 100.")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal SGSTPercentage { get; set; } = 2.5m;
    }
}
