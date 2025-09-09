using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using InvoiceApi.DTOs;

namespace InvoiceApi.Data
{
    public class InvoiceRepository
    {
        private readonly SqlHelper _sql;
        public InvoiceRepository(SqlHelper sql) => _sql = sql;

        // Crea factura usando SP con TVP
        public async Task<int> CreateInvoiceAsync(InvoiceCreateDto dto)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand("dbo.sp_CreateInvoice", conn)
            
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@InvoiceNumber", dto.InvoiceNumber);
            cmd.Parameters.AddWithValue("@ClientId", dto.ClientId);
            cmd.Parameters.AddWithValue("@Subtotal", dto.Subtotal);
            cmd.Parameters.AddWithValue("@Tax", dto.Tax);
            cmd.Parameters.AddWithValue("@Total", dto.Total);

            // preparar DataTable para TVP
            var table = new DataTable();
            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("Quantity", typeof(int));
            table.Columns.Add("UnitPrice", typeof(decimal));
            table.Columns.Add("Total", typeof(decimal));

            foreach (var d in dto.Details)
                table.Rows.Add(d.ProductId, d.Quantity, d.UnitPrice, d.Total);

            var p = cmd.Parameters.Add("@Details", SqlDbType.Structured);
            p.TypeName = "dbo.InvoiceDetailType";
            p.Value = table;

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return reader.GetInt32(0);

            throw new Exception("No se obtuvo el id de la factura");
        }

        public async Task<List<object>> GetClientsAsync()
        {
            var list = new List<object>();
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand("dbo.sp_GetClients", conn) { CommandType = CommandType.StoredProcedure };
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                list.Add(new
                {
                    clientId = rdr.GetInt32(rdr.GetOrdinal("ClientId")),
                    name = rdr.GetString(rdr.GetOrdinal("Name")),
                    document = rdr.IsDBNull(rdr.GetOrdinal("Document")) ? null : rdr.GetString(rdr.GetOrdinal("Document")),
                    email = rdr.IsDBNull(rdr.GetOrdinal("Email")) ? null : rdr.GetString(rdr.GetOrdinal("Email"))
                });
            }
            return list;
        }

        public async Task<List<object>> GetProductsAsync()
        {
            var list = new List<object>();
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand("dbo.sp_GetProducts", conn) { CommandType = CommandType.StoredProcedure };
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                list.Add(new
                {
                    productId = rdr.GetInt32(rdr.GetOrdinal("ProductId")),
                    name = rdr.GetString(rdr.GetOrdinal("Name")),
                    unitPrice = rdr.GetDecimal(rdr.GetOrdinal("UnitPrice")),
                    imageUrl = rdr.IsDBNull(rdr.GetOrdinal("ImageUrl")) ? null : rdr.GetString(rdr.GetOrdinal("ImageUrl"))
                });
            }
            return list;
        }

        // Obtener factura completa (cabecera + detalles) - SP devuelve 2 result sets
        public async Task<InvoiceFullDto?> GetInvoiceByNumberAsync(string number)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand("dbo.sp_GetInvoiceByNumber", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@InvoiceNumber", number);

            using var rdr = await cmd.ExecuteReaderAsync();

            if (!await rdr.ReadAsync())
                return null;

            var header = new InvoiceHeaderDto
            {
                InvoiceId = rdr.GetInt32(rdr.GetOrdinal("InvoiceId")),
                InvoiceNumber = rdr.GetString(rdr.GetOrdinal("InvoiceNumber")),
                ClientId = rdr.GetInt32(rdr.GetOrdinal("ClientId")),
                ClientName = rdr.GetString(rdr.GetOrdinal("ClientName")),
                Subtotal = rdr.GetDecimal(rdr.GetOrdinal("Subtotal")),
                Tax = rdr.GetDecimal(rdr.GetOrdinal("Tax")),
                Total = rdr.GetDecimal(rdr.GetOrdinal("Total")),
                CreatedAt = rdr.GetDateTime(rdr.GetOrdinal("CreatedAt"))
            };

            var res = new InvoiceFullDto { Header = header };

            // pasar al segundo resultado (detalles)
            if (await rdr.NextResultAsync())
            {
                while (await rdr.ReadAsync())
                {
                    var d = new InvoiceDetailResponseDto
                    {
                        InvoiceDetailId = rdr.GetInt32(rdr.GetOrdinal("InvoiceDetailId")),
                        ProductId = rdr.GetInt32(rdr.GetOrdinal("ProductId")),
                        ProductName = rdr.GetString(rdr.GetOrdinal("ProductName")),
                        Quantity = rdr.GetInt32(rdr.GetOrdinal("Quantity")),
                        UnitPrice = rdr.GetDecimal(rdr.GetOrdinal("UnitPrice")),
                        Total = rdr.GetDecimal(rdr.GetOrdinal("Total"))
                    };
                    res.Details.Add(d);
                }
            }

            return res;
        }

        public async Task<List<object>> SearchInvoicesAsync(string type, int? clientId, string? number, DateTime? from, DateTime? to)
        {
            var list = new List<object>();
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand("dbo.sp_SearchInvoices", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@SearchType", type);
            cmd.Parameters.AddWithValue("@ClientId", (object?)clientId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@InvoiceNumber", (object?)number ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FromDate", (object?)from ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate", (object?)to ?? DBNull.Value);

            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                list.Add(new
                {
                    invoiceId = rdr.GetInt32(rdr.GetOrdinal("InvoiceId")),
                    invoiceNumber = rdr.GetString(rdr.GetOrdinal("InvoiceNumber")),
                    clientName = rdr.GetString(rdr.GetOrdinal("ClientName")),
                    subtotal = rdr.GetDecimal(rdr.GetOrdinal("Subtotal")),
                    tax = rdr.GetDecimal(rdr.GetOrdinal("Tax")),
                    total = rdr.GetDecimal(rdr.GetOrdinal("Total")),
                    createdAt = rdr.GetDateTime(rdr.GetOrdinal("CreatedAt"))
                });
            }
            return list;
        }
    }
}