using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventos.Web.Models;

public class Evento
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    [Display(Name = "Fecha y hora")]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "El lugar es obligatorio.")]
    [StringLength(100)]
    public string Lugar { get; set; } = string.Empty;

    [Range(1, 5000, ErrorMessage = "El aforo debe estar entre 1 y 5000.")]
    public int Aforo { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(0, 99999999.99, ErrorMessage = "El precio no puede ser negativo.")]
    [Display(Name = "Precio (S/)")]
    public decimal Precio { get; set; }

    [Display(Name = "Categoría")]
    [ForeignKey(nameof(Categoria))]
    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

    public bool Activo { get; set; } = true;

    [Required(ErrorMessage = "La modalidad es obligatoria.")]
    [StringLength(20)]
    [RegularExpression("^(Presencial|Virtual)$", ErrorMessage = "La modalidad debe ser Presencial o Virtual.")]
    public string Modalidad { get; set; } = "Presencial";

    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
