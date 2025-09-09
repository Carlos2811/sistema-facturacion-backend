namespace InvoiceApi.DTOs
{
    public class InvoiceDetailDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }
}