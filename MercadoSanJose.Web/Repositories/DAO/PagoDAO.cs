using System.Data;
using Microsoft.Data.SqlClient;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.Interfaces;

namespace MercadoSanJose.Web.Repositories.DAO;

public class PagoDAO : IPago
{
    private readonly string _connectionString;

    public PagoDAO()
    {
        _connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json")
            .Build().GetConnectionString("dataBase");
    }

    public IEnumerable<Pago> getAll() => throw new NotImplementedException();
    public Pago getById(int id) => throw new NotImplementedException();
    public int add(Pago entidad) => throw new NotImplementedException();
    public int delete(int id) => throw new NotImplementedException();
}