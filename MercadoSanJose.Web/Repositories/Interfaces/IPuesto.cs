using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Models.DTO;

namespace MercadoSanJose.Web.Repositories.Interfaces;

public interface IPuesto
{
    IEnumerable<Puesto> getAll();
    Puesto getById(int id);
    int crearPuesto(PuestoDTO puesto);
    int update(Puesto entidad);
    int delete(int id);

    int updatePuesto(PuestoDTO puesto);
}