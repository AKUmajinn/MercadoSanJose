using MercadoSanJose.Web.Data; // Asegúrate de tener esta referencia
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore; // Necesario para consultas

namespace MercadoSanJose.Web.Repositories.DAO;

public class DeudaDAO : IDeuda
{
    private readonly ApplicationDbContext _context;

    // Inyección de dependencias: ASP.NET Core se encarga de crear el contexto
    public DeudaDAO(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Deuda> getAll()
    {
        return _context.Deudas.ToList();
    }

    public Deuda getById(int id)
    {
        return _context.Deudas.Find(id);
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