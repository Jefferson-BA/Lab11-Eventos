using System.ComponentModel.DataAnnotations;

namespace Eventos.Web.Models;

public class Inscripcion
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int EventoId { get; set; }

    public Evento? Evento { get; set; }

    [Required]
    public int AsistenteId { get; set; }

    public Asistente? Asistente { get; set; }

    public DateTime FechaInscripcion { get; set; } = DateTime.Now;

    [Required]
    [RegularExpression("^(Confirmada|Cancelada)$")]
    public string Estado { get; set; } = "Confirmada";
}
