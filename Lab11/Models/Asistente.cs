using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Models;

[Index(nameof(Dni), IsUnique = true)]
[Index(nameof(Correo), IsUnique = true)]
public class Asistente
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required, StringLength(8, MinimumLength = 8)]
    [RegularExpression("^\\d{8}$", ErrorMessage = "El DNI debe contener exactamente 8 dígitos.")]
    public string Dni { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Correo { get; set; } = string.Empty;

    [StringLength(9)]
    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
