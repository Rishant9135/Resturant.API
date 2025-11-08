namespace DataServiceAPI.Helper
{
    public class Helper
    {
        public DateTime GetCurrentIST()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
            );
        }

    }
}
