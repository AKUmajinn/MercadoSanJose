using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Models.DTO;

namespace MercadoSanJose.Web.Repositories.Interfaces;

public interface IPersona
{
    IEnumerable<Persona> getAll();
    Persona getById(int id);
    int CrearPersona(PersonaDTO persona);
    int update(Persona entidad);
    int delete(int id);
}