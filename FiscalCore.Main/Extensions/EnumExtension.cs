using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Main.Extensions
{
    public static class EnumExtension
    {
        public static T ObterAtributo<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (T)attributes[0];
        }

        public static string Descricao(this Enum value)
        {
            var attribute = value.ObterAtributo<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string XmlDescricao(this Enum value)
        {
            var attribute = value.ObterAtributo<XmlEnumAttribute>();
            return attribute == null ? value.ToString() : attribute.Name.ToString();
        }
    }
}
