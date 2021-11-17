using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;

namespace ApeVolo.Common.DB
{
    public static class BaseDbConfig
    {
        public static DataBaseOperate GetDataBaseOperate => InitDataBaseConn();

        private static DataBaseOperate InitDataBaseConn()
        {
            DataBaseOperate dataBase = null;
            string path = "appsettings.json";
            using var file = new StreamReader(path);
            using var reader = new JsonTextReader(file);
            var jObj = (JObject) JToken.ReadFrom(reader);
            if (!string.IsNullOrWhiteSpace("DBS"))
            {
                var secJt = jObj["DBS"];
                if (secJt != null)
                {
                    for (int i = 0; i < secJt.Count(); i++)
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        if (!secJt[i]["Enabled"].ToBool() ||
                            secJt[i]["ConnId"]?.ToString() != DatabaseEntry.CurrentDbConnId) continue;
                        dataBase = new DataBaseOperate()
                        {
                            ConnId = secJt[i]["ConnId"]?.ToString(),
                            Conn = secJt[i]["Connection"]?.ToString(),
                            DbType = (DataBaseType) secJt[i]["DBType"].ToInt(),
                        };
                        break;
                    }
                }
            }

            if (dataBase.IsNull())
            {
                throw new System.Exception("请确保appsettings.json中配置连接字符串,并设置Enabled为true;");
            }

            return dataBase;
        }
    }
}