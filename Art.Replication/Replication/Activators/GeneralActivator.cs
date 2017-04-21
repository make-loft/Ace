using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Art.Replication.Activators
{
    public class GeneralActivator
    {
        public object CreateInstance(object sample, Type type, params object[] args)
        {
            var instance = Activator.CreateInstance(type, args);
            return instance;
        }
    }
}
