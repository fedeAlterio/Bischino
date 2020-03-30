using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Bischino.Base.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RoleAttribute : Attribute
    {
        public string Role { get; }

        public RoleAttribute(Type type)
        {
            Role = GetRole(type);
        }

        public static string GetRole(Type type) => type.Name;
    }
}
