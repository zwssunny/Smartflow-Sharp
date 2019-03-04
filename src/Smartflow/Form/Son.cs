using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartflow.Form
{
    /// <summary>
    /// 表单关系映射
    /// </summary>
    [Serializable]
    public class Son
    {
        /// <summary>
        /// 表名（即组名）
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        ///  外键 relationshipID
        /// </summary>
        public string Relationship
        {
            get;
            set;
        }
    }
}
