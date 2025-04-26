using NodaTime;

// Copyright (C) ColhounTech Limited. All rights Reserved
// Author: Micheal Colhoun
// Date: Aug 2021

namespace VideoBatch.Model
{
    public static class Zone
    {
        public static DateTimeZone UKZone => DateTimeZoneProviders.Tzdb["Europe/London"];

        public static Instant GetCurrentInstant()
        {
            return SystemClock.Instance.GetCurrentInstant();
        }

        public static Instant ConvertToUtc(LocalDateTime localTime, DateTimeZone zone)
        {
            return localTime.InZoneStrictly(zone).ToInstant();
        }

        public static LocalDateTime ConvertFromUtc(Instant instant, DateTimeZone targetZone)
        {
            return instant.InZone(targetZone).LocalDateTime;
        }

        public static ZonedDateTime ConvertToZoned(Instant instant, DateTimeZone zone)
        {
            return instant.InZone(zone);
        }
    }
}
