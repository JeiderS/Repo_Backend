# 🧾 Inventory — Backend (.NET 8)

API REST multi-tenant para inventario, con usuarios, roles y permisos por acción.

## 📚 Características

- 🧩 **Clean Architecture** (Domain, Application, Infrastructure, Api) con **CQRS** sobre **MediatR**
- 🧠 Modelo de dominio rico: las reglas de negocio viven dentro de las entidades
- 🔐 Autenticación con **JWT propio** (contraseñas con BCrypt) y autorización por códigos de acción (`UsersView`, `RolesEdit`…)
- 🏢 **Multi-tenant**: el tenant se resuelve por el dominio de la petición y cada tenant tiene su propia base de datos
- ✅ Validación con **FluentValidation** y manejo global de errores con un formato de respuesta único
- 🛢️ **Entity Framework Core** sobre **SQL Server**; el esquema se gestiona con scripts SQL versionados (ver `Inventory/Sql/README.md`)
- 🧾 Documentación de API con **Swagger**

## 🧰 Tecnologías

C# / .NET 8 · ASP.NET Core · EF Core 8 · SQL Server · MediatR · FluentValidation · BCrypt · JWT · Swagger · xUnit

## 🗂️ Estructura

```
Inventory/
├── Inventory.Domain/                          # Entidades, reglas de negocio, contratos de repositorio
├── Inventory.Application/                     # Casos de uso (Commands/Queries), validadores, DTOs
├── Inventory.Infrastructure/                  # JWT, BCrypt, caché
├── Inventory.Infrastructure.Persistence.Mysql/# EF Core: DbContext, configuraciones, repositorios (SQL Server)
├── Inventory.Api/                             # Controllers, middlewares, tenancy, Program.cs
├── Inventory.Tests/                           # Pruebas unitarias de dominio + integración
├── Sql/                                       # Scripts SQL por tenant (bootstrap y migraciones)
└── docs/
    ├── guia-arquitectura.md                   # 👈 Empieza aquí: qué va en cada capa y por qué
    └── authorization-architecture.md
```

## 🚀 Ejecutar en local

1. Prepara una base de datos SQL Server por tenant ejecutando los scripts de
   `Inventory/Sql/TenantBootstrap/` en orden (detalles en `Inventory/Sql/README.md`).
2. Configura `Inventory/Inventory.Api/appsettings.json` (o, mejor, *user secrets*):
   - `Tenants:Registry:<tenant>:ConnectionString` para cada tenant
   - `Jwt:Key` (clave larga y secreta), `Jwt:Issuer`, `Jwt:Audience`
3. Compila y ejecuta:

```bash
cd Inventory
dotnet restore
dotnet build
dotnet run --project Inventory.Api
```

Swagger queda en la raíz: `http://localhost:8081`.

## 🧪 Pruebas

```bash
cd Inventory
dotnet test
```

- Las pruebas de `Inventory.Tests/Domain` **no necesitan base de datos**.
- Las de integración necesitan bases reales por tenant. Configura
  `INVENTORY_TEST_DEFAULT_CONNECTION`, `INVENTORY_TEST_ACME_CONNECTION` e
  `INVENTORY_TEST_GLOBEX_CONNECTION`; sin ellas, esas pruebas fallan con un
  mensaje `BLOCKED` explicando qué falta.
