using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class AsistentesController(AsistenteRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index() => View(await repositorio.ObtenerActivosAsync());

    public IActionResult Create() => View(new Asistente());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Asistente asistente)
    {
        if (!string.IsNullOrWhiteSpace(asistente.Dni) && await repositorio.ExisteDniAsync(asistente.Dni))
        {
            ModelState.AddModelError(nameof(asistente.Dni), "Ya existe un asistente registrado con este DNI.");
        }

        if (!string.IsNullOrWhiteSpace(asistente.Correo) && await repositorio.ExisteCorreoAsync(asistente.Correo))
        {
            ModelState.AddModelError(nameof(asistente.Correo), "Ya existe un asistente registrado con este correo.");
        }

        if (!ModelState.IsValid)
        {
            return View(asistente);
        }

        await repositorio.CrearAsync(asistente);
        TempData["Mensaje"] = "El asistente se creó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await repositorio.DesactivarAsync(id))
        {
            return NotFound();
        }

        TempData["Mensaje"] = "El asistente se desactivó correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
