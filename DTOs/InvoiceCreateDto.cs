using System.Collections.Generic;

namespace InvoiceApi.DTOs
{
    public class InvoiceCreateDto
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public List<InvoiceDetailDto> Details { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }
}