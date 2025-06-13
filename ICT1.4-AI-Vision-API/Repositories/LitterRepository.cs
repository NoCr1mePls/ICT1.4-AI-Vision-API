using Dapper;
using Microsoft.Data.SqlClient;
using HomeTry.Models;
using HomeTry.Interfaces;

namespace HomeTry.Repositories
{
    public class LitterRepository : ILitterRepository
    {
        private readonly string sqlConnectionString;

        public LitterRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        /// <summary>
        /// Inserts a weather and litter record into the database.
        /// </summary>
        /// <param name="litter">The litter model to insert.</param>
        /// <param name="weather">The weather model to insert.</param>
        /// <returns>The inserted litter model with correlating weather data.</returns>
        public async Task<Litter> InsertAsync(Litter litter, Weather weather)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.OpenAsync();

            //inserts weather record
            await sqlConnection.ExecuteAsync(
                @"INSERT INTO Litter (litter_id, litter_classification, confidence, location_latitude, location_longitude)
                  VALUES (@litter_id, @litter_classification, @confidence, @location_latitude, @location_longitude)", litter);
            //inserts litter record
            await sqlConnection.ExecuteAsync(
                @"INSERT INTO Weather (weather_id, temperature_celsius, humidity, conditions)
                  VALUES (@weather_id, @temperature_celsius, @humidity, @conditions)", weather);

            //selects the combination of the litter and weather record previosly made
            var sql =
                @"SELECT Litter.litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                         Weather.weather_id AS weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                  FROM Litter
                  JOIN Weather ON Litter.litter_id = Weather.weather_id
                  WHERE Litter.litter_id = @id
                  ORDER BY detection_time DESC";

            //parses the weather information into a litter model
            var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
            {
                litter.weather = weather;
                return litter;
            }, new { id = litter.litter_id }, splitOn: "weather_id");

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves all litter records with correlating weather data from the database.
        /// </summary>
        /// <returns>A collection of all litter records.</returns>
        public async Task<IEnumerable<Litter>> ReadAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                //select weather and litter records and return a combined table where litter_id = weather_id
                var sql =
                    @"SELECT Litter.litter_id AS litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                    Weather.weather_id AS weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                    FROM dbo.Litter
                    JOIN Weather ON Litter.litter_id = Weather.weather_id
                    ORDER BY detection_time DESC";

                //parses the weather information into a litter model
                var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
                {
                    litter.weather = weather;
                    return litter;
                },
                splitOn: "weather_id");
                return result;
            }
        }

        /// <summary>
        /// Retrieves a specific litter record by its ID, including associated weather data.
        /// </summary>
        /// <param name="id">The unique identifier of the litter record.</param>
        /// <returns>The matching litter record if found; otherwise, null.</returns>
        public async Task<Litter?> ReadAsync(Guid id)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            //select weather and litter records and return a combined table where litter_id = weather_id
            var sql =
                @"SELECT Litter.litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                         Weather.weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                  FROM Litter
                  JOIN Weather ON Litter.litter_id = Weather.weather_id
                  WHERE Litter.litter_id = @id
                  ORDER BY detection_time DESC";

            //parses the weather information into a litter model
            var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
            {
                litter.weather = weather;
                return litter;
            }, new { id }, splitOn: "weather_id");

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves litter records detected after a specified start time, including associated weather data.
        /// </summary>
        /// <param name="startTime">The start time to filter records by.</param>
        /// <returns>A collection of matching litter records.</returns>
        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            //select weather and litter records and return a combined table where litter_id = weather_id
            var sql =
                @"SELECT Litter.litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                         Weather.weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                  FROM Litter
                  JOIN Weather ON Litter.litter_id = Weather.weather_id
                  WHERE Litter.detection_time >= @startTime
                  ORDER BY detection_time DESC";

            //parses the weather information into a litter model
            var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
            {
                litter.weather = weather;
                return litter;
            }, new { startTime }, splitOn: "weather_id");

            return result;
        }

        /// <summary>
        /// Retrieves litter records detected after a specified start time and matching a specific classification,
        /// including associated weather data.
        /// </summary>
        /// <param name="startTime">The start time to filter records by.</param>
        /// <param name="litterClassification">The classification to filter litter records by.</param>
        /// <returns>A collection of matching litter records.</returns>
        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, int litterClassification)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            //select weather and litter records and return a combined table where litter_id = weather_id
            var sql =
                @"SELECT Litter.litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                         Weather.weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                  FROM Litter
                  JOIN Weather ON Litter.litter_id = Weather.weather_id
                  WHERE Litter.detection_time >= @startTime AND Litter.litter_classification = @litterClassification
                  ORDER BY detection_time DESC";

            //parses the weather information into a litter model
            var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
            {
                litter.weather = weather;
                return litter;
            }, new { startTime, litterClassification }, splitOn: "weather_id");

            return result;
        }

        /// <summary>
        /// Retrieves litter records detected between a start and stop time, including associated weather data.
        /// </summary>
        /// <param name="startTime">The start time to filter records by.</param>
        /// <param name="stopTime">The end time to filter records by.</param>
        /// <returns>A collection of matching litter records.</returns>
        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, DateTime stopTime)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            //select weather and litter records and return a combined table where litter_id = weather_id
            var sql =
                @"SELECT Litter.litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                         Weather.weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                  FROM Litter
                  JOIN Weather ON Litter.litter_id = Weather.weather_id
                  WHERE Litter.detection_time >= @startTime AND Litter.detection_time <= @stopTime
                  ORDER BY detection_time DESC";

            //parses the weather information into a litter model
            var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
            {
                litter.weather = weather;
                return litter;
            }, new { startTime, stopTime }, splitOn: "weather_id");

            return result;
        }

        /// <summary>
        /// Retrieves litter records detected between a start and stop time and matching a specific classification,
        /// including associated weather data.
        /// </summary>
        /// <param name="startTime">The start time to filter records by.</param>
        /// <param name="stopTime">The end time to filter records by.</param>
        /// <param name="litterClassification">The classification to filter litter records by.</param>
        /// <returns>A collection of matching litter records.</returns>
        public async Task<IEnumerable<Litter>> ReadAsync(DateTime startTime, DateTime stopTime, int litterClassification)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            //select weather and litter records and return a combined table where litter_id = weather_id
            var sql =
                @"SELECT Litter.litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                         Weather.weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                  FROM Litter
                  JOIN Weather ON Litter.litter_id = Weather.weather_id
                  WHERE Litter.detection_time >= @startTime AND Litter.detection_time <= @stopTime AND Litter.litter_classification = @litterClassification
                  ORDER BY detection_time DESC";

            //parses the weather information into a litter model
            var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
            {
                litter.weather = weather;
                return litter;
            }, new { startTime, stopTime, litterClassification }, splitOn: "weather_id");

            return result;
        }

        /// <summary>
        /// Retrieves litter records matching a specific classification, including associated weather data.
        /// </summary>
        /// <param name="litterClassification">The classification to filter litter records by.</param>
        /// <returns>A collection of matching litter records.</returns>
        public async Task<IEnumerable<Litter>> ReadAsync(int litterClassification)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            //select weather and litter records and return a combined table where litter_id = weather_id
            var sql =
                @"SELECT Litter.litter_id, Litter.litter_classification, Litter.confidence, Litter.location_latitude, Litter.location_longitude, Litter.detection_time,
                         Weather.weather_id, Weather.temperature_celsius, Weather.humidity, Weather.conditions
                  FROM Litter
                  JOIN Weather ON Litter.litter_id = Weather.weather_id
                  WHERE Litter.litter_classification = @litterClassification
                  ORDER BY detection_time DESC";
            
            //parses the weather information into a litter model
            var result = await sqlConnection.QueryAsync<Litter, Weather, Litter>(sql, (litter, weather) =>
            {
                litter.weather = weather;
                return litter;
            }, new { litterClassification }, splitOn: "weather_id");

            return result;
        }
    }
}
