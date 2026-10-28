using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class EventoRepositorio
{
    private readonly EventosContext _context;

    public EventoRepositorio(EventosContext context)
    {
        _context = context;
    }

    // ---------- Listas desplegables ----------

    public async Task<List<Categoria>> ObtenerCategoriasActivasAsync()
    {
        return await _context.Categorias
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<bool> ExisteCategoriaActivaAsync(int categoriaId)
    {
        return await _context.Categorias.AnyAsync(c => c.Id == categoriaId && c.Activo);
    }

    // ---------- Lecturas ----------

    public async Task<List<EventoItem>> ListarActivosAsync(int? categoriaId)
    {
        var consulta = _context.Eventos
            .AsNoTracking()
            .Where(e => e.Activo);

        if (categoriaId.HasValue)
        {
            consulta = consulta.Where(e => e.CategoriaId == categoriaId.Value);
        }

        return await Proyectar(consulta.OrderBy(e => e.Fecha)).ToListAsync();
    }

    public async Task<EventoItem?> ObtenerItemAsync(int id)
    {
        return await Proyectar(_context.Eventos.AsNoTracking().Where(e => e.Id == id && e.Activo))
            .FirstOrDefaultAsync();
    }

    public async Task<Evento?> ObtenerPorIdAsync(int id)
    {
        return await _context.Eventos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.Activo);
    }

    private static IQueryable<EventoItem> Proyectar(IQueryable<Evento> consulta)
    {
        return consulta.Select(e => new EventoItem
        {
            Id = e.Id,
            Titulo = e.Titulo,
            Fecha = e.Fecha,
            Lugar = e.Lugar,
            Modalidad = e.Modalidad,
            Precio = e.Precio,
            Aforo = e.Aforo,
            Categoria = e.Categoria!.Nombre,
            Confirmados = e.Inscripciones.Count(i => i.Estado == EstadosInscripcion.Confirmada)
        });
    }

    // ---------- Escrituras ----------

    public async Task CrearAsync(Evento evento)
    {
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ActualizarAsync(Evento datos)
    {
        var existente = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == datos.Id && e.Activo);
        if (existente == null)
        {
            return false;
        }

        existente.Titulo = datos.Titulo;
        existente.Fecha = datos.Fecha;
        existente.Lugar = datos.Lugar;
        existente.Aforo = datos.Aforo;
        existente.Precio = datos.Precio;
        existente.CategoriaId = datos.CategoriaId;
        existente.Modalidad = datos.Modalidad;

        await _context.SaveChangesAsync();
        return true;
    }

    // Eliminación lógica: nunca se hace DELETE físico.
    public async Task<bool> EliminarLogicoAsync(int id)
    {
        var existente = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == id && e.Activo);
        if (existente == null)
        {
            return false;
        }

        existente.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
