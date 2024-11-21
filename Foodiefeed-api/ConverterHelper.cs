namespace Foodiefeed_api
{
    public class ConverterHelper
    {
        public static string ConvertDateTimeToTimeSpan(DateTime dateTime)
        {
            TimeSpan timeSpan = DateTime.Now.ToUniversalTime() - dateTime.ToUniversalTime();

            if (timeSpan.TotalSeconds < 60)
            {
                int seconds = (int)timeSpan.TotalSeconds;
                return seconds == 1 ? "1 second ago" : $"{seconds} seconds ago";
            }
            else if (timeSpan.TotalMinutes < 60)
            {
                int minutes = (int)timeSpan.TotalMinutes;
                return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
            }
            else if (timeSpan.TotalHours < 24)
            {
                int hours = (int)timeSpan.TotalHours;
                return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
            }
            else if (timeSpan.TotalDays < 30)
            {
                int days = (int)timeSpan.TotalDays;
                return days == 1 ? "1 day ago" : $"{days} days ago";
            }
            else if (timeSpan.TotalDays < 365)
            {
                int months = (int)(timeSpan.TotalDays / 30);
                return months == 1 ? "1 month ago" : $"{months} months ago";
            }
            else
            {
                int years = (int)(timeSpan.TotalDays / 365);
                return years == 1 ? "1 year ago" : $"{years} years ago";
            }
        }
    }
}
