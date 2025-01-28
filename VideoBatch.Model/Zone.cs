using NodaTime;
using NodaTime.Extensions;

// Copyright (C) ColhounTech Limited. All rights Reserved
// Author: Micheal Colhoun
// Date: Aug 2021

namespace VideoBatch.Model
{
    public class Zone
    {
        public readonly static DateTimeZone UKZone = DateTimeZoneProviders.Tzdb["Europe/London"];
        public readonly static ZonedClock UKClock = SystemClock.Instance.InZone(UKZone);


    }
}
