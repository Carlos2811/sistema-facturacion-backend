# 📌 README para el **Backend (ASP.NET Core Web API)**

`/backend/README.md`

```markdown
# ⚙️ Sistema de Facturación - Backend (ASP.NET Core Web API)

Este es el **backend** de la aplicación de facturación, desarrollado en **ASP.NET Core Web API (.NET 7/8)** con SQL Server.  
Expone endpoints REST para gestionar **facturas, clientes y productos**.

---

## 🚀 Tecnologías usadas
- ASP.NET Core Web API (.NET 7/8)
- C#
- ADO.NET (Microsoft.Data.SqlClient)
- SQL Server
- Swagger (documentación de APIs)
- CORS habilitado para Angular (http://localhost:4200)

---

## 📦 Instalación y ejecución

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/tuusuario/sistema-facturacion.git
   cd sistema-facturacion/backend
Restaurar dependencias:

bash
Copiar código
dotnet restore
Ejecutar el servidor:

bash
Copiar código
dotnet run
Acceder a Swagger:
https://localhost:7123/swagger

🗄️ Base de datos
Crear la base de datos DevLab en SQL Server.

Ejecutar los scripts SQL incluidos en /db:

01_create_tables.sql

02_create_procedures.sql

03_seed_data.sql

Ajustar la cadena de conexión en appsettings.json:

json
Copiar código
"ConnectionStrings": {
  "DefaultConnection": "Server=andres;Database=DevLab;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
}
📁 Estructura principal
bash
Copiar código
InvoiceApi/
 ├── Controllers/
 │    ├── InvoiceController.cs   # Endpoints para facturas
 │    ├── ClientsController.cs   # Endpoints para clientes
 │    └── ProductsController.cs  # Endpoints para productos
 │
 ├── DTOs/                       # Modelos de transferencia de datos
 ├── Data/                       # SqlHelper + Repositorios ADO.NET
 ├── Services/                   # Lógica de negocio
 ├── Program.cs                  # Configuración principal
 └── appsettings.json             # Configuración (cadena de conexión)
✅ Endpoints principales
Facturas
POST /api/invoice → Crear nueva factura.

GET /api/invoice/{number} → Consultar factura por número.

GET /api/invoice/search → Buscar facturas por cliente, número o fechas.

Clientes
GET /api/clients → Listar clientes.

Productos
GET /api/products → Listar productos.

🛠️ Scripts útiles
dotnet build → Compila la solución.

dotnet run → Ejecuta el servidor.

dotnet watch run → Ejecuta con hot reload.

