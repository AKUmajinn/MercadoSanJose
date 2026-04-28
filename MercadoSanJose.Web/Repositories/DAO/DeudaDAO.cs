using MercadoSanJose.Web.Data;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MercadoSanJose.Web.Repositories.DAO;

public class DeudaDAO : IDeuda
{
    private readonly ApplicationDbContext _context;

    public DeudaDAO(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Deuda> getAll()
    {
        return _context.Deudas
            .Include(d => d.Puesto)
            .Include(d => d.ConceptoDeuda)
            .Include(d => d.Responsable)
            .ToList();
    }

    public Deuda getById(int id)
    {
        return _context.Deudas
            .Include(d => d.Puesto)
            .Include(d => d.ConceptoDeuda)
            .Include(d => d.Responsable)
            .FirstOrDefault(d => d.Id == id)!;
    }

    public int add(Deuda entidad)
    {
        _context.Deudas.Add(entidad);
        return _context.SaveChanges();
    }

    public int update(Deuda entidad)
    {
        _context.Deudas.Update(entidad);
        return _context.SaveChanges();
    }

    public int delete(int id)
    {
        var deuda = _context.Deudas.Find(id);
        if (deuda != null)
        {
            _context.Deudas.Remove(deuda);
            return _context.SaveChanges();
        }
        return 0;
    }
}