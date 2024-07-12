using Ape.Volo.Common.Attributes;
using Ape.Volo.Common.Exception;
using Ape.Volo.Common.Extensions;

namespace Ape.Volo.Common.Helper;

public static class RequiredHelper
{
    public static void IsValid<T>(T entity) where T : new()
    {
        var message = "";
        bool isTrue = true;
        if (entity.IsNull())
            throw new BadRequestException("Object cannot be null. (Parameter '" + new T().GetType().Name + "')");
        var reqName = nameof(ApeVoloRequiredAttribute);
        var pis = entity.GetType().GetProperties();
        foreach (var pi in pis)
        {
            var isReq = false;
            object[] attrs = pi.GetCustomAttributes(true);
            foreach (var attr in attrs)
            {
                if (attr.GetType().Name == "JsonIgnoreAttribute") continue;

                message = ((ApeVoloRequiredAttribute)attr).Message;
                var agt = attr.GetType();
                if (agt.Name == reqName)
                {
                    isReq = true;
                    break;
                }
            }

            if (!isReq) continue;
            var value = pi.GetValue(entity, null);
            if (value.IsNullOrEmpty())
            {
                isTrue = false;
                break;
            }

            var dtlType = pi.PropertyType.Name;
            if (dtlType == "Int32")
            {
                if ((dynamic)value < 0)
                {
                    isTrue = false;
                    break;
                }
            }
        }

        if (!isTrue)
        {
            throw new BadRequestException(message);
        }
    }
}
