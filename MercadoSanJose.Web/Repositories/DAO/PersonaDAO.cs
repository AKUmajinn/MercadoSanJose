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
    public IEnumerable<Persona> ListarPersona()
    {
        List<Persona> personas = new List<Persona>();

        using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
        {
            sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand("usp_Listar_Personas", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                personas.Add(new Persona()
                {
                    Id = reader.GetInt32(0),
                    DNI = reader.GetString(1),
                    Nombre = reader.GetString(2),
                    Telefono = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    EsGerencia = reader.GetBoolean(4),
                    Activo = reader.GetBoolean(5)
                });
            }
        }

        return personas;
    }
    public Persona getById(int id)
    {
        Persona persona = null;

        using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
        {
            sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand("usp_Obtener_Persona_Por_Id", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@Id", id);

            SqlDataReader reader = sqlCommand.ExecuteReader();

            if (reader.Read())
            {
                persona = new Persona()
                {
                    Id = reader.GetInt32(0),
                    DNI = reader.GetString(1),
                    Nombre = reader.GetString(2),
                    Telefono = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    EsGerencia = reader.GetBoolean(4),
                    Activo = reader.GetBoolean(5)
                };
            }
        }

        return persona;
    }
    public int update(Persona entidad)
    {
        using SqlConnection sqlConnection = new SqlConnection(_connectionString);
        sqlConnection.Open();

        using SqlCommand sqlCommand = new SqlCommand("usp_Actualizar_Persona", sqlConnection);
        sqlCommand.CommandType = CommandType.StoredProcedure;

        sqlCommand.Parameters.AddWithValue("@Id", entidad.Id);
        sqlCommand.Parameters.AddWithValue("@DNI", entidad.DNI);
        sqlCommand.Parameters.AddWithValue("@Nombre", entidad.Nombre);
        sqlCommand.Parameters.AddWithValue("@Telefono", entidad.Telefono);
        sqlCommand.Parameters.AddWithValue("@EsGerencia", entidad.EsGerencia);

        object resultado = sqlCommand.ExecuteScalar();

        return Convert.ToInt32(resultado);
    }
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