using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventos.Web.Models;

public static class EstadosInscripcion
{
    public const string Confirmada = "Confirmada";
    public const string Cancelada = "Cancelada";
}

public class Inscripcion
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Evento")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un evento.")]
    [ForeignKey(nameof(Evento))]
    public int EventoId { get; set; }

    public Evento? Evento { get; set; }

    [Display(Name = "Asistente")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un asistente.")]
    [ForeignKey(nameof(Asistente))]
    public int AsistenteId { get; set; }

    public Asistente? Asistente { get; set; }

    [Display(Name = "Fecha de inscripción")]
    public DateTime FechaInscripcion { get; set; } = DateTime.Now;

    [Required]
    [StringLength(20)]
    public string Estado { get; set; } = EstadosInscripcion.Confirmada;
}
