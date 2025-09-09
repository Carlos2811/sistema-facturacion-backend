using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using InvoiceApi.Data;
using InvoiceApi.DTOs;

namespace InvoiceApi.Services
{
    public class InvoiceService
    {
        private readonly InvoiceRepository _repo;
        public InvoiceService(InvoiceRepository repo) => _repo = repo;

        // Validaciones + delega creación al repo
        public async Task<int> CreateInvoiceAsync(InvoiceCreateDto dto)
        {
            // reglas basicas
            if (string.IsNullOrWhiteSpace(dto.InvoiceNumber))
                throw new ArgumentException("Número de factura es requerido");

            if (dto.Details == null || !dto.Details.Any())
                throw new ArgumentException("La factura debe contener al menos un detalle");

            // Recalcular totales del lado servidor
            decimal subtotal = dto.Details.Sum(d => d.Total);
            decimal tax = Math.Round(subtotal * 0.19m, 2);
            decimal total = subtotal + tax;

            if (subtotal != dto.Subtotal || tax != dto.Tax || total != dto.Total)
                throw new ArgumentException("Totales no coinciden con los detalles; se recalcularon en servidor");

            // Opcional: validar precios vs BD
            var products = await _repo.GetProductsAsync();
            var prodMap = products.ToDictionary(p => (int)((dynamic)p).productId, p => (decimal)((dynamic)p).unitPrice);

            foreach (var det in dto.Details)
            {
                if (!prodMap.ContainsKey(det.ProductId))
                    throw new ArgumentException($"Producto {det.ProductId} no existe");

                if (prodMap[det.ProductId] != det.UnitPrice)
                    throw new ArgumentException($"Precio del producto {det.ProductId} no coincide con el precio en BD");
            }

            // Llamar repo
            return await _repo.CreateInvoiceAsync(dto);
        }

        public Task<List<object>> GetClientsAsync() => _repo.GetClientsAsync();
        public Task<List<object>> GetProductsAsync() => _repo.GetProductsAsync();
        public Task<InvoiceFullDto?> GetInvoiceByNumberAsync(string number) => _repo.GetInvoiceByNumberAsync(number);
        public Task<List<object>> SearchInvoicesAsync(string type, int? clientId, string? number, DateTime? from, DateTime? to) =>
            _repo.SearchInvoicesAsync(type, clientId, number, from, to);
    }
}