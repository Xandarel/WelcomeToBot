using System.ComponentModel;
using System.Reflection;

namespace WelcomeToBot.BL.Extension
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            DescriptionAttribute? attr = field?.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? string.Empty;
        }

        public static T? ParseByDescription<T>(string description) where T : struct, Enum
        {
            foreach (T value in Enum.GetValues<T>())
            {
                if (value.GetDescription().Equals(description, StringComparison.Ordinal))
                    return value;
            }
            return null;
        }
    }

}
