using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class EventoRepositorio(EventosContext context)
{
    public async Task<List<EventoListaViewModel>> ObtenerActivosAsync(int? categoriaId = null)
    {
        var query = context.Eventos.AsNoTracking()
            .Where(e => e.Activo)
            .AsQueryable();

        if (categoriaId.HasValue)
        {
            query = query.Where(e => e.CategoriaId == categoriaId.Value);
        }

        return await query
            .OrderBy(e => e.Fecha)
            .Select(e => new EventoListaViewModel
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Fecha = e.Fecha,
                Lugar = e.Lugar,
                Aforo = e.Aforo,
                Precio = e.Precio,
                Modalidad = e.Modalidad,
                Categoria = e.Categoria!.Nombre,
                InscritosConfirmados = e.Inscripciones.Count(i => i.Estado == "Confirmada")
            })
            .ToListAsync();
    }

    public Task<Evento?> ObtenerPorIdAsync(int id) =>
        context.Eventos.AsNoTracking()
            .Include(e => e.Categoria)
            .Include(e => e.Inscripciones.Where(i => i.Estado == "Confirmada"))
                .ThenInclude(i => i.Asistente)
            .FirstOrDefaultAsync(e => e.Id == id && e.Activo);

    public async Task CrearAsync(Evento evento)
    {
        context.Eventos.Add(evento);
        await context.SaveChangesAsync();
    }

    public async Task<bool> EditarAsync(Evento evento)
    {
        var existente = await context.Eventos.FirstOrDefaultAsync(e => e.Id == evento.Id && e.Activo);
        if (existente is null)
        {
            return false;
        }

        existente.Titulo = evento.Titulo;
        existente.Fecha = evento.Fecha;
        existente.Lugar = evento.Lugar;
        existente.Aforo = evento.Aforo;
        existente.Precio = evento.Precio;
        existente.Modalidad = evento.Modalidad;
        existente.CategoriaId = evento.CategoriaId;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var evento = await context.Eventos.FirstOrDefaultAsync(e => e.Id == id && e.Activo);
        if (evento is null)
        {
            return false;
        }

        evento.Activo = false;
        await context.SaveChangesAsync();
        return true;
    }

    public Task<List<Categoria>> ObtenerCategoriasActivasAsync() =>
        context.Categorias.AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();

    public async Task<bool> DesactivarCategoriaAsync(int id)
    {
        var categoria = await context.Categorias.FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        if (categoria is null)
        {
            return false;
        }

        categoria.Activo = false;
        await context.SaveChangesAsync();
        return true;
    }
}
