namespace Eventos.Web.ViewModels;

public class EventoItem
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public string Modalidad { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Aforo { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public int Confirmados { get; set; }

    public int CuposDisponibles => Aforo - Confirmados;
}
