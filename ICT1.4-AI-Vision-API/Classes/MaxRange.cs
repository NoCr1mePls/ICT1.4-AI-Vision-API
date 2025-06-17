using Microsoft.EntityFrameworkCore.Design;
using System.Globalization;

namespace SensoringApi.Classes
{
    public class MaxRange
    {
        public bool DateAllowed(DateTime? dateTime, DateOnly? dateOnly)
        {
            if(dateOnly.HasValue)
            {
                dateTime = dateOnly.Value.ToDateTime(TimeOnly.MaxValue);
            }

            DateTime maxDate = DateTime.Today + TimeOnly.MaxValue.ToTimeSpan();
            DateTime minDate = new DateTime(2025, 05, 02);
                        
            if(dateTime > minDate && dateTime < maxDate)
            {
                Console.WriteLine($"minDate: {minDate} < dateTime: {dateTime} < maxDate: {maxDate}");
                return true;
            }
            else
            {
                Console.WriteLine($"!(minDate: {minDate} < dateTime: {dateTime} < maxDate: {maxDate})");
                return false;
            }
        }
    }
}
