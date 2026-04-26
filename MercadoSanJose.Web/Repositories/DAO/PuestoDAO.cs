using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Models.DTO;
using MercadoSanJose.Web.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

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
    public int update(Puesto entidad) => throw new NotImplementedException();
    public int delete(int id) => throw new NotImplementedException();
    public int crearPuesto(PuestoDTO puesto)
    {
        using SqlConnection sqlConnection = new(_connectionString);
        sqlConnection.Open();

        SqlCommand sqlCommand = new SqlCommand("usp_crear_puesto", sqlConnection);
        sqlCommand.CommandType = CommandType.StoredProcedure;

        sqlCommand.Parameters.AddWithValue("@NumeroPuesto", puesto.NumeroPuesto);
        sqlCommand.Parameters.AddWithValue("@Sector", puesto.Sector);
        sqlCommand.Parameters.AddWithValue("@PropietarioId", (object?)puesto.PropietarioId ?? DBNull.Value);
        sqlCommand.Parameters.AddWithValue("@InquilinoId", (object?)puesto.InquilinoId ?? DBNull.Value);

        return Convert.ToInt32(sqlCommand.ExecuteScalar());
    }
}