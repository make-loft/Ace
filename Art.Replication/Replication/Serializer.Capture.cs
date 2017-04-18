namespace Art.Replication
{
    public static partial class Serializer
    {
        public static object Capture(this string data, KeepProfile keepProfile, ref int offset)
        {
            switch (keepProfile.MatchHead(data, ref offset))
            {
                case KeepProfile.Map:
                    return new Map().Capture(keepProfile, data, ref offset);
                case KeepProfile.Set:
                    return new Set().Capture(keepProfile, data, ref offset);
                default:
                    var simplex = keepProfile.CaptureSimplex(data, ref offset);
                    return keepProfile.SimplexConverter.Convert(simplex);
            }
        }

        private static object Capture(this object items, KeepProfile keepProfile, string data, ref int offset)
        {
            while (!keepProfile.MatchTail(data, ref offset, items is Map))
            {
                keepProfile.SkipHeadIndent(data, ref offset);

                if (items is Map map)
                {
                    var key = keepProfile.CaptureSimplex(data, ref offset).ToString();
                    map.Add(key, Capture(data, keepProfile, ref offset));
                }
                else if (items is Set set) set.Add(Capture(data, keepProfile, ref offset));

                keepProfile.SkipTailIndent(data, ref offset);
            }

            return items;
        }
    }
}
