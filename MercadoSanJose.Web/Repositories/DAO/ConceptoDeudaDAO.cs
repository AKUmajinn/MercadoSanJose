using System.Data;
using Microsoft.Data.SqlClient;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.Interfaces;

namespace MercadoSanJose.Web.Repositories.DAO;

public class ConceptoDeudaDAO : IConceptoDeuda
{
    private readonly string _connectionString;

    public ConceptoDeudaDAO()
    {
        _connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json")
            .Build().GetConnectionString("dataBase");
    }

    public IEnumerable<ConceptoDeuda> getAll() => throw new NotImplementedException();
    public ConceptoDeuda getById(int id) => throw new NotImplementedException();
    public int add(ConceptoDeuda entidad) => throw new NotImplementedException();
    public int update(ConceptoDeuda entidad) => throw new NotImplementedException();
    public int delete(int id) => throw new NotImplementedException();
}