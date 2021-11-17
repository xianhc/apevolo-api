using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace ApeVolo.Common.Extention
{
    /// <summary>
    /// 时间转换器 全局序列化使用
    /// </summary>
    public class UnixDateTimeConvertor : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            long jsTimeStamp = long.Parse(reader.Value.ToString());
            System.DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
            DateTime dt = startTime.AddMilliseconds(jsTimeStamp);
            return dt;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            System.DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
            long timeStamp = (long)(((DateTime)value) - startTime).TotalMilliseconds;
            writer.WriteValue(timeStamp);
        }
    }
}
