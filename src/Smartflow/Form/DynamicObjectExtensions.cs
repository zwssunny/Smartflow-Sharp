using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Smartflow.Form
{
    public static class DynamicObjectExtensions
    {
        public static bool Constant(this Object data, string propetyName)
        {
            return data.GetType().GetProperty(propetyName) != null;
        }

        public static object Value(this Object data, string propetyName)
        {
            if (data.Constant(propetyName))
            {
                return data.GetType().GetProperty(propetyName).GetValue(data, null);
            }
            return null;
        }
    }
}
