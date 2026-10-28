using System.ComponentModel.DataAnnotations;

namespace Eventos.Web.ViewModels;

public class InscripcionCrearViewModel
{
    [Display(Name = "Evento")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un evento.")]
    public int EventoId { get; set; }

    [Display(Name = "Asistente")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un asistente.")]
    public int AsistenteId { get; set; }
}
