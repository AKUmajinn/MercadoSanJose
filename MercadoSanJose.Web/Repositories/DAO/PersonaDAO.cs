using MercadoSanJose.Web.Data;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Models.DTO;
using MercadoSanJose.Web.Repositories.Interfaces;

namespace MercadoSanJose.Web.Repositories.DAO;

public class PersonaDAO : IPersona
{
    private readonly ApplicationDbContext _context;

    public PersonaDAO(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Persona> ListarPersona()
    {
        return _context.Personas.ToList();
    }

    public Persona getById(int id)
    {
        return _context.Personas.Find(id);
    }

    public int CrearPersona(PersonaDTO persona)
    {
        var nuevaPersona = new Persona
        {
            DNI = persona.Dni,
            Nombre = persona.Nombre,
            Telefono = persona.Telefono,
            EsGerencia = persona.EsGerencia,
            Activo = true
        };

        _context.Personas.Add(nuevaPersona);
        _context.SaveChanges();
        return nuevaPersona.Id;
    }

    public int update(Persona entidad)
    {
        var personaExistente = _context.Personas.Find(entidad.Id);
        if (personaExistente == null) return 0;

        personaExistente.DNI = entidad.DNI;
        personaExistente.Nombre = entidad.Nombre;
        personaExistente.Telefono = entidad.Telefono;
        personaExistente.EsGerencia = entidad.EsGerencia;

        _context.Personas.Update(personaExistente);
        return _context.SaveChanges();
    }

    public int delete(int id)
    {
        var persona = _context.Personas.Find(id);
        if (persona == null) return 0;

        _context.Personas.Remove(persona);
        return _context.SaveChanges();
    }
}