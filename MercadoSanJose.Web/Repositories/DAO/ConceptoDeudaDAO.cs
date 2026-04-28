using MercadoSanJose.Web.Data;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.Interfaces;

namespace MercadoSanJose.Web.Repositories.DAO;

public class ConceptoDeudaDAO : IConceptoDeuda
{
    private readonly ApplicationDbContext _context;

    public ConceptoDeudaDAO(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<ConceptoDeuda> getAll()
    {
        return _context.ConceptosDeuda.ToList();
    }

    public ConceptoDeuda getById(int id)
    {
        return _context.ConceptosDeuda.Find(id);
    }

    public int add(ConceptoDeuda entidad)
    {
        _context.ConceptosDeuda.Add(entidad);
        return _context.SaveChanges();
    }

    public int update(ConceptoDeuda entidad)
    {
        _context.ConceptosDeuda.Update(entidad);
        return _context.SaveChanges();
    }

    public int delete(int id)
    {
        var concepto = _context.ConceptosDeuda.Find(id);
        if (concepto == null) return 0;

        _context.ConceptosDeuda.Remove(concepto);
        return _context.SaveChanges();
    }
}