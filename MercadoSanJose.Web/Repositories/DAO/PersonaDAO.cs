using System.Data;
using Microsoft.Data.SqlClient;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.Interfaces;

namespace MercadoSanJose.Web.Repositories.DAO;

public class PersonaDAO : IPersona
{
    private readonly string _connectionString;

    public PersonaDAO()
    {
        _connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json")
            .Build().GetConnectionString("dataBase");
    }

    public IEnumerable<Persona> getAll() => throw new NotImplementedException();
    public Persona getById(int id) => throw new NotImplementedException();
    public int add(Persona entidad) => throw new NotImplementedException();
    public int update(Persona entidad) => throw new NotImplementedException();
    public int delete(int id) => throw new NotImplementedException();
}