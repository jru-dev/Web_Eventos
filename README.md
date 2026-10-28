# Lab S11 – EF Core Code First: Eventos e inscripciones

Proyecto ASP.NET Core MVC (`Eventos.Web`) con EF Core Code First, repositorios y 3 migraciones.

## 1. Requisitos (una sola vez)

- .NET SDK 8 (`dotnet --version` debe mostrar 8.x)
- SQL Server (Express / Developer / LocalDB) y SSMS o Azure Data Studio
- VS Code con la extensión **C# Dev Kit**
- Herramienta de migraciones:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.11
```

> Si ya la tienes instalada: `dotnet tool update --global dotnet-ef --version 8.0.11`
> Si tu SDK es 9 o 10, cambia `TargetFramework` y la versión de los 2 paquetes en `Eventos.Web.csproj` a esa versión (y instala el mismo `dotnet-ef`).

## 2. Cadena de conexión

Edita `Eventos.Web/appsettings.json` según tu servidor:

| Tu SQL Server | Server= |
|---|---|
| Instancia por defecto | `localhost` |
| SQL Express | `localhost\\SQLEXPRESS` |
| LocalDB | `(localdb)\\MSSQLLocalDB` |

Ya incluye `TrustServerCertificate=True`. Si usas usuario/clave: `User Id=sa;Password=TU_CLAVE;` en lugar de `Trusted_Connection=True;`.

## 3. Ejecutar (con las 3 migraciones ya incluidas)

```powershell
cd Eventos.Web
dotnet restore
dotnet build
dotnet ef migrations list
dotnet ef database update
dotnet run
```

Abre http://localhost:5080

**Orden importante:** `dotnet ef database update` va ANTES de `dotnet run`, porque los datos iniciales (Program.cs) se cargan al arrancar y necesitan que las tablas ya existan.

## 4. Evidencia para el informe

En SQL Server, sobre la base `EventosDB`:

```sql
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
```

Deben salir 3 filas:

```
20261028210000_InitialCreate
20261028210100_AgregarModalidadEvento
20261028210200_AgregarTelefonoAsistente
```

Captura también la carpeta `Migrations` expandida en VS Code (se ven los 3 archivos `.cs`, sus `.Designer.cs` y `EventosContextModelSnapshot.cs`).

Comandos de migración a adjuntar en el informe:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet ef migrations add AgregarModalidadEvento      # defaultValue cambiado a "Presencial"
dotnet ef database update
dotnet ef migrations add AgregarTelefonoAsistente
dotnet ef database update
```

## 5. Capturas de vistas sugeridas

Eventos (Index con filtro, Create, Edit, Details, Delete), Asistentes (Index, Create con error de DNI/correo duplicado), Inscripciones (Index con botón Cancelar, Create con error de aforo / ya inscrito / evento pasado).

## 6. Pruebas rápidas

- El evento "Congreso de Desarrollo de Software" tiene aforo 3: inscribe 3 asistentes y el cuarto debe mostrar "El aforo del evento está completo".
- Inscribir dos veces al mismo asistente en el mismo evento muestra el error de duplicado.
- Cancelar una inscripción libera un cupo (se ve en Eventos → Index).
- Eliminar un evento lo oculta del Index, pero la fila sigue en SQL con `Activo = 0`.

## 7. Estructura

```
Eventos.Web/
  Controllers/   Eventos, Asistentes, Inscripciones
  Data/          EventosContext (sin Fluent API)
  Migrations/    3 migraciones + EventosContextModelSnapshot
  Models/        Categoria, Evento, Asistente, Inscripcion (solo Data Annotations)
  Repositorios/  EventoRepositorio, AsistenteRepositorio, InscripcionRepositorio
  ViewModels/    EventoItem
  Views/         Eventos, Asistentes, Inscripciones, Shared/_Layout
  Program.cs     DI, AddDbContext, AddScoped, datos iniciales
```
