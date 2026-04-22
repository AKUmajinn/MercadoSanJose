using MercadoSanJose.Web.Models;

namespace MercadoSanJose.Web.Repositories.Interfaces;

public interface IPersona
{
    IEnumerable<Persona> getAll();
    Persona getById(int id);
    int add(Persona entidad);
    int update(Persona entidad);
    int delete(int id);
}