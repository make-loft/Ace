using Art.Replication;

namespace Art.Serialization.Serializers
{
    public static partial class Serializer
    {
        public static object Capture(this string matrix, KeepProfile keepProfile, ref int offset)
        {
            switch (keepProfile.MatchHead(matrix, ref offset)) /* ("{" or "[") else simplex value */
            {
                case KeepProfile.Map:
                    return new Map().CaptureComplex(keepProfile, matrix, ref offset);
                case KeepProfile.Set:
                    return new Set().CaptureComplex(keepProfile, matrix, ref offset);
                default:
                    var simplex = keepProfile.CaptureSimplex(matrix, ref offset);
                    var value = keepProfile.SimplexConverter.Convert(simplex);
                    return value;
            }
        }

        private static object CaptureComplex(this object items, KeepProfile keepProfile, string data, ref int offset)
        {
            while (!keepProfile.MatchTail(data, ref offset, items is Map)) /* "}" or "]" */
            {
                keepProfile.SkipHeadIndent(data, ref offset);

                if (items is Map map)
                {
                    var key = keepProfile.CaptureSimplex(data, ref offset).ToString();
                    map.Add(key, data.Capture(keepProfile, ref offset));
                }
                else if (items is Set set) set.Add(data.Capture(keepProfile, ref offset));

                keepProfile.SkipTailIndent(data, ref offset);
            }

            return items;
        }
    }
}
