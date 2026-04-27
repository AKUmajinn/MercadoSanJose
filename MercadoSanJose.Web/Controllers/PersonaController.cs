using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Models.DTO;
using MercadoSanJose.Web.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MercadoSanJose.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonaController : ControllerBase
{
    private readonly IPersona _personaDao;

    public PersonaController(IPersona personaDao)
    {
        _personaDao = personaDao;
    }

    [HttpPost("crearPersona")]
    public IActionResult CrearPersona([FromBody] PersonaDTO persona)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        int idCreado = _personaDao.CrearPersona(persona);

        if (idCreado <= 0)
            return BadRequest("No se pudo registrar la persona.");

        return Created("", new
        {
            id = idCreado,
            dni = persona.Dni,
            nombre = persona.Nombre,
            telefono = persona.Telefono,
            esGerencia = persona.EsGerencia
        });
    }

    [HttpGet("listarPersona")]
    public IActionResult ListarPersona()
    {
        var personas = _personaDao.ListarPersona();

        return Ok(personas);
    }

    [HttpGet("obtenerPersona/{id}")]
    public IActionResult ObtenerPersona(int id)
    {
        var persona = _personaDao.getById(id);

        if (persona == null)
            return NotFound("Persona no encontrada");

        return Ok(persona);
    }

    [HttpPut("actualizarPersona/{id}")]
    public IActionResult ActualizarPersona(int id, [FromBody] Persona persona)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        persona.Id = id;

        int resultado = _personaDao.update(persona);

        if (resultado == 0)
            return NotFound("Persona no encontrada o inactiva.");

        return Ok(persona);
    }
}