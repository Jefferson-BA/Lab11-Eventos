namespace Eventos.Web.ViewModels;

public class EventoListaViewModel
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public int Aforo { get; set; }
    public decimal Precio { get; set; }
    public string Modalidad { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int InscritosConfirmados { get; set; }
    public int CuposDisponibles => Aforo - InscritosConfirmados;
}
