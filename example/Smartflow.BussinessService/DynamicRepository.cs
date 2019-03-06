using Smartflow.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;

namespace Smartflow.BussinessService
{
    public class DynamicRepository
    {

        private static Dictionary<string, Type> _cache = new Dictionary<string, Type>();

        public static object GetInstance(string instanceID, FormRelationship relationship)
        {
            string sql = string.Format(" select * from {0} where instanceID='{0}'",
                            relationship.Name,
                            instanceID);

            Dictionary<string, Type> properties = new Dictionary<string, Type>();
            foreach (FormRelationship item in relationship.Items)
            {
                Dictionary<string, Type> subProperties = new Dictionary<string, Type>();
                if (!_cache.ContainsKey(item.Name))
                {
                    Type subType = TypeCreator.Creator(item.Name, subProperties);
                    Type genericType = typeof(List<>);
                    properties[item.Name] = genericType.MakeGenericType(subType);
                }
            }

            Type baseType = TypeCreator.Creator(relationship.Name, properties);
            Object dataObject = DblHelper.CreateConnection().Query(baseType, sql);

            foreach (FormRelationship item in relationship.Items)
            {
                sql = string.Format(" select * from {0} where instanceID='{0}'",item.Name,instanceID);
                dataObject.SetValue(item.Name, DblHelper.CreateConnection().Query(_cache[item.Name], sql));
            }

            return dataObject;
        }
    }
}
