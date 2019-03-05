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
        public static object GetInstance(string instanceID, FormRelationship relationship)
        {
            //relationship.Name

            //DblHelper.CreateConnection()

            string sql = string.Format(" select * from {0} where instanceID='{0}'",
                            relationship.Name,
                            instanceID);

            dynamic data = DblHelper.CreateConnection().Query(sql);
            foreach (FormRelationship item in relationship.Items)
            {

            }

            return null;
        }
    }
}
