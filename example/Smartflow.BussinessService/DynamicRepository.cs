using Dapper;
using Smartflow.Form;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Smartflow.BussinessService
{
    public class DynamicRepository
    {
        private static Dictionary<string, Type> _cache = new Dictionary<string, Type>();

        public static object GetInstance(string instanceID, FormRelationship relationship)
        {
            IDbConnection connection = DblHelper.CreateConnection();
            string sql = string.Format(" select * from {0} where instanceID='{0}'", relationship.Name, instanceID);
            Dictionary<string, Type> properties = GetProprties(relationship.Name);
            foreach (FormRelationship item in relationship.Items)
            {
                Dictionary<string, Type> subProperties = GetProprties(item.Name); ;
                if (!_cache.ContainsKey(item.Name))
                {
                    Type subType = TypeCreator.Creator(item.Name, subProperties);
                    Type genericType = typeof(List<>);
                    properties[item.Name] = genericType.MakeGenericType(subType);
                }
            }

            Type baseType = TypeCreator.Creator(relationship.Name, properties);
            Object dataObject = connection.Query(baseType, sql);

            foreach (FormRelationship item in relationship.Items)
            {
                sql = string.Format(" select * from {0} where instanceID='{0}'", item.Name, instanceID);
                dataObject.SetValue(item.Name, connection.Query(_cache[item.Name], sql));
            }

            return dataObject;
        }

        public static void Persistent(Object proxy, FormRelationship relationship)
        {
            IDbConnection connection = DblHelper.CreateConnection();
            connection.Execute(InsertCommand(relationship.Name), proxy);
            foreach (FormRelationship item in relationship.Items)
            {
                if (proxy.Value(item.Name) is System.Collections.IEnumerable)
                {
                    var collection = proxy.Value(item.Name) as System.Collections.IEnumerable;
                    foreach (var subProxy in collection)
                    {
                        connection.Execute(InsertCommand(item.Name), subProxy);
                    }
                }
            }
        }

        public static Type BuildDynamicObjectType(FormRelationship relationship)
        {
            IDbConnection connection = DblHelper.CreateConnection();
            Dictionary<string, Type> properties = GetProprties(relationship.Name);
            foreach (FormRelationship item in relationship.Items)
            {
                Dictionary<string, Type> subProperties = GetProprties(item.Name); ;
                if (!_cache.ContainsKey(item.Name))
                {
                    Type subType = TypeCreator.Creator(item.Name, subProperties);
                    Type genericType = typeof(List<>);
                    properties[item.Name] = genericType.MakeGenericType(subType);
                }
            }

            return TypeCreator.Creator(relationship.Name, properties);
        }

        private static Dictionary<string, Type> GetProprties(string identity)
        {
            IDbConnection connection = DblHelper.CreateConnection();
            string fieldSQL = string.Format(" SELECT COLUMN_NAME,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{0}' ", identity);
            List<FieldInfo> fields = connection.Query<FieldInfo>(fieldSQL).ToList();
            Dictionary<string, Type> properties = new Dictionary<string, Type>();
            foreach (FieldInfo field in fields)
            {
                SqlDbType dbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), field.DATA_TYPE);
                properties.Add(field.COLUMN_NAME, DbTypeCsharpType(dbType));
            }
            return properties;
        }

        private static string InsertCommand(string identity)
        {
            IDbConnection connection = DblHelper.CreateConnection();
            string fieldSQL = string.Format(" SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{0}' ", identity);
            List<string> fields = connection.Query<string>(fieldSQL).ToList();
            StringBuilder command = new StringBuilder();

            command.Append(" INSERT INTO ")
              .Append(identity)
              .Append(" ( ")
              .Append(string.Join(",", fields))
              .Append(" ) ")
              .Append(" VALUES ")
              .Append(" ( ")
              .Append(string.Join(",@", fields))
              .Append(" ) ");


            return command.ToString();
        }

        private static Type DbTypeCsharpType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(Int64);
                case SqlDbType.Binary:
                    return typeof(Object);
                case SqlDbType.Bit:
                    return typeof(Boolean);
                case SqlDbType.Char:
                    return typeof(String);
                case SqlDbType.DateTime:
                    return typeof(DateTime);
                case SqlDbType.Decimal:
                    return typeof(Decimal);
                case SqlDbType.Float:
                    return typeof(Double);
                case SqlDbType.Image:
                    return typeof(Object);
                case SqlDbType.Int:
                    return typeof(Int32);
                case SqlDbType.Money:
                    return typeof(Decimal);
                case SqlDbType.NChar:
                    return typeof(String);
                case SqlDbType.NText:
                    return typeof(String);
                case SqlDbType.NVarChar:
                    return typeof(String);
                case SqlDbType.Real:
                    return typeof(Single);
                case SqlDbType.SmallDateTime:
                    return typeof(DateTime);
                case SqlDbType.SmallInt:
                    return typeof(Int16);
                case SqlDbType.SmallMoney:
                    return typeof(Decimal);
                case SqlDbType.Text:
                    return typeof(String);
                case SqlDbType.Timestamp:
                    return typeof(Object);
                case SqlDbType.TinyInt:
                    return typeof(Byte);
                case SqlDbType.Udt://自定义的数据类型
                    return typeof(Object);
                case SqlDbType.UniqueIdentifier:
                    return typeof(Object);
                case SqlDbType.VarBinary:
                    return typeof(Object);
                case SqlDbType.VarChar:
                    return typeof(String);
                case SqlDbType.Variant:
                    return typeof(Object);
                case SqlDbType.Xml:
                    return typeof(Object);
                default:
                    return null;
            }
        }
    }
}
