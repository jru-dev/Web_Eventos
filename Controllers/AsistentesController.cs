using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class AsistentesController : Controller
{
    private readonly AsistenteRepositorio _repositorio;

    public AsistentesController(AsistenteRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IActionResult> Index()
    {
        var asistentes = await _repositorio.ListarActivosAsync();
        return View(asistentes);
    }

    public IActionResult Create()
    {
        return View(new Asistente());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Nombres,Dni,Correo,Telefono")] Asistente asistente)
    {
        asistente.Nombres = asistente.Nombres?.Trim() ?? string.Empty;
        asistente.Correo = asistente.Correo?.Trim() ?? string.Empty;

        if (ModelState.IsValid)
        {
            if (await _repositorio.ExisteDniAsync(asistente.Dni))
            {
                ModelState.AddModelError(nameof(Asistente.Dni), "Ya existe un asistente con ese DNI.");
            }

            if (await _repositorio.ExisteCorreoAsync(asistente.Correo))
            {
                ModelState.AddModelError(nameof(Asistente.Correo), "Ya existe un asistente con ese correo.");
            }
        }

        if (ModelState.IsValid)
        {
            await _repositorio.CrearAsync(asistente);
            TempData["Mensaje"] = "Asistente registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        return View(asistente);
    }
}
