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

            DateTime maxDate = DateTime.Today.AddDays(1);
            DateTime minDate = new DateTime(2025, 05, 01);
                        
            if(dateTime > minDate && dateTime < maxDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
