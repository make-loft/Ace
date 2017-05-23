using System;

namespace Art.Serialization.Serializers
{
    public class DateTimeIsoFastConverter : StringSerializer
    {
        public string DateTimeOffsetFormat = "O";
        public string DateTimeFormat = "O";
        public string TimeSpanFormat = "G";

        public override bool CanApply(object value, KeepProfile keepProfile) => 
            value is DateTime || value is DateTimeOffset || value is TimeSpan;

        public override string Convert(object value)
        {
            switch (value)
            {
                case DateTime d:
                    return $"{d.Year}-{d.Month}-{d.Day}T{d.Hour}:{d.Minute}:{d.Second}.{d.Millisecond}";
                case DateTimeOffset d:
                    return $"{d.Year}-{d.Month}-{d.Day}T{d.Hour}:{d.Minute}:{d.Second}.{d.Millisecond}+{d.Offset}";
                case TimeSpan d:
                    return d.ToString(TimeSpanFormat, ActiveCulture);
                default:
                    return null;
            }
        }
    }
}