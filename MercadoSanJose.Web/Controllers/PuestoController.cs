using MercadoSanJose.Web.Models.DTO;
using MercadoSanJose.Web.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MercadoSanJose.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PuestoController : ControllerBase
{
    private readonly IPuesto _dao;

    public PuestoController(IPuesto dao)
    {
        _dao = dao;
    }

    [HttpPost("crearPuesto")]
    public ActionResult crearPuesto([FromBody] PuestoDTO puesto)
    {
        int idGenerado = _dao.crearPuesto(puesto);
        puesto.Id = idGenerado;
        return Created("", puesto);
    }

    [HttpPut("actualizarPuesto/{id}")]
    public IActionResult ActualizarPuesto(int id, [FromBody] PuestoDTO puesto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        puesto.Id = id;

        int resultado = _dao.updatePuesto(puesto);

        if (resultado == 0)
            return NotFound("Puesto no encontrado.");

        return Ok(new
        {
            mensaje = "Puesto actualizado correctamente",
            estado = puesto.Estado,
            propietarioId = puesto.PropietarioId,
            inquilinoId = puesto.InquilinoId
        });
    }
}