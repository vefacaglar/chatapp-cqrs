using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace ChatApp.Infrastructure
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }

    public interface IPostgresDbConnectionFactory : IDbConnectionFactory
    {
    }

    public class PostgresDbConnectionFactory : IPostgresDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public PostgresDbConnectionFactory(
            IConfiguration configuration
            )
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_configuration["ConnectionStrings:ChatDbRead"]);
        }
    }
}
