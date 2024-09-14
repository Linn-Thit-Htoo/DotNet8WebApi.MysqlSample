using MySqlConnector;

namespace DotNet8WebApi.MysqlSample.Services
{
    public class AdoDotNetService
    {
        private readonly IConfiguration _configuration;

        public AdoDotNetService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private MySqlConnection GetMySqlConnection() => new(_configuration.GetSection("DbConnection").Value!);
    }
}
