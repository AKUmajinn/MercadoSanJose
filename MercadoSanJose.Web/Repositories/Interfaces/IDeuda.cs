using MercadoSanJose.Web.Models;

namespace MercadoSanJose.Web.Repositories.Interfaces;

public interface IDeuda
{
    IEnumerable<Deuda> getAll();
    Deuda getById(int id);
    int add(Deuda entidad);
    int update(Deuda entidad);
    int delete(int id);
}