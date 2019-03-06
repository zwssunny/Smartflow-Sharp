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
    public class FormRelationship
    {
        /// <summary>
        /// 组名
        /// </summary>
        public string Group
        {
            get;
            set;
        }

        /// <summary>
        /// 根（表名）
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 关系标识
        /// </summary>
        public string Relationship
        {
            get;
            set;
        }

        /// <summary>
        /// 表单详情配置项
        /// </summary>
        public List<FormRelationship> Items
        {
            get;
            set;
        }
    }
}
