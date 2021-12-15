using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApeVolo.Common.DB
{
    public static class BaseDbConfig
    {
        public static (DataBaseOperate MasterDb, List<DataBaseOperate> SlaveDbs) GetDataBaseOperate =>
            InitDataBaseConn();

        private static (DataBaseOperate, List<DataBaseOperate>) InitDataBaseConn()
        {
            DataBaseOperate masterDb = null;
            var slaveDbs = new List<DataBaseOperate>();
            var allDbs = new List<DataBaseOperate>();
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
                        if (!secJt[i]["Enabled"]
                            .ToBool()) //|| secJt[i]["ConnId"]?.ToString() != DatabaseEntry.CurrentDbConnId)
                            continue;
                        allDbs.Add(new DataBaseOperate
                        {
                            ConnId = secJt[i]["ConnId"]?.ToString(),
                            HitRate = secJt[i]["HitRate"].ToInt(),
                            ConnectionString = secJt[i]["ConnectionString"]?.ToString(),
                            DbType = (DataBaseType) secJt[i]["DBType"].ToInt()
                        });
                    }
                }
            }

            if (allDbs.Count < 1)
            {
                throw new System.Exception("请确保appsettings.json中配置连接字符串,并设置Enabled为true;");
            }

            masterDb = allDbs.FirstOrDefault(x => x.ConnId == GlobalVar.CurrentDbConnId);
            if (masterDb.IsNull())
            {
                throw new System.Exception($"请确保主库ID:{GlobalVar.CurrentDbConnId}的Enabled为true;");
            }

            //如果开启读写分离
            if (AppSettings.GetValue("CQRSEnabled").ToBool())
            {
                slaveDbs = allDbs.Where(x => x.DbType == masterDb.DbType && x.ConnId != GlobalVar.CurrentDbConnId)
                    .ToList();
                if (slaveDbs.Count < 1)
                {
                    throw new System.Exception($"请确保主库ID:{GlobalVar.CurrentDbConnId}对应的从库的Enabled为true;");
                }
            }


            return (masterDb, slaveDbs);
        }
    }
}