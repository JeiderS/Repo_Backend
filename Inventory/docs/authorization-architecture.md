# Arquitectura de Usuarios, Roles y Autorización

Documenta el sistema de gestión de usuarios y autorización granular implementado en dos cambios SDD:

- **Fase 1 — `user-management`** (commit `3973d54`): gestión de usuarios/roles admin-only, catálogo de Actions, cierre del auto-registro público.
- **Fase 2 — `claims-authorization-cutover`** (commits `f306655`, `fbe0cc1`): autorización real basada en Actions vía claims dinámicos, retiro del modelo viejo (`RoleModules`/`HasPermissionAttribute`).

Ambas archivadas en el repo frontend: `openspec/changes/archive/2026-08-23-{user-management,claims-authorization-cutover}/`.

## 1. Modelo de datos

```
Module ──1:N── Action ──N:M (RoleAction)── Role ──1:N── User
```

- **`Module`** (`Modules`): categorías del menú (Inventario, Facturación, Usuarios, Roles, Módulos...). Se crean/editan desde la pantalla de Gestión de Módulos.
- **`Action`** (`Actions`): permisos concretos, sembrados por SQL, nunca editables desde la app. Código `{Module}{Verbo}` en PascalCase (`UsersView`, `InventoryCreate`). Se generan automáticamente por cada `Module` × {`View`, `Create`, `Edit`, `Delete`} — ver `Sql/TenantBootstrap/03_Actions_Seed.sql`.
- **`RoleAction`**: asignación de Actions a un Role. Editable desde la pantalla de Gestión de Roles (`PUT api/v1/roles/{id}/actions`).
- **`Role`**: perfil de permisos del tenant (hoy solo existe "Admin"; otros roles se crean libremente desde la UI).
- **`User.RoleId`**: **un solo rol por usuario** (FK simple, no N:M). Reasignar el rol reemplaza el anterior, no lo acumula.

### Flags especiales

- **`Role.IsSystemAdmin`** (bool): marca el rol raíz del sistema. Solo se setea por SQL (`Sql/TenantBootstrap/04_Admin_Role.sql`), ninguna pantalla lo permite tocar.
- **`Module.RequiresSystemAdmin`** (bool): un módulo con este flag solo aparece en el menú para roles con `IsSystemAdmin=true`, sin importar qué Actions tenga asignadas el rol. Hoy solo el módulo "Modules" lo tiene.

## 2. Flujo de autorización por request

```
Request (Bearer JWT)
   │
   ▼
UseAuthentication                      — valida firma/expiración del JWT
   │
   ▼
TenantClaimValidationMiddleware        — el claim "tenant" del JWT debe matchear el subdominio
   │
   ▼
ActiveUserValidationMiddleware         — rechaza si el usuario está desactivado (aunque el JWT siga vigente)
   │
   ▼
PermissionClaimsMiddleware             — ver sección 3
   │
   ▼
UseAuthorization                       — evalúa [Authorize(Roles=...)] / [Authorize(Policy=...)]
   │
   ▼
Controller
```

Cada middleware está en `Inventory.Api/Middleware/` y se registra en ese orden exacto en `Program.cs`. El orden importa: cada uno asume que el anterior ya validó lo suyo (ej: `PermissionClaimsMiddleware` no tendría sentido correr antes de saber que el tenant es el correcto).

## 3. `PermissionClaimsMiddleware` — el corazón de la Fase 2

El JWT **no lleva permisos**. Al llegar cada request autenticado, este middleware:

1. Saca el `userId` del claim `NameIdentifier`.
2. Busca su `RoleId` (con cache).
3. Busca las Actions de ese rol vía `RoleActions` (con cache) y si `Role.IsSystemAdmin`.
4. **Borra** cualquier claim de rol que el JWT ya trajera, y recién ahí agrega uno por cada código de Action (`ClaimTypes.Role = "UsersView"`, etc.) más, si corresponde, un claim separado `system_admin=true`.

### Por qué borra antes de agregar (y no solo agrega)

Los nombres de rol son texto libre — cualquier tenant admin puede crear un rol llamado literalmente `"UsersView"` o `"SystemAdmin"`. Si el middleware solo agregara claims nuevos sin sacar los viejos, un JWT emitido *antes* de este cambio (con el nombre de rol viejo como claim) podría colarse con un permiso que no le corresponde, durante todo lo que le quede de vida al token (`Jwt:ExpiresMinutes`, hoy 120 min). Por eso el reemplazo es obligatorio, no opcional.

Como los permisos se recalculan en cada request desde la base (no desde el token), un JWT emitido *antes* del deploy de este cambio sigue funcionando bien después — no hace falta forzar un re-login masivo.

### Cache

`IPermissionCache` (`Inventory.Api/Auth/PermissionCache.cs`), separado del cache de "usuario activo" de la Fase 1 (`IActiveUserCache`) porque guarda datos distintos (un set de Actions, no un booleano). TTL corto (60s), invalidado sincrónicamente cuando:
- Se reasigna el rol de un usuario (`UpdateUserCommandHandler`).
- Se desactiva o reactiva un usuario (`SetUserStatusCommandHandler`).
- Se cambian las Actions de un rol (`AssignRoleActionsCommandHandler`).

## 4. Dos formas de proteger un endpoint

| Mecanismo | Ejemplo | Significa | Delegable |
|---|---|---|---|
| `[Authorize(Roles = "UsersView")]` | `UsersController`, `RolesController`, `ActionsController` | "¿el usuario tiene esta Action asignada por su rol?" | **Sí** — cualquier admin puede dársela a cualquier rol custom desde la pantalla de Roles |
| `[Authorize(Policy = "SystemAdminOnly")]` | `ModulesController` | "¿el rol del usuario tiene `IsSystemAdmin=true`?" | **No** — no depende de ninguna Action asignable, solo del flag que nadie puede tocar desde la UI |

La policy se registra en `Program.cs`:

```csharp
options.AddPolicy(AuthorizationPolicies.SystemAdminOnly,
    p => p.RequireClaim(PermissionClaimTypes.SystemAdmin, "true"));
```

### Por qué `ModulesController` usa la policy y no una Action

Los módulos son las categorías que arman el menú de toda la aplicación. Si se protegieran con una Action normal (`ModulesCreate`, etc.), un admin podría, sin querer, tildarle esos permisos a un rol custom (aparecen en la misma lista que "ver usuarios" o "crear factura") y ese rol terminaría pudiendo borrar/crear módulos del sistema. La policy es una segunda cerradura que no se afloja desde ninguna pantalla — solo cambia con SQL directo.

### ⚠️ Si se agrega un módulo admin-only nuevo en el futuro

El sistema **no conecta esto automáticamente**. Hay dos pasos manuales independientes:

1. **Ocultarlo del menú**: sembrar la fila del módulo con `RequiresSystemAdmin=1`. Esto sí es genérico — `ModuleMenuService` ya sabe esconder cualquier módulo con ese flag a quien no sea `IsSystemAdmin`.
2. **Proteger el endpoint de verdad**: el desarrollador tiene que poner `[Authorize(Policy = AuthorizationPolicies.SystemAdminOnly)]` a mano en el controller nuevo. Marcar el módulo como `RequiresSystemAdmin=1` **no protege el backend por sí solo** — si te olvidás del paso 2, el ítem desaparece del menú pero el endpoint sigue respondiendo a cualquiera que le pegue directo a la URL.

## 5. Visibilidad del menú lateral

`ModuleMenuService.GetMenuForUserAsync` — un módulo se muestra si:

```
(el rol tiene ≥1 de las 4 Actions del módulo) AND (NOT RequiresSystemAdmin OR el rol es IsSystemAdmin)
```

Antes (Fase 1 y antes) hacía falta específicamente el permiso de "Ver". Ahora alcanza con cualquiera de los 4 (ver/crear/editar/borrar) — si un rol solo puede crear pero no ver, igual necesita ver el módulo en el menú para poder entrar a crear algo.

Los contenedores (módulos padre sin Actions propias, solo agrupan hijos) se siguen mostrando si algún hijo es visible, sin importar sus propias Actions.

## 6. Organización de los scripts SQL

```
Sql/
├── README.md                          — runbook operativo, orden exacto de ejecución
├── TenantBootstrap/                    — idempotentes, correr SIEMPRE (tenant nuevo o existente)
│   ├── 01_Actions_Schema.sql
│   ├── 02_Users_Columns.sql
│   ├── 02b_Claims_Columns.sql          — Roles.IsSystemAdmin, Modules.RequiresSystemAdmin, siembra "Modules"
│   ├── 02c_Core_Modules_Seed.sql       — siembra "Users"/"Roles" como módulos (ver nota abajo)
│   ├── 03_Actions_Seed.sql             — genera Actions = Modules × verbos
│   ├── 04_Admin_Role.sql               — asegura rol Admin, IsSystemAdmin=1, todas las Actions
│   └── 05_Admin_User_Assign.sql        — asegura que al menos 1 usuario sea Admin
└── Migrations/                         — one-shot, un tenant nuevo se las salta
    ├── 2026-08-user-management/        — colapso de roles N:M a un solo RoleId
    └── 2026-08-claims-authorization-cutover/
        ├── 01_Report_RoleModules_Drift.sql   — SOLO LECTURA, nunca inserta nada
        └── 02_Drop_RoleModules.sql           — diferido, correr solo tras verificar en producción
```

**Por qué existe `02c_Core_Modules_Seed.sql`**: se descubrió corriendo los tests contra las 3 bases reales que nunca se habían sembrado "Users" ni "Roles" como filas de `Modules` — ni en Fase 1 ni en Fase 2. Como los códigos de Action se generan a partir de las filas de `Modules`, sin esas dos filas los códigos `UsersView`/`RolesEdit`/etc. no podían existir, y con el recableo de la Fase 2 eso dejaba a **todos los roles, incluido Admin**, sin acceso a esas pantallas. El script es idempotente y sigue la misma convención que `02b`.

### Regla de reconciliación (importante)

Antes de activar la Fase 2 en un tenant con datos previos, `01_Report_RoleModules_Drift.sql` genera un reporte de solo lectura (nunca inserta ni borra nada) para que un humano revise diferencias entre el modelo viejo (`RoleModules`) y el nuevo (`RoleActions`). **Nunca se debe re-correr el script de backfill de la Fase 1** (`Migrations/2026-08-user-management/02_Backfill_RoleActions.sql`) como "arreglo" — como `RoleActions` ya se puede editar desde la pantalla de Roles desde la Fase 1, volver a correr ese INSERT podría resucitar en silencio un permiso que un admin sacó a propósito.

## 7. Qué queda pendiente

- **Checkpoint C** (`Sql/Migrations/2026-08-claims-authorization-cutover/02_Drop_RoleModules.sql`): borra la tabla vieja `RoleModules`. Diferido a propósito — recién correr después de verificar esta fase funcionando en un entorno real desplegado, tenant por tenant.
- **Cobertura de tests heredada de Fase 1**: hay escenarios del spec de gestión de usuarios (email duplicado, auto-desactivación, etc.) sin test que los pruebe. No los toca este cambio, quedaron aceptados como fuera de alcance.
- **`ModulesController`**: ya protegido con la policy (ver sección 4) — el gap original de Fase 1 quedó resuelto acá.
