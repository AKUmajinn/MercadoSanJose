using MercadoSanJose.Web.Models;

namespace MercadoSanJose.Web.Repositories.Interfaces;

public interface IPuesto
{
    IEnumerable<Puesto> getAll();
    Puesto getById(int id);
    int add(Puesto entidad);
    int update(Puesto entidad);
    int delete(int id);
}