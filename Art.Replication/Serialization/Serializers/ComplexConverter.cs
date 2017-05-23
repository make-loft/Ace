using System;

namespace Art.Serialization.Serializers
{
    public class ComplexConverter : StringSerializer
    {
        public string DateTimeOffsetFormat = "O";
        public string DateTimeFormat = "O";
        public string TimeSpanFormat = "G";
        public string GuidFormat = "D";

        public override bool CanApply(object value, KeepProfile keepProfile) => 
            value.GetType().IsValueType || value is Guid || value is Uri || value is Enum;

        public override string Convert(object value)
        {
            switch (value)
            {
                case Enum e:
                    return e.ToString();
                case Type t:
                    return t.AssemblyQualifiedName;
                case Uri u:
                    return u.ToString();
                case Guid d:
                    return d.ToString(GuidFormat);
                case DateTime d:
                    return d.ToString(DateTimeFormat, ActiveCulture);
                case DateTimeOffset d:
                    return d.ToString(DateTimeOffsetFormat, ActiveCulture);
                case TimeSpan d:
                    return d.ToString(TimeSpanFormat, ActiveCulture);
                default:
                    return null;
            }
        }
    }
}