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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Eventos}/{action=Index}/{id?}");

await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EventosContext>();
    await context.Database.MigrateAsync();
    await SeedDataAsync(context);
}

app.Run();

static async Task SeedDataAsync(EventosContext context)
{
    if (!await context.Categorias.AnyAsync())
    {
        context.Categorias.AddRange(
            new Categoria { Nombre = "Conferencias" },
            new Categoria { Nombre = "Talleres" },
            new Categoria { Nombre = "Networking" });
        await context.SaveChangesAsync();
    }

    if (!await context.Eventos.AnyAsync())
    {
        var categorias = await context.Categorias.AsNoTracking().OrderBy(c => c.Id).ToListAsync();
        context.Eventos.AddRange(
            new Evento { Titulo = "Congreso de tecnología", Fecha = DateTime.Today.AddDays(30), Lugar = "Auditorio Central", Aforo = 300, Precio = 45.00m, Modalidad = "Presencial", CategoriaId = categorias[0].Id },
            new Evento { Titulo = "Desarrollo web moderno", Fecha = DateTime.Today.AddDays(45), Lugar = "Sala Innovación", Aforo = 80, Precio = 25.00m, Modalidad = "Presencial", CategoriaId = categorias[1].Id },
            new Evento { Titulo = "Cloud y .NET", Fecha = DateTime.Today.AddDays(60), Lugar = "En línea", Aforo = 500, Precio = 0.00m, Modalidad = "Virtual", CategoriaId = categorias[0].Id },
            new Evento { Titulo = "Encuentro de profesionales", Fecha = DateTime.Today.AddDays(75), Lugar = "Centro de Convenciones", Aforo = 150, Precio = 15.00m, Modalidad = "Presencial", CategoriaId = categorias[2].Id });
        await context.SaveChangesAsync();
    }

    if (!await context.Asistentes.AnyAsync())
    {
        context.Asistentes.AddRange(
            new Asistente { Nombres = "Ana Torres", Dni = "12345678", Correo = "ana@example.com", Telefono = "987654321" },
            new Asistente { Nombres = "Luis Gómez", Dni = "23456789", Correo = "luis@example.com" },
            new Asistente { Nombres = "María Rojas", Dni = "34567890", Correo = "maria@example.com" },
            new Asistente { Nombres = "Carlos Pérez", Dni = "45678901", Correo = "carlos@example.com" },
            new Asistente { Nombres = "Sofía Díaz", Dni = "56789012", Correo = "sofia@example.com" });
        await context.SaveChangesAsync();
    }
}
