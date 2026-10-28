using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class AsistenteRepositorio(EventosContext context)
{
    public Task<List<Asistente>> ObtenerActivosAsync() =>
        context.Asistentes.AsNoTracking()
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombres)
            .ToListAsync();

    public async Task CrearAsync(Asistente asistente)
    {
        context.Asistentes.Add(asistente);
        await context.SaveChangesAsync();
    }

    public async Task<bool> DesactivarAsync(int id)
    {
        var asistente = await context.Asistentes.FirstOrDefaultAsync(a => a.Id == id && a.Activo);
        if (asistente is null)
        {
            return false;
        }

        asistente.Activo = false;
        await context.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExisteDniAsync(string dni) =>
        context.Asistentes.AsNoTracking().AnyAsync(a => a.Dni == dni);

    public Task<bool> ExisteCorreoAsync(string correo) =>
        context.Asistentes.AsNoTracking().AnyAsync(a => a.Correo == correo);
}
