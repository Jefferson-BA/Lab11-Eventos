using Eventos.Web.Repositorios;
using Eventos.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class InscripcionesController(InscripcionRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index() => View(await repositorio.ObtenerTodasAsync());

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new InscripcionCrearViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InscripcionCrearViewModel inscripcion)
    {
        if (ModelState.IsValid)
        {
            if (await repositorio.EventoYaPasoAsync(inscripcion.EventoId))
            {
                ModelState.AddModelError(nameof(inscripcion.EventoId), "No es posible inscribirse a un evento que ya pasó o no está disponible.");
            }

            if (await repositorio.YaInscritoAsync(inscripcion.EventoId, inscripcion.AsistenteId))
            {
                ModelState.AddModelError(nameof(inscripcion.AsistenteId), "El asistente ya tiene una inscripción confirmada para este evento.");
            }

            if (await repositorio.AforoCompletoAsync(inscripcion.EventoId))
            {
                ModelState.AddModelError(nameof(inscripcion.EventoId), "El evento alcanzó su aforo máximo.");
            }
        }

        if (!ModelState.IsValid)
        {
            await CargarListasAsync();
            return View(inscripcion);
        }

        await repositorio.CrearAsync(inscripcion.EventoId, inscripcion.AsistenteId);
        TempData["Mensaje"] = "La inscripción se confirmó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        if (!await repositorio.CancelarAsync(id))
        {
            return NotFound();
        }

        TempData["Mensaje"] = "La inscripción se canceló correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync()
    {
        ViewBag.Eventos = new SelectList(await repositorio.ObtenerEventosActivosAsync(), "Id", "Titulo");
        ViewBag.Asistentes = new SelectList(await repositorio.ObtenerAsistentesActivosAsync(), "Id", "Nombres");
    }
}
