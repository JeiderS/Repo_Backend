# 🧾 FleetManager - Backend (.NET 8 + Keycloak)

# 📚 Características

- 🔐 Autenticación con **Keycloak** (OAuth2)
- 🧩 Arquitectura limpia con Repository CQRS Mediator y Command
- 🛢️ Acceso a datos con **Entity Framework Core**
- 🧪 Migraciones para gestión de esquema de base de datos
- 🧾 Documentación de API con **Swagger**
- 🐳 Contenedores Docker para despliegue
- 💾 Backup de base de datos en formato SQL incluido

## 🧰 Tecnologías Usadas

- C# (.NET 8)
- Entity Framework Core
- MySQL
- Keycloak
- Swagger
- Docker

# 🛠️ Requisitos Previos

- [.NET SDK 8](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/)
- [Git](https://git-scm.com/)
- Keycloak (Docker o ya configurado)
- MySQL

 # Descargar
 [Descargar realm-export.json](./keycloak/realm-export.json)
 [Descargar Backup SQL](./backup/fleetmanager-backup.sql)

## 🚀 Instrucciones de Despliegue (Docker)

### 1. Clonar el repositorio
```
git clone https://github.com/jeider05/BackendFleet.git
cd FleetManager

3. Construir la imagen del backend
docker build -t fleetmanager-api .
4. Ejecutar el contenedor
docker run -p 8081:8081 fleetmanager-api
```
URL API:
http://localhost:8081

🔐 Keycloak
Se recomienda tener un Keycloak corriendo por separado (por Docker o instancia propia) y luego importar manualmente el archivo realm-export.json.

Para iniciar Keycloak con configuración básica:
📄 [Descargar realm-export.json](./keycloak/realm-export.json)

Iniciar Keycloak con configuración básica en Docker:

```
docker run -p 8080:8080 \
    -e KEYCLOAK_ADMIN=admin \
    -e KEYCLOAK_ADMIN_PASSWORD=admin \
    quay.io/keycloak/keycloak:24.0.1 \
    start-dev
```
URL Admin Panel:
http://localhost:8080

Le dan Click en 
Realm settings
Partial import

![image](https://github.com/user-attachments/assets/c8e634b5-81ad-467a-b1b4-c55c52cd8a2a)

por ultimo suben el archivo 
![image](https://github.com/user-attachments/assets/50fea07b-bc5d-48d7-b12c-fa10ae6e5110)

Haora pueden generar un token JWT usando este enpoint desde postman 

Post http://localhost:8080/realms/App/protocol/openid-connect/token

Con los siguientes parámetros (formulario x-www-form-urlencoded):

```
Clave	Valor
grant_type	password
client_id	admin
username	admin
password	password
```
🛢️ Backup de la Base de Datos
Se incluye un archivo fleetmanager_backup.sql con la estructura y datos de ejemplo de la base de datos MySQL utilizada en el proyecto.

📥 Descargar Backup
📄 [Descargar Backup SQL](./backup/fleetmanager-backup.sql)

## 🗂️ Estructura del Proyecto

```
📦 FleetManager
├── 📁 Api                       # Proyecto principal de la API REST
│   ├── Controllers             # Controladores HTTP
│   └── Program.cs             # Configuración inicial y dependencias
├── 📁 Application              # Lógica de negocio (CQRS)
│   ├── Commands                # Comandos (escrituras)
│   ├── Queries                 # Consultas (lecturas)
│   ├── Dtos                    # Objetos de transferencia de datos
│   └── Interfaces              # Interfaces de servicios
├── 📁 Domain                   # Entidades del dominio
│   └── Entities                # Clases que representan las tablas
├── 📁 Infrastructure           # Capa de acceso a datos
│   ├── Persistence             # Implementación de interfaces
│   └── Context                 # DbContext con EF Core
├── 📁 keycloak                 # Archivo de configuración del realm
│   └── realm-export.json      # Realm exportado de Keycloak
├── 📁 backup                   # Copia de seguridad de base de datos
│   └── fleetmanager-backup.sql # Dump SQL de la base de datos
```









