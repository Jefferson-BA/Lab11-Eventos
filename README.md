# EF Core Code First: Eventos e inscripciones

Aplicación MVC en ASP.NET Core 8, proyecto y ensamblado `Eventos.Web`. La base `EventosDB` se construye con migraciones de Entity Framework Core; el mapeo se configura con Data Annotations, sin `OnModelCreating`, Fluent API ni consultas SQL manuales.

## Requisitos y ejecución

- .NET 8 SDK.
- SQL Server Express accesible como `LAPTOP-T466KP2I\SQLEXPRESS` con autenticación de Windows (o actualizar `DefaultConnection` en `Lab11/appsettings.json` y `Lab11/appsettings.Development.json`).
- Paquetes `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Design` y `Microsoft.EntityFrameworkCore.Tools` 8.0.22.

Desde la raíz del repositorio:

```powershell
dotnet restore .\Lab11\Eventos.Web.csproj
dotnet run --project .\Lab11\Eventos.Web.csproj
```

Al iniciar, la aplicación ejecuta `Database.MigrateAsync()` y después carga, mediante `AnyAsync()`, tres categorías, cuatro eventos y cinco asistentes cuando cada tabla correspondiente está vacía.

## Comandos de migración utilizados (Visual Studio / PMC y dotnet CLI)

Las migraciones se crean en orden, al incorporar el modelo base y luego cada propiedad evolutiva:

| Etapa | Visual Studio / Package Manager Console | .NET CLI |
|---|---|---|
| 1. Esquema inicial | `Add-Migration InitialCreate` | `dotnet ef migrations add InitialCreate` |
| Aplicar | `Update-Database` | `dotnet ef database update` |
| 2. Modalidad | `Add-Migration AgregarModalidadEvento` | `dotnet ef migrations add AgregarModalidadEvento` |
| Aplicar | `Update-Database` | `dotnet ef database update` |
| 3. Teléfono | `Add-Migration AgregarTelefonoAsistente` | `dotnet ef migrations add AgregarTelefonoAsistente` |
| Aplicar | `Update-Database` | `dotnet ef database update` |

En Visual Studio, seleccionar `Eventos.Web` como proyecto predeterminado de PMC y como proyecto de inicio. Para la CLI, ejecutar los comandos desde `Lab11` o agregar `--project .\Lab11\Eventos.Web.csproj`.

1. `InitialCreate` crea las tablas de las cuatro entidades base, sus claves, relaciones e índices únicos.
2. `AgregarModalidadEvento` agrega `Evento.Modalidad`. **Editar manualmente el archivo de migración** para que el `AddColumn` no nullable use `defaultValue: "Presencial"` (en lugar de la cadena vacía), conservando el valor de los registros existentes.
3. `AgregarTelefonoAsistente` agrega `Asistente.Telefono` como columna nullable.

Los archivos de las tres migraciones y el snapshot de EF Core están incluidos en `Lab11/Migrations`.

## Repositorios y controladores

- `EventoRepositorio`: consulta solo eventos activos, admite filtro por categoría y orden por fecha. Proyecta la cantidad de inscripciones confirmadas y calcula cupos disponibles; incluye categoría y confirmados en detalles. Crea, edita y desactiva eventos, y entrega categorías activas para los selectores.
- `AsistenteRepositorio`: lista asistentes activos por nombre, crea asistentes, comprueba DNI y correo duplicados y desactiva asistentes con `Activo = false`.
- `InscripcionRepositorio`: incluye evento y asistente en lecturas, crea inscripciones confirmadas, cancela cambiando el estado a `Cancelada` y valida fecha del evento, duplicidad de confirmación y aforo.
- `EventosController`, `AsistentesController` e `InscripcionesController` dependen únicamente de sus repositorios. Las operaciones de formulario usan POST/Redirect/GET y `TempData`; los errores de DNI, correo y las reglas de inscripción vuelven al formulario mediante `ModelState.AddModelError`.

Las lecturas de repositorio usan `AsNoTracking()`. Las desactivaciones son lógicas; no se borran registros ni se elimina el historial de inscripciones. `Categoria` también expone la operación de desactivación lógica en el repositorio de eventos.

## Vistas y Layout

Las vistas Razor fuertemente tipadas están organizadas en `Views/Eventos`, `Views/Asistentes` y `Views/Inscripciones`, con formularios, validación, filtros, detalles, cupos y estado de inscripción. `_Layout.cshtml` usa Bootstrap e incluye navegación superior a Eventos, Asistentes e Inscripciones; los mensajes `TempData` se muestran como alertas.

## Consulta SQL para verificación de evidencia

Después de aplicar las migraciones a SQL Server, ejecutar en `EventosDB`:

```sql
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
```

## Observaciones y conclusiones técnicas

- La base y su historial se administran exclusivamente mediante migraciones Code First.
- Los índices únicos de categoría, DNI y correo están definidos con `[Index(..., IsUnique = true)]`; `Precio` usa `decimal(10,2)`.
- La integridad de datos se valida en modelos y controladores; las lecturas son asíncronas y sin seguimiento.
- Para ejecutar la aplicación y verificar `__EFMigrationsHistory`, SQL Server debe estar iniciado y accesible con la cadena configurada.
