using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace InvoiceApp.Models
{   
    /// <summary>
    /// Represents a product with pricing, tax, and packaging details for the invoice application.
    /// </summary>
    public class Product            
    {
        /// <summary>
        /// Primary key for the product.
        /// </summary>
        [Key]
        public int ProductId { get; set; }

        /// <summary>
        /// Name of the product.
        /// </summary>
        [Required(ErrorMessage = "Product Name is required.")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        /// <summary>
        /// Basic rate of the product.
        /// </summary>
        [Required(ErrorMessage = "Basic Rate is required.")]
        [Display(Name = "Basic Rate")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BasicRate { get; set; }

        /// <summary>
        /// Indicates if GST is applicable.
        /// </summary>
        [Display(Name = "Is GST Applicable?")]
        public bool IsGSTAplicable { get; set; } = false;

        /// <summary>
        /// Buying cost (calculated, read-only).
        /// </summary>
        [Display(Name = "Buying Cost")]
        [Column(TypeName = "decimal(18, 2)")]
        [ReadOnly(true)] // Calculated, so read-only on the form
        public decimal BuyingCost { get; set; }

        /// <summary>
        /// Maximum retail price.
        /// </summary>
        [Required(ErrorMessage = "MRP is required.")]
        [Display(Name = "MRP")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal MRP { get; set; }

        /// <summary>
        /// Selling price of the product.
        /// </summary>
        [Required(ErrorMessage = "Selling Price is required.")]
        [Display(Name = "Selling Price")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SellingPrice { get; set; }

        /// <summary>
        /// CGST percentage applied to the product.
        /// </summary>
        [Display(Name = "CGST Percentage (%)")]
        [Range(0, 100, ErrorMessage = "CGST Percentage must be between 0 and 100.")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal CGSTPercentage { get; set; } = 2.5m;

        /// <summary>
        /// SGST percentage applied to the product.
        /// </summary>
        [Display(Name = "SGST Percentage (%)")]
        [Range(0, 100, ErrorMessage = "SGST Percentage must be between 0 and 100.")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal SGSTPercentage { get; set; } = 2.5m;
    }
}
