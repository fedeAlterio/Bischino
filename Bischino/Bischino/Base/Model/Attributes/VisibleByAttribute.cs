using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Bischino.Base.Model.Attributes
{
    public class VisibleByAttribute : Attribute
    {
        private readonly Type[] _types;
        private IList<Type> _possibleObservers;

        private IList<Type> PossibleObservers
        {
            get
            {
                if (_possibleObservers != null)
                    return _possibleObservers;

                var totObservers = new List<Type>();
                foreach (var type in _types)
                    totObservers.AddRange(AttributesHelper.PossibleObserversByType(type));

                _possibleObservers = totObservers;
                return _possibleObservers;
            }
        }

        public VisibleByAttribute(params Type[] types)
        {
            _types = types;
        }

        public bool IsVisibleBy(string roleName)
        {
            var results =
                from observer in PossibleObservers
                where observer.Name.Equals(roleName)
                select observer;
            var ret = results.Any();
            return ret;
        }
    }
}
