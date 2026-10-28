using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class EventosController : Controller
{
    private readonly EventoRepositorio _repositorio;

    public EventosController(EventoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IActionResult> Index(int? categoriaId)
    {
        var categorias = await _repositorio.ObtenerCategoriasActivasAsync();
        ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre", categoriaId);

        var eventos = await _repositorio.ListarActivosAsync(categoriaId);
        return View(eventos);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var evento = await _repositorio.ObtenerItemAsync(id.Value);
        if (evento == null) return NotFound();

        return View(evento);
    }

    // ---------- Create ----------

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new Evento { Fecha = DateTime.Today.AddDays(7).AddHours(18) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Titulo,Fecha,Lugar,Aforo,Precio,CategoriaId,Modalidad")] Evento evento)
    {
        if (!await _repositorio.ExisteCategoriaActivaAsync(evento.CategoriaId))
        {
            ModelState.AddModelError(nameof(Evento.CategoriaId), "Seleccione una categoría activa.");
        }

        if (ModelState.IsValid)
        {
            await _repositorio.CrearAsync(evento);
            TempData["Mensaje"] = "Evento creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        await CargarListasAsync(evento.CategoriaId, evento.Modalidad);
        return View(evento);
    }

    // ---------- Edit ----------

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var evento = await _repositorio.ObtenerPorIdAsync(id.Value);
        if (evento == null) return NotFound();

        await CargarListasAsync(evento.CategoriaId, evento.Modalidad);
        return View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,Titulo,Fecha,Lugar,Aforo,Precio,CategoriaId,Modalidad")] Evento evento)
    {
        if (id != evento.Id) return NotFound();

        if (!await _repositorio.ExisteCategoriaActivaAsync(evento.CategoriaId))
        {
            ModelState.AddModelError(nameof(Evento.CategoriaId), "Seleccione una categoría activa.");
        }

        if (ModelState.IsValid)
        {
            var actualizado = await _repositorio.ActualizarAsync(evento);
            if (!actualizado) return NotFound();

            TempData["Mensaje"] = "Evento actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        await CargarListasAsync(evento.CategoriaId, evento.Modalidad);
        return View(evento);
    }

    // ---------- Delete (lógico) ----------

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var evento = await _repositorio.ObtenerItemAsync(id.Value);
        if (evento == null) return NotFound();

        return View(evento);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var eliminado = await _repositorio.EliminarLogicoAsync(id);

        if (eliminado)
            TempData["Mensaje"] = "Evento eliminado correctamente.";
        else
            TempData["Error"] = "No se encontró el evento.";

        return RedirectToAction(nameof(Index));
    }

    // ---------- Auxiliares ----------

    private async Task CargarListasAsync(int? categoriaId = null, string? modalidad = null)
    {
        var categorias = await _repositorio.ObtenerCategoriasActivasAsync();
        ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre", categoriaId);
        ViewBag.Modalidades = new SelectList(new[] { "Presencial", "Virtual" }, modalidad);
    }
}
