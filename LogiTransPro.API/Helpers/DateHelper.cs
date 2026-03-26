namespace LogiTransPro.API.Helpers
{
    public static class DateHelper
    {
        /// <summary>
        /// Convierte una fecha a formato ISO 8601
        /// </summary>
        public static string ToIsoString(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        /// <summary>
        /// Obtiene la fecha actual en UTC
        /// </summary>
        public static DateTime NowUtc => DateTime.UtcNow;

        /// <summary>
        /// Calcula los días restantes hasta una fecha
        /// </summary>
        public static int DaysRemaining(DateTime targetDate)
        {
            return (int)Math.Ceiling((targetDate - DateTime.UtcNow).TotalDays);
        }

        /// <summary>
        /// Verifica si una fecha es hoy
        /// </summary>
        public static bool IsToday(DateTime date)
        {
            return date.Date == DateTime.UtcNow.Date;
        }

        /// <summary>
        /// Verifica si una fecha está dentro del rango
        /// </summary>
        public static bool IsWithinRange(DateTime date, DateTime start, DateTime end)
        {
            return date >= start && date <= end;
        }

        /// <summary>
        /// Obtiene el inicio del día
        /// </summary>
        public static DateTime StartOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        /// <summary>
        /// Obtiene el fin del día
        /// </summary>
        public static DateTime EndOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, DateTimeKind.Utc);
        }
    }
}