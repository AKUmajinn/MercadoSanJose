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
}