using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class InscripcionesController : Controller
{
    private readonly InscripcionRepositorio _repositorio;

    public InscripcionesController(InscripcionRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IActionResult> Index()
    {
        var inscripciones = await _repositorio.ListarAsync();
        return View(inscripciones);
    }

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new Inscripcion());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EventoId,AsistenteId")] Inscripcion inscripcion)
    {
        if (ModelState.IsValid)
        {
            var evento = await _repositorio.ObtenerEventoActivoAsync(inscripcion.EventoId);

            if (evento == null)
            {
                ModelState.AddModelError(nameof(Inscripcion.EventoId), "El evento seleccionado no existe o está inactivo.");
            }
            else
            {
                if (evento.Fecha < DateTime.Now)
                {
                    ModelState.AddModelError(nameof(Inscripcion.EventoId), "El evento ya pasó, no se puede inscribir.");
                }

                if (await _repositorio.ContarConfirmadasAsync(evento.Id) >= evento.Aforo)
                {
                    ModelState.AddModelError(nameof(Inscripcion.EventoId), "El aforo del evento está completo.");
                }
            }

            if (!await _repositorio.ExisteAsistenteActivoAsync(inscripcion.AsistenteId))
            {
                ModelState.AddModelError(nameof(Inscripcion.AsistenteId), "El asistente seleccionado no existe o está inactivo.");
            }
            else if (evento != null &&
                     await _repositorio.ExisteInscripcionConfirmadaAsync(inscripcion.EventoId, inscripcion.AsistenteId))
            {
                ModelState.AddModelError(nameof(Inscripcion.AsistenteId), "El asistente ya está inscrito en este evento.");
            }
        }

        if (ModelState.IsValid)
        {
            inscripcion.FechaInscripcion = DateTime.Now;
            inscripcion.Estado = EstadosInscripcion.Confirmada;

            await _repositorio.CrearAsync(inscripcion);
            TempData["Mensaje"] = "Inscripción registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        await CargarListasAsync();
        return View(inscripcion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        var cancelada = await _repositorio.CancelarAsync(id);

        if (cancelada)
            TempData["Mensaje"] = "Inscripción cancelada correctamente.";
        else
            TempData["Error"] = "No se pudo cancelar: la inscripción no existe o ya estaba cancelada.";

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync()
    {
        var eventos = await _repositorio.ObtenerEventosActivosAsync();
        var asistentes = await _repositorio.ObtenerAsistentesActivosAsync();

        ViewBag.Eventos = eventos
            .Select(e => new SelectListItem($"{e.Titulo} — {e.Fecha:dd/MM/yyyy HH:mm}", e.Id.ToString()))
            .ToList();

        ViewBag.Asistentes = asistentes
            .Select(a => new SelectListItem($"{a.Nombres} (DNI {a.Dni})", a.Id.ToString()))
            .ToList();
    }
}
