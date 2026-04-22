using MercadoSanJose.Web.Models;

namespace MercadoSanJose.Web.Repositories.Interfaces;

public interface IPago
{
    IEnumerable<Pago> getAll();
    Pago getById(int id);
    int add(Pago entidad);
    int delete(int id);
}