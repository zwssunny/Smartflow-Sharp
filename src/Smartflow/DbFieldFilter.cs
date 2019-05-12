using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartflow
{
    public class DbFieldFilter : IFilter
    {
        public string Filter(string cmdText)
        {
            return cmdText;
        }
    }
}
