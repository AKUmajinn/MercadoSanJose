using System.Data;
using Microsoft.Data.SqlClient;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.Interfaces;

namespace MercadoSanJose.Web.Repositories.DAO;

public class DeudaDAO : IDeuda
{
    private readonly string _connectionString;

    public DeudaDAO()
    {
        _connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json")
            .Build().GetConnectionString("dataBase");
    }

    public IEnumerable<Deuda> getAll() => throw new NotImplementedException();
    public Deuda getById(int id) => throw new NotImplementedException();
    public int add(Deuda entidad) => throw new NotImplementedException();
    public int update(Deuda entidad) => throw new NotImplementedException();
    public int delete(int id) => throw new NotImplementedException();
}