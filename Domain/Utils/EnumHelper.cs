using Domain.Data.Models.Utils;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Domain.Utils;

/// <summary>
/// Provides a static utility object of methods and properties to interact
/// with enumerated types.
/// </summary>
public static class EnumHelper
{
    /// <summary>
    /// Gets the <see cref="DescriptionAttribute" /> of an <see cref="Enum" />
    /// type value.
    /// </summary>
    /// <param name="value">The <see cref="Enum" /> type value.</param>
    /// <returns>A string containing the text of the
    /// <see cref="DescriptionAttribute"/>.</returns>
    public static string GetDescription(this Enum value)
    {
        if (value == null)
        {
            throw new ArgumentNullException("value");
        }

        string description = value.ToString();
        FieldInfo? fieldInfo = value.GetType().GetField(description);
        if (fieldInfo == null)
        {
            return description;
        }
        EnumDescriptionAttribute[] attributes = (EnumDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);

        if (attributes != null && attributes.Length > 0)
        {
            description = attributes[0].Description;
        }
        return description;
    }

    public static T GetValueFromDescription<T>(string description)
    {
        string toBecomparedField = "";
        var type = typeof(T);
        if (!type.IsEnum) throw new InvalidOperationException();
        foreach (var field in type.GetFields())
        {
            if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                if (attribute.Description == description)
                {
                    object? value = field.GetValue(null);
                    if (value is T tValue)
                        return tValue;
                    else
                        throw new ArgumentException("Enum value is null or not of expected type.", nameof(description));
                }
            }
            else
            {
                FieldInfo? fieldInfo = type.GetField(field.Name);
                if (fieldInfo != null)
                {
                    EnumDescriptionAttribute[] attributes = (EnumDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
                    if (attributes != null && attributes.Length > 0) { toBecomparedField = attributes[0].Description; }
                    if (toBecomparedField == description)
                    {
                        object? value = field.GetValue(null);
                        if (value is T tValue)
                            return tValue;
                        else
                            throw new ArgumentException("Enum value is null or not of expected type.", nameof(description));
                    }
                }
            }
        }
        throw new ArgumentException("Not found.", "description");
        // or return default(T);
    }

    /// <summary>
    /// Converts the <see cref="Enum" /> type to an <see cref="IList" />
    /// compatible object.
    /// </summary>
    /// <param name="type">The <see cref="Enum"/> type.</param>
    /// <returns>An <see cref="IList"/> containing the enumerated
    /// type value and description.</returns>
    public static IList ToList(this Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        ArrayList list = new ArrayList();
        Array enumValues = Enum.GetValues(type);

        foreach (Enum value in enumValues)
        {
            list.Add(new KeyValuePair<Enum, string>(value, GetDescription(value)));
        }

        return list;
    }

    /// <summary>
    /// Converts the <see cref="Enum" /> type to an <see cref="IList" />
    /// compatible object.
    /// </summary>
    /// <param name="type">The <see cref="Enum"/> type.</param>
    /// <returns>An <see cref="IList"/> containing the enumerated
    /// type value and description.</returns>
    public static IList ToListWithFirstBlank(this Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        ArrayList list = new ArrayList();
        Array enumValues = Enum.GetValues(type);

        list.Add(new KeyValuePair<Enum, string>(null, null));

        foreach (Enum value in enumValues)
        {
            list.Add(new KeyValuePair<Enum, string>(value, GetDescription(value)));
        }

        return list;
    }

    /// <summary>
    /// Converts the <see cref="Enum"/> type to a string <see cref="IList" /> of Desriptions.
    /// </summary>
    /// <param name="type">The <see cref="Enum"/> type.</param>
    /// <returns>An <see cref="IList"/> containing the descriptions.</returns>
    public static IList<string> ToDescriptionList(this Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        IList<string> list = new List<string>();
        Array enumValues = Enum.GetValues(type);

        foreach (Enum value in enumValues)
        {
            list.Add(GetDescription(value));
        }

        return list;
    }

    /// <summary>
    /// Converts the <see cref="Enum"/> type to a <see cref="IList" /> of <see cref="Enum"/>.
    /// </summary>
    /// <param name="type">The <see cref="Enum"/> type.</param>
    /// <returns>An <see cref="IList"/> containing the <see cref="Enum"/>.</returns>
    public static IList<Enum> ToEnumList(this Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        IList<Enum> list = new List<Enum>();
        Array enumValues = Enum.GetValues(type);

        foreach (Enum value in enumValues)
        {
            list.Add(value);
        }

        return list;
    }

    public static List<EnumModel> GetEnumValues(Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("Type must be an enum", nameof(enumType));

        return [.. Enum.GetValues(enumType)
                       .Cast<Enum>()
                       .Select(e => new EnumModel
                       {
                           Value = Convert.ToInt32(e),
                           Description = GetDescription(e)
                       })];
    }
}
