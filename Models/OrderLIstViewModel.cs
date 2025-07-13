using System;

namespace InvoiceApp.Models
{
    /// <summary>
    /// ViewModel for displaying order list with product and order details.
    /// </summary>
    public class OrderLIstViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal BasicRate { get; set; }
        public decimal BuyingCost { get; set; }
        public decimal SellingPrice { get; set; }
        public int CrateCount { get; set; }
        public int Quntity { get; set; }
        public int UnitsPerCrate { get; set; }
    }
}
