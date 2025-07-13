using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace InvoiceApp.Models
{
    /// <summary>
    /// Represents an item in an order, including product, quantity, costs, and crate details.
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// Primary key for the order item.
        /// </summary>
        [Key]
        public int OrderId { get; set; }

        /// <summary>
        /// Date when the order was placed.
        /// </summary>
        [Required(ErrorMessage = "Order Date is required.")]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// The product associated with this order item.
        /// </summary>
        [Required(ErrorMessage = "Product is required.")]
        public Product Product { get; set; }

        /// <summary>
        /// Quantity of the product ordered.
        /// </summary>
        [DefaultValue(0)]
        public int Quntity { get; set; }

        /// <summary>
        /// Total selling cost for this order item (calculated, read-only).
        /// </summary>
        [Display(Name = "Total Selling Cost")]
        [Column(TypeName = "decimal(18, 2)")]
        [ReadOnly(true)]
        public decimal TotalSellingCost { get; set; }

        /// <summary>
        /// Number of units per crate for this order item.
        /// </summary>
        public int UnitsPerCrate { get; set; } 

        /// <summary>
        /// Number of crates for this order item.
        /// </summary>
        public int CrateCount { get; set; }
    }
}
