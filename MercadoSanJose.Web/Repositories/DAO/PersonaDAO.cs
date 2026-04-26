using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Models.DTO;
using MercadoSanJose.Web.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

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
    public int update(Persona entidad) => throw new NotImplementedException();
    public int delete(int id)
    {
        using SqlConnection sqlConnection = new(_connectionString);
        sqlConnection.Open();

        using SqlCommand sqlCommand = new("usp_Eliminar_Persona", sqlConnection);
        sqlCommand.CommandType = CommandType.StoredProcedure;

        sqlCommand.Parameters.AddWithValue("@Id", id);

        object result = sqlCommand.ExecuteScalar();

        return Convert.ToInt32(result);
    }
    public int CrearPersona(PersonaDTO persona)
    {
        using SqlConnection sqlConnection = new(_connectionString);
        sqlConnection.Open();

        using SqlCommand sqlCommand = new("usp_Guardar_Persona", sqlConnection);
        sqlCommand.CommandType = CommandType.StoredProcedure;

        sqlCommand.Parameters.AddWithValue("@DNI", persona.Dni);
        sqlCommand.Parameters.AddWithValue("@Nombre", persona.Nombre);
        sqlCommand.Parameters.AddWithValue("@Telefono", persona.Telefono);
        sqlCommand.Parameters.AddWithValue("@EsGerencia", persona.EsGerencia);

        object resultado = sqlCommand.ExecuteScalar();

        return Convert.ToInt32(resultado);
    }
}