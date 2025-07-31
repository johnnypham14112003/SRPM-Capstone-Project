using System.Reflection;
using System.Runtime.Serialization;

namespace SRPM_Services.Extensions.Enumerables;

public static class EnumUtils
{
    public static string GetEnumMemberValue(this Enum enumValue)
    {
        return enumValue
            .GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault()?
            .GetCustomAttribute<EnumMemberAttribute>()?
            .Value ?? enumValue.ToString();
    }

}
