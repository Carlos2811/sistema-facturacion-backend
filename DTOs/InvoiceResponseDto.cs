using System;
using System.Collections.Generic;

namespace InvoiceApi.DTOs
{
    public class InvoiceHeaderDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class InvoiceDetailResponseDto
    {
        public int InvoiceDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }

    public class InvoiceFullDto
    {
        public InvoiceHeaderDto Header { get; set; } = new();
        public List<InvoiceDetailResponseDto> Details { get; set; } = new();
    }
}