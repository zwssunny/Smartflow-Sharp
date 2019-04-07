using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartflow.Form
{
    public interface IBase
    {
        string INSTANCEID
        {
            get;
            set;
        }

        string RESOURCEID
        {
            get;
            set;
        }
    }
}
