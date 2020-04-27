using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Model
{
    public class BlockingSet<T>
    {
        private readonly object Lock = new object();
        private HashSet<T> _set = new HashSet<T>();

        public void Add(T t)
        {
            lock (Lock)
            {
                _set.Add(t);
            }
        }

        public void Remove(T t)
        {
            lock(Lock)
            {
                _set.Remove(t);
            }
        }

        public IList<T> GetAll(Predicate<T> predicate)
        {
            lock (Lock)
            {
                var ret = from t in _set where predicate(t) select t;
                return ret.ToList();
            }
        }

        public T First()
        {
            lock (Lock)
            {
                var ret = _set.First();
                return ret;
            }
        }
    }
}
