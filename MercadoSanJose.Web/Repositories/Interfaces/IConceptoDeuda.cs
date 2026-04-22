using MercadoSanJose.Web.Models;

namespace MercadoSanJose.Web.Repositories.Interfaces;

public interface IConceptoDeuda
{
    IEnumerable<ConceptoDeuda> getAll();
    ConceptoDeuda getById(int id);
    int add(ConceptoDeuda entidad);
    int update(ConceptoDeuda entidad);
    int delete(int id);
}