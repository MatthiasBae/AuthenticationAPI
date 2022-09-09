namespace AuthenticateUserApi.Services {
    public interface IWeatherDatabase {
        public WeatherForecast? GetForecast(DateTime date);
        public List<WeatherForecast> GetForecasts();
        public void AddForecast(DateTime date,int temperature);
        public void DeleteForecast(WeatherForecast weatherForecast);

        public bool Save();
    }
}
