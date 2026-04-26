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
}