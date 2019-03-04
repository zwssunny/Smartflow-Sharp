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
    public class Root
    {
        /// <summary>
        /// 根（表名）
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 表单详情配置项
        /// </summary>
        public List<Son> Items
        {
            get;
            set;
        }
    }
}
