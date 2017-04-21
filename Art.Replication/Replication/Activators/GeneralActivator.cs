using System;
using System.Text.RegularExpressions;

namespace Art.Replication.Activators
{
    public interface IActivator
    {
        object CreateInstance(Map map, Type type, params object[] args);
    }

    public class RegexActivator : IActivator
    {
        public object CreateInstance(Map map, Type type, params object[] args)
        {
            if (type != typeof(Regex)) return null;
            return new Regex(map["pattern"].ToString());
        }
    }
}
