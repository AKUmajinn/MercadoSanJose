using System.Data;
using Microsoft.Data.SqlClient;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.Interfaces;

namespace MercadoSanJose.Web.Repositories.DAO;

public class PuestoDAO : IPuesto
{
    private readonly string _connectionString;

    public PuestoDAO()
    {
        _connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json")
            .Build().GetConnectionString("dataBase");
    }

    public IEnumerable<Puesto> getAll() => throw new NotImplementedException();
    public Puesto getById(int id) => throw new NotImplementedException();
    public int add(Puesto entidad) => throw new NotImplementedException();
    public int update(Puesto entidad) => throw new NotImplementedException();
    public int delete(int id) => throw new NotImplementedException();
}