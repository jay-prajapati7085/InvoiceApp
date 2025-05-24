namespace InvoiceApp.Models
{
    public class InvoiceViewModel
    {
        public required Invoice Invoice { get; set; }
        public required string AmountInWords { get; set; }
    }
}
