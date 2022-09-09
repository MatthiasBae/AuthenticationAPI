namespace AuthenticateUserApi.Services {
    public class WeatherDatabase : IWeatherDatabase {

        WeatherContext WeatherContext;

        public WeatherDatabase(WeatherContext context) {
            this.WeatherContext = context;
        }

        public void AddForecast(DateTime date,int temperature) {
            this.WeatherContext.WeatherForecasts.Add(
                new WeatherForecast {
                    Date = date,
                    TemperatureC = temperature
                }
            );
            this.Save();
        }

        public void DeleteForecast(WeatherForecast weatherForecast) {
            this.WeatherContext.WeatherForecasts.Remove(weatherForecast);
            this.Save();
        }

        public WeatherForecast? GetForecast(DateTime date) {
            return this.WeatherContext.WeatherForecasts
                .Where(item => item.Date == date)
                .FirstOrDefault();
        }
        public List<WeatherForecast> GetForecasts() {
            return this.WeatherContext.WeatherForecasts.ToList();
        }
        public bool Save() {
            return (this.WeatherContext.SaveChanges() >= 1);
        }
    }
}
