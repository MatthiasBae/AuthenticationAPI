using Microsoft.EntityFrameworkCore;

namespace AuthenticateUserApi.Services {
    public class WeatherContext :DbContext {
        
        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) { 

        }
    }
}
