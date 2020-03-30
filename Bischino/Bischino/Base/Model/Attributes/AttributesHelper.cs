using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Bischino.Base.Model.Attributes
{
    public static class AttributesHelper
    {
        private static Dictionary<Type, IList<Type>> PossibleObserversDictionary { get; } = new Dictionary<Type, IList<Type>>();
        private static Dictionary<(Type, string), IList<PropertyInfo>> HiddenPropertiesDictionary { get; } = new Dictionary<(Type, string), IList<PropertyInfo>>();
        private static Dictionary<Type, IList<PropertyInfo>> PrivatePropertiesDictionary { get; } = new Dictionary<Type, IList<PropertyInfo>>();

        public static IList<PropertyInfo> GetHiddenProperties(Type t, string roleName)
        {
            if (HiddenPropertiesDictionary.TryGetValue((t, roleName), out var properties))
                return properties;

            properties = (from property in t.GetProperties()
                          from attribute in property.GetCustomAttributes(true)
                          where attribute is VisibleByAttribute
                          let visibleAttr = attribute as VisibleByAttribute
                          where !visibleAttr.IsVisibleBy(roleName)
                          select property).ToArray();
            HiddenPropertiesDictionary.Add((t, roleName), properties);
            return properties;
        }

        public static IList<Type> PossibleObserversByType(Type type)
        {
            if (PossibleObserversDictionary.TryGetValue(type, out var typePossibleObservers))
                return typePossibleObservers;
            typePossibleObservers =
                (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                 from assemblyType in domainAssembly.GetTypes()
                 where type.IsAssignableFrom(assemblyType)
                 select assemblyType).ToArray();
            PossibleObserversDictionary.Add(type, typePossibleObservers);
            return typePossibleObservers;
        }

        public static IList<PropertyInfo> GetPrivateProperties(Type t)
        {
            if (PrivatePropertiesDictionary.TryGetValue(t, out var privateProperties))
                return privateProperties;
            privateProperties = (
                from property in t.GetProperties()
                from attribute in property.GetCustomAttributes(true)
                where attribute is PrivateAttribute
                let privateAttribute = attribute as PrivateAttribute
                select property).ToArray();

            PrivatePropertiesDictionary.Add(t, privateProperties);
            return privateProperties;
        }


        public static void GetHiddenAndPrivateProperties(Type t, string roleName, out IList<PropertyInfo> hiddenProperties, out IList<PropertyInfo> privateProperties)
        {
            var hiddenInDictionary = HiddenPropertiesDictionary.TryGetValue((t, roleName), out _);
            var privateInDictionary = PrivatePropertiesDictionary.TryGetValue(t, out _);

            void GetProperties()
            {
                IList<PropertyInfo> hiddenProps = new List<PropertyInfo>();
                IList<PropertyInfo> privateProps = new List<PropertyInfo>();
                foreach (var propertyInfo in t.GetProperties())
                    foreach (var attribute in propertyInfo.GetCustomAttributes())
                        if (attribute is PrivateAttribute)
                            privateProps.Add(propertyInfo);
                        else if (attribute is VisibleByAttribute visibleByAttribute)
                            if (!visibleByAttribute.IsVisibleBy(roleName))
                                hiddenProps.Add(propertyInfo);
                HiddenPropertiesDictionary.Add((t, roleName), hiddenProps);
                PrivatePropertiesDictionary.Add(t, privateProps);
            }

            if (hiddenInDictionary)
            {
                if (!privateInDictionary)
                    GetPrivateProperties(t);
            }
            else if (privateInDictionary)
                GetHiddenProperties(t, roleName);
            else
                GetProperties();
            HiddenPropertiesDictionary.TryGetValue((t, roleName), out hiddenProperties);
            PrivatePropertiesDictionary.TryGetValue(t, out privateProperties);
        }
    }
}
