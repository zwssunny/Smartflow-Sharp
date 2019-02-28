/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using Smartflow.Dapper;
using Smartflow.Enums;
using Newtonsoft.Json;

namespace Smartflow.Elements
{
    public class Command : Element, IRelationship
    {
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        [JsonProperty("script")]
        [XmlElement("script")]
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        [JsonProperty("connecte")]
        [XmlElement("connecte")]
        public string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// 数据库访问提供者
        /// </summary>
        [JsonProperty("providername")]
        [XmlElement("providername")]
        public string ProviderName
        {
            get;
            set;
        }

        /// <summary>
        /// 命令类型（目前只支持文本命令、后续扩展存储过程）
        /// </summary>
        [JsonProperty("commandtype")]
        [XmlElement("commandtype")]
        public string CommandType
        {
            get;
            set;
        }

        [JsonIgnore]
        [XmlIgnore]
        public string RelationshipID
        {
            get;
            set;
        }

        internal override void Persistent()
        {
            string sql = "INSERT INTO T_Command(NID,RelationshipID,Name,Text,ConnectionString,InstanceID,ProviderName,CommandType) VALUES(@NID,@RelationshipID,@Name,@Text,@ConnectionString,@InstanceID,@ProviderName,@CommandType)";
            Connection.Execute(sql, new
            {
                NID = Guid.NewGuid().ToString(),
                RelationshipID = RelationshipID,
                Name = Name,
                Text = Text,
                ConnectionString = ConnectionString,
                InstanceID = InstanceID,
                ProviderName = ProviderName,
                CommandType = CommandType
            });
        }
    }
}
