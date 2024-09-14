using DotNet8WebApi.MysqlSample.Models;
using DotNet8WebApi.MysqlSample.Services;
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
        private readonly AdoDotNetService _adoDotNetService;

        public BlogController(IConfiguration configuration, AdoDotNetService adoDotNetService)
        {
            _configuration = configuration;
            _adoDotNetService = adoDotNetService;
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

        [HttpGet("V1")]
        public async Task<IActionResult> GetBlogsV1()
        {
            try
            {
                string query = @"SELECT BlogId, BlogTitle, BlogAuthor, BlogContent FROM Tbl_Blog";
                var lst = await _adoDotNetService.QueryAsync<BlogModel>(query);

                return Ok(lst);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlog(int id)
        {
            try
            {
                string query = @"SELECT BlogId, BlogTitle, BlogAuthor, BlogContent FROM Tbl_Blog WHERE BlogId = @BlogId";
                var parameters = new List<MySqlParameter>()
                {
                    new("BlogId", id)
                };

                var dt = await _adoDotNetService.QueryFirstOrDefaultAsync(query, parameters.ToArray());
                if (dt.Rows.Count <= 0)
                {
                    return NotFound("No data found.");
                }

                var blog = new BlogModel()
                {
                    BlogId = Convert.ToInt32(dt.Rows[0]["BlogId"]),
                    BlogTitle = Convert.ToString(dt.Rows[0]["BlogTitle"])!,
                    BlogAuthor = Convert.ToString(dt.Rows[0]["BlogAuthor"])!,
                    BlogContent = Convert.ToString(dt.Rows[0]["BlogContent"])!
                };
                return Ok(blog);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
