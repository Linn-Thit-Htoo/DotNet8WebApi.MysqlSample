using MySqlConnector;
using Newtonsoft.Json;
using System.Data;

namespace DotNet8WebApi.MysqlSample.Services
{
    public class AdoDotNetService
    {
        private readonly IConfiguration _configuration;

        public AdoDotNetService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<T>> QueryAsync<T>(string query, MySqlParameter[]? parameters = null)
        {
            try
            {
                var connection = GetMySqlConnection();
                await connection.OpenAsync();

                MySqlCommand command = new(query, connection);
                if (parameters is not null)
                {
                    command.Parameters.AddRange(parameters);
                }

                DataTable dataTable = new();
                MySqlDataAdapter adapter = new(command);
                adapter.Fill(dataTable);

                await connection.CloseAsync();

                string jsonStr = JsonConvert.SerializeObject(dataTable);
                return JsonConvert.DeserializeObject<List<T>>(jsonStr)!;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private MySqlConnection GetMySqlConnection() => new(_configuration.GetConnectionString("DbConnection"));
    }
}
