using Dapper;
using Microsoft.Data.SqlClient;
using HomeTry.Models;

namespace HomeTry.Repositories
{
    public class LitterRepository
    {
        private readonly string sqlConnectionString;

        public LitterRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        //insert a new record
        public async Task<Litter> InsertAsync(Litter litter, Weather weather)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.OpenAsync();

            // Insert Litter referencing the weather
            await sqlConnection.ExecuteAsync(
                @"INSERT INTO Litter (litter_id, litter_classification, confidence, location_latitude, location_longitude)
                  VALUES (@litter_id, @litter_classification, @confidence, @location_latitude, @location_longitude)", litter);

            // Insert Weather first
            await sqlConnection.ExecuteAsync(
                @"INSERT INTO Weather (weather_id, temperature_celsius, humidity, conditions)
                  VALUES (@weather_id, @temperature_celsius, @humidity, @conditions)", weather);

            // Return the full inserted Litter + Weather
            var sql =
                @"SELECT Litter.litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                         Weather.weather_id AS weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                  FROM Litter
                  JOIN Weather ON Litter.litter_id = Weather.weather_id
                  WHERE Litter.litter_id = @id
                  ORDER BY detection_time DESC";

            var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
            {
                litter.weather = weather;
                return litter;
            }, new { id = litter.litter_id }, splitOn: "weather_id");

            return result.FirstOrDefault();
        }


        //read all litter + weather records
        public async Task<IEnumerable<Litter>> ReadAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var sql =
                    @"SELECT Litter.litter_id AS litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                    Weather.weather_id AS weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                    FROM dbo.Litter
                    JOIN Weather ON Litter.litter_id = Weather.weather_id
                    ORDER BY detection_time DESC";

                var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
                {
                    litter.weather = weather;
                    return litter;
                },
                splitOn: "weather_id");
                return result;
            }
        }

        //read a litter + weather record by litter id
        public async Task<Litter?> ReadAsync(Guid id)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            var sql =
                @"SELECT Litter.litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                         Weather.weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                  FROM Litter
                  JOIN Weather ON Litter.litter_id = Weather.weather_id
                  WHERE Litter.litter_id = @id
                  ORDER BY detection_time DESC";

            var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
            {
                litter.weather = weather;
                return litter;
            }, new { id }, splitOn: "weather_id");

            return result.FirstOrDefault();
        }

        //read all litter + weather records from a certain date
        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            var sql =
                @"SELECT Litter.litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                         Weather.weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                  FROM Litter
                  JOIN Weather ON Litter.litter_id = Weather.weather_id
                  WHERE Litter.detection_time >= @startTime
                  ORDER BY detection_time DESC";

            var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
            {
                litter.weather = weather;
                return litter;
            }, new { startTime }, splitOn: "weather_id");

            return result;
        }

        //read all litter + weather records from a certain date with a certain classification
        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, int litterClassification)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            var sql =
                @"SELECT Litter.litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                         Weather.weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                  FROM Litter
                  JOIN Weather ON Litter.litter_id = Weather.weather_id
                  WHERE Litter.detection_time >= @startTime AND Litter.litter_classification = @litterClassification
                  ORDER BY detection_time DESC";

            var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
            {
                litter.weather = weather;
                return litter;
            }, new { startTime, litterClassification }, splitOn: "weather_id");

            return result;
        }

        //read all litter + weather records between certain dates
        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, DateTime stopTime)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            var sql =
                @"SELECT Litter.litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                         Weather.weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                  FROM Litter
                  JOIN Weather ON Litter.litter_id = Weather.weather_id
                  WHERE Litter.detection_time >= @startTime AND Litter.detection_time <= @stopTime
                  ORDER BY detection_time DESC";

            var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
            {
                litter.weather = weather;
                return litter;
            }, new { startTime, stopTime }, splitOn: "weather_id");

            return result;
        }

        //read all litter + weather records between certain dates and fit a certain class
        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, DateTime stopTime, int litterClassification)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            var sql =
                @"SELECT Litter.litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                         Weather.weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                  FROM Litter
                  JOIN Weather ON Litter.litter_id = Weather.weather_id
                  WHERE Litter.detection_time >= @startTime AND Litter.detection_time <= @stopTime AND Litter.litter_classification = @litterClassification
                  ORDER BY detection_time DESC";

            var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
            {
                litter.weather = weather;
                return litter;
            }, new { startTime, stopTime, litterClassification }, splitOn: "weather_id");

            return result;
        }

        //read all litter + weather records with a specific class
        public async Task<IEnumerable<Litter>> ReadAsync(int litterClassification)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            var sql =
                @"SELECT Litter.litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                         Weather.weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                  FROM Litter
                  JOIN Weather ON Litter.litter_id = Weather.weather_id
                  WHERE Litter.litter_classification = @litterClassification
                  ORDER BY detection_time DESC";

            var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
            {
                litter.weather = weather;
                return litter;
            }, new { litterClassification }, splitOn: "weather_id");

            return result;
        }
    }
}
