using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class InscripcionRepositorio(EventosContext context)
{
    public Task<List<Inscripcion>> ObtenerTodasAsync() =>
        context.Inscripciones.AsNoTracking()
            .Include(i => i.Evento)
            .Include(i => i.Asistente)
            .OrderByDescending(i => i.FechaInscripcion)
            .ToListAsync();

    public async Task CrearAsync(int eventoId, int asistenteId)
    {
        context.Inscripciones.Add(new Inscripcion
        {
            EventoId = eventoId,
            AsistenteId = asistenteId,
            FechaInscripcion = DateTime.Now,
            Estado = "Confirmada"
        });
        await context.SaveChangesAsync();
    }

    public async Task<bool> CancelarAsync(int id)
    {
        var inscripcion = await context.Inscripciones
            .FirstOrDefaultAsync(i => i.Id == id && i.Estado == "Confirmada");
        if (inscripcion is null)
        {
            return false;
        }

        inscripcion.Estado = "Cancelada";
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EventoYaPasoAsync(int eventoId)
    {
        var fecha = await context.Eventos.AsNoTracking()
            .Where(e => e.Id == eventoId && e.Activo)
            .Select(e => (DateTime?)e.Fecha)
            .FirstOrDefaultAsync();
        return !fecha.HasValue || fecha.Value < DateTime.Now;
    }

    public Task<bool> YaInscritoAsync(int eventoId, int asistenteId) =>
        context.Inscripciones.AsNoTracking()
            .AnyAsync(i => i.EventoId == eventoId && i.AsistenteId == asistenteId && i.Estado == "Confirmada");

    public async Task<bool> AforoCompletoAsync(int eventoId)
    {
        var evento = await context.Eventos.AsNoTracking()
            .Where(e => e.Id == eventoId && e.Activo)
            .Select(e => new
            {
                e.Aforo,
                Confirmados = e.Inscripciones.Count(i => i.Estado == "Confirmada")
            })
            .FirstOrDefaultAsync();
        return evento is null || evento.Confirmados >= evento.Aforo;
    }

    public Task<List<Evento>> ObtenerEventosActivosAsync() =>
        context.Eventos.AsNoTracking()
            .Where(e => e.Activo)
            .OrderBy(e => e.Fecha)
            .ToListAsync();

    public Task<List<Asistente>> ObtenerAsistentesActivosAsync() =>
        context.Asistentes.AsNoTracking()
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombres)
            .ToListAsync();
}
