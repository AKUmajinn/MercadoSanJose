using MercadoSanJose.Web.Data;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Models.DTO;
using MercadoSanJose.Web.Models.Enums;
using MercadoSanJose.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MercadoSanJose.Web.Repositories.DAO;

public class PuestoDAO : IPuesto
{
    private readonly ApplicationDbContext _context;

    public PuestoDAO(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Puesto> getAll()
    {
        return _context.Puestos
            .Include(p => p.Propietario)
            .Include(p => p.Inquilino)
            .ToList();
    }

    public Puesto getById(int id)
    {
        return _context.Puestos
            .Include(p => p.Propietario)
            .Include(p => p.Inquilino)
            .FirstOrDefault(p => p.Id == id);
    }

    public int crearPuesto(PuestoDTO puesto)
    {
        var nuevoPuesto = new Puesto
        {
            NumeroPuesto = puesto.NumeroPuesto,
            Sector = puesto.Sector,
            PropietarioId = puesto.PropietarioId == 0 ? null : puesto.PropietarioId,
            InquilinoId = puesto.InquilinoId == 0 ? null : puesto.InquilinoId,
            Estado = EstadoPuesto.Disponible
        };

        _context.Puestos.Add(nuevoPuesto);
        _context.SaveChanges();
        return nuevoPuesto.Id;
    }

    public int updatePuesto(PuestoDTO puesto)
    {
        var puestoExistente = _context.Puestos.Find(puesto.Id);
        if (puestoExistente == null) return 0;

        if (Enum.TryParse<EstadoPuesto>(puesto.Estado, out var estadoParseado))
        {
            puestoExistente.Estado = estadoParseado;
        }

        puestoExistente.PropietarioId = puesto.PropietarioId == 0 ? null : puesto.PropietarioId;
        puestoExistente.InquilinoId = puesto.InquilinoId == 0 ? null : puesto.InquilinoId;

        _context.Puestos.Update(puestoExistente);
        return _context.SaveChanges();
    }

    public int update(Puesto entidad)
    {
        _context.Puestos.Update(entidad);
        return _context.SaveChanges();
    }

    public int delete(int id)
    {
        var puesto = _context.Puestos.Find(id);
        if (puesto == null) return 0;

        _context.Puestos.Remove(puesto);
        return _context.SaveChanges();
    }
}