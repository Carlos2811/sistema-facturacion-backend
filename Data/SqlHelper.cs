using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace InvoiceApi.Data
{
    public class SqlHelper
    {
        private readonly string _conn;
        public SqlHelper(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_conn);
        }
    }
}