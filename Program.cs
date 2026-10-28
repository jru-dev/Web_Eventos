using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

var app = builder.Build();

// ---------- Datos iniciales ----------
// Se ejecuta DESPUÉS de aplicar las migraciones (dotnet ef database update).
// Solo inserta datos si la tabla correspondiente está vacía.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EventosContext>();

    if (!await context.Categorias.AnyAsync())
    {
        context.Categorias.AddRange(
            new Categoria { Nombre = "Tecnología" },
            new Categoria { Nombre = "Negocios" },
            new Categoria { Nombre = "Arte y Cultura" });
        await context.SaveChangesAsync();
    }

    if (!await context.Eventos.AnyAsync())
    {
        var categorias = await context.Categorias.AsNoTracking().ToListAsync();
        int Cat(string nombre) => categorias.First(c => c.Nombre == nombre).Id;

        context.Eventos.AddRange(
            new Evento
            {
                Titulo = "Congreso de Desarrollo de Software",
                Fecha = DateTime.Today.AddDays(15).AddHours(9),
                Lugar = "Auditorio TECSUP, Lima",
                Aforo = 3,
                Precio = 50.00m,
                CategoriaId = Cat("Tecnología"),
                Modalidad = "Presencial"
            },
            new Evento
            {
                Titulo = "Taller de Emprendimiento",
                Fecha = DateTime.Today.AddDays(20).AddHours(15),
                Lugar = "Zoom",
                Aforo = 100,
                Precio = 0m,
                CategoriaId = Cat("Negocios"),
                Modalidad = "Virtual"
            },
            new Evento
            {
                Titulo = "Feria de Arte Contemporáneo",
                Fecha = DateTime.Today.AddDays(30).AddHours(11),
                Lugar = "Centro Cultural, Lima",
                Aforo = 500,
                Precio = 15.50m,
                CategoriaId = Cat("Arte y Cultura"),
                Modalidad = "Presencial"
            },
            new Evento
            {
                Titulo = "Meetup de Inteligencia Artificial",
                Fecha = DateTime.Today.AddDays(10).AddHours(19),
                Lugar = "Google Meet",
                Aforo = 200,
                Precio = 10.00m,
                CategoriaId = Cat("Tecnología"),
                Modalidad = "Virtual"
            });
        await context.SaveChangesAsync();
    }

    if (!await context.Asistentes.AnyAsync())
    {
        context.Asistentes.AddRange(
            new Asistente { Nombres = "Ana Torres Quispe", Dni = "70123456", Correo = "ana.torres@correo.com", Telefono = "987654321" },
            new Asistente { Nombres = "Luis Ramírez Soto", Dni = "70234567", Correo = "luis.ramirez@correo.com", Telefono = "976543210" },
            new Asistente { Nombres = "María Fernández Díaz", Dni = "70345678", Correo = "maria.fernandez@correo.com" },
            new Asistente { Nombres = "Carlos Mendoza Rojas", Dni = "70456789", Correo = "carlos.mendoza@correo.com", Telefono = "965432109" },
            new Asistente { Nombres = "Lucía Paredes Vega", Dni = "70567890", Correo = "lucia.paredes@correo.com" });
        await context.SaveChangesAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Eventos/Index");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Eventos}/{action=Index}/{id?}");

app.Run();
