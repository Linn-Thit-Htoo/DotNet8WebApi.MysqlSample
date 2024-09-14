using DotNet8WebApi.MysqlSample.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace DotNet8WebApi.MysqlSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public BlogController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetBlogs()
        {
            try
            {
                var lst = new List<BlogModel>();

                var connection = new MySqlConnection(_configuration.GetConnectionString("DbConnection"));
                await connection.OpenAsync();

                string query = @"SELECT BlogId, BlogTitle, BlogAuthor, BlogContent FROM Tbl_Blog";
                var command = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new();
                MySqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    int id = reader.GetInt32("BlogId");
                    string blogTitle = reader.GetString("BlogTitle");
                    string blogAuthor = reader.GetString("BlogAuthor");
                    string blogContent = reader.GetString("BlogContent");

                    lst.Add(new BlogModel()
                    {
                        BlogId = id,
                        BlogTitle = blogTitle,
                        BlogAuthor = blogAuthor,
                        BlogContent = blogContent
                    });
                }

                await connection.CloseAsync();

                return Ok(lst);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
