using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class EventosController(EventoRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index(int? categoriaId)
    {
        ViewBag.Categorias = new SelectList(await repositorio.ObtenerCategoriasActivasAsync(), "Id", "Nombre", categoriaId);
        ViewBag.CategoriaId = categoriaId;
        return View(await repositorio.ObtenerActivosAsync(categoriaId));
    }

    public async Task<IActionResult> Details(int id)
    {
        var evento = await repositorio.ObtenerPorIdAsync(id);
        return evento is null ? NotFound() : View(evento);
    }

    public async Task<IActionResult> Create()
    {
        await CargarCategoriasAsync();
        return View(new Evento { Fecha = DateTime.Today.AddDays(1), Modalidad = "Presencial" });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var evento = await repositorio.ObtenerPorIdAsync(id);
        return evento is null ? NotFound() : View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Evento evento)
    {
        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync(evento.CategoriaId);
            return View(evento);
        }

        await repositorio.CrearAsync(evento);
        TempData["Mensaje"] = "El evento se creó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var evento = await repositorio.ObtenerPorIdAsync(id);
        if (evento is null)
        {
            return NotFound();
        }

        evento.Inscripciones = new List<Inscripcion>();
        await CargarCategoriasAsync(evento.CategoriaId);
        return View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Evento evento)
    {
        if (id != evento.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync(evento.CategoriaId);
            return View(evento);
        }

        if (!await repositorio.EditarAsync(evento))
        {
            return NotFound();
        }

        TempData["Mensaje"] = "El evento se actualizó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName(nameof(Delete))]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await repositorio.EliminarAsync(id))
        {
            return NotFound();
        }

        TempData["Mensaje"] = "El evento se desactivó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarCategoriasAsync(int? categoriaId = null)
    {
        ViewBag.CategoriaId = new SelectList(
            await repositorio.ObtenerCategoriasActivasAsync(),
            "Id",
            "Nombre",
            categoriaId);
    }
}
