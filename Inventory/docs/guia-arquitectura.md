# Guía de arquitectura del backend Inventory

Esta guía explica **cómo está organizado el backend y por qué**. Está pensada
para aprender: cada sección usa código real de este repositorio.

---

## 1. La idea central: Clean Architecture

Cada capa responde **una sola pregunta**:

| Capa | Proyecto | Pregunta que responde |
|---|---|---|
| **Domain** | `Inventory.Domain` | ¿**Qué** cosas existen en el negocio y qué reglas cumplen? |
| **Application** | `Inventory.Application` | ¿**Qué** puede hacer el sistema, paso a paso? |
| **Infrastructure** | `Inventory.Infrastructure` + `Inventory.Infrastructure.Persistence.Mysql` | ¿**Cómo** se hace lo técnico (BD, JWT, BCrypt, caché)? |
| **Api** | `Inventory.Api` | ¿**Por dónde** entra y sale la información (HTTP)? |

### La regla de oro: las dependencias apuntan hacia adentro

```
                 Inventory.Api  (HTTP, controllers, middlewares, Program.cs)
                 │        │        │
                 ▼        │        ▼
   Inventory.Application  │   Inventory.Infrastructure          (JWT, BCrypt, caché)
                 │        │        │  implementa interfaces de Application
                 ▼        ▼        ▼
                  Inventory.Domain  ◄── Inventory.Infrastructure.Persistence.Mysql
                  (no depende de nadie)      (EF Core: implementa los repositorios)
```

- **Domain no conoce a nadie**: ni EF Core, ni HTTP, ni JWT.
- **Application** solo conoce a Domain. Cuando necesita algo técnico, pide una
  **interfaz** (`IPasswordHasher`, `IUserRepository`) y otro la implementa.
- **Infrastructure** implementa esas interfaces.
- **Api** es la *raíz de composición*: en `Program.cs` conecta todo con
  `AddApplication()`, `AddInfrastructure()` y `AddPersistence()`.

---

## 2. Qué va en cada capa

### Domain — "las reglas del juego"

| Carpeta | Qué contiene |
|---|---|
| `*/Entity/` | Entidades con sus reglas (`UserEntity.CanLogin()`, `ModuleEntity.Deactivate()`) |
| `*/Repositories/` | **Contratos** de acceso a datos (`IUserRepository`) |
| `Modules/ModuleMenuBuilder.cs` | Servicio de dominio: regla que necesita ver muchos módulos a la vez |
| `*/…Errors.cs` | Errores que detecta la propia entidad |
| `common/Results/` | `Result<T, Error>`, `Error`, `ErrorType` |
| `common/Persistence/` | `IUnitOfWork`, `DuplicateRecordException` |

**Nunca va aquí:** EF Core, `HttpStatusCode`, controllers, JWT, SQL.

### Application — "las jugadas"

| Carpeta | Qué contiene |
|---|---|
| `*/Command/*` | Casos de uso que **cambian** algo (+ su `Validator`) |
| `*/Query/*` | Casos de uso que solo **leen** |
| `*/Dto/` | Lo que se devuelve hacia afuera |
| `*/Errors/` | Errores de cada caso de uso (email repetido, rol inexistente…) |
| `common/Interfaces/` | Contratos técnicos (`IPasswordHasher`, `IJwtTokenGenerator`, cachés) |
| `common/Behaviors/` | `ValidationBehavior`: se ejecuta antes de cada handler |

**Nunca va aquí:** `DbContext`, LINQ contra la BD, `HttpContext`, códigos HTTP.

### Infrastructure — "la nevera y la estufa"

- `Inventory.Infrastructure.Persistence.Mysql`: `DataBaseContext`,
  `EntityConfiguration/`, `Repositories/` y `UnitOfWork`.
- `Inventory.Infrastructure`: `Security/` (JWT, BCrypt) y `Caching/`.

**Nunca va aquí:** reglas de negocio ("quién puede ver qué", "cómo se arma un código").

### Api — "el mesero"

`Controllers/` (heredan de `ApiControllerBase`), `Middleware/`, `Tenancy/`,
`Common/Responses/` (el sobre `{ statusCode, success, message, data }`) y `Program.cs`.

**Nunca va aquí:** lógica de negocio ni consultas a la BD.

---

## 3. Los dos tipos de lógica

| Lógica de **dominio** (reglas) | Lógica de **aplicación** (orquestación) |
|---|---|
| "¿Qué es válido?" | "¿Qué hago primero y qué después?" |
| Solo usa los datos de la entidad | Llama a repositorios, hasher, token… |
| `user.CanLogin()` | Buscar usuario → verificar contraseña → generar token |

**Prueba rápida:** *¿Esta línea necesita algo de afuera (BD, BCrypt, token)?*
Sí → Application. No, solo usa datos de la entidad → Domain.

**Caso especial — reglas sobre el conjunto:** "no puede haber dos usuarios con
el mismo email" es una regla de negocio, pero una entidad no puede ver a las
demás. Por eso la comprueba el handler (`EmailExistsAsync`) y la base de datos
la garantiza con un índice único. Si dos peticiones chocan al mismo tiempo,
`UnitOfWork` traduce el error de SQL Server a `DuplicateRecordException` y la
API responde 409.

---

## 4. Patrones de diseño usados

| Patrón | Dónde | Para qué |
|---|---|---|
| **Mediator** | MediatR (`mediator.Send`) | El controller no conoce al handler |
| **CQRS** | `Command/` vs `Query/` | Separar escritura de lectura |
| **Pipeline / Decorator** | `ValidationBehavior` | Validar antes de cada handler sin repetir código |
| **Repository** | `IUserRepository`, `IRoleRepository`… | Esconder cómo se accede a los datos |
| **Unit of Work** | `IUnitOfWork` | Guardar todos los cambios en una transacción |
| **Result** | `Result<T, Error>` | Errores esperados sin excepciones |
| **Factory Method** | `UserEntity.Create`, `Error.CreateInstance` | Crear objetos siempre válidos |
| **Rich Domain Model** | Entidades con `private set` y métodos | Que nadie se salte una regla |
| **Domain Service** | `ModuleMenuBuilder` | Reglas que no caben en una sola entidad |
| **Chain of Responsibility** | Middlewares | Cada eslabón decide si deja pasar la petición |
| **Options** | `TenantRegistryOptions`, `JwtOptions` | Configuración tipada |
| **Cache-aside** | `PermissionCache`, `ActiveUserCache` | Caché primero, BD si no está |

**Excepciones vs Result:** los errores *esperados* (email repetido) viajan como
`Result` y los traduce `ApiControllerBase.FromResult`. Las excepciones son para
lo inesperado o lo que un componente genérico no puede expresar como `Result`
(la validación del pipeline); las atrapa `GlobalExceptionHandler`.

---

## 5. Recorrido de una petición: `POST /api/v1/users`

```
1. Api            TenantResolution → Authentication → TenantClaim → ActiveUser → PermissionClaims
2. Api            UsersController recibe el JSON como CreateUserCommand → mediator.Send
3. Application    ValidationBehavior ejecuta CreateUserCommandValidator (400 si falla)
4. Application    CreateUserCommandHandler:
                    - ¿email repetido?  → IUserRepository  (409)
                    - ¿rol existe?      → IRoleRepository  (404)
                    - UserEntity.Create(...)  ← reglas del dominio (activo, debe cambiar clave)
                    - IUserRepository.AddAsync + IUnitOfWork.SaveChangesAsync
5. Infrastructure UserRepository / UnitOfWork guardan con EF Core
6. Application    devuelve Result<UserDto, Error>
7. Api            ApiControllerBase.FromResult → 201 (o el código del ErrorType)
```

---

## 6. Qué cambió en el refactor (un commit por paso)

| Commit | Qué se hizo | Qué se aprende |
|---|---|---|
| 1/7 | Limpiar código muerto | Menos código = menos confusión |
| 2/7 | `ErrorType` en vez de `HttpStatusCode`; respuesta HTTP en Api | Domain no debe conocer HTTP |
| 3/7 | Un repositorio por entidad; middlewares sin `DbContext` | Interfaces del tamaño correcto; Api sin EF |
| 4/7 | Reglas dentro de las entidades (`private set` + métodos) | Modelo rico vs anémico |
| 5/7 | Reglas del menú y permisos fuera de Infrastructure | Infra solo trae datos |
| 6/7 | Validadores + `GlobalExceptionHandler` + `DuplicateRecordException` | Validación de forma vs reglas de negocio |
| 7/7 | Proyecto `Inventory.Infrastructure` (JWT, BCrypt, caché) | Lo técnico no vive en Api |

Para ver el detalle de cada paso: `git show <commit>`.

### Lo que NO se hizo (y por qué)

- **Renombrar `Persistence.Mysql`**: el proyecto usa SQL Server, pero renombrarlo
  toca todos los namespaces sin enseñar nada nuevo. Se puede hacer cuando quieras.
- **AutoMapper, repositorio genérico, Specification, Domain Events**: no hacen
  falta para un sistema de este tamaño; agregarían complejidad sin beneficio.
