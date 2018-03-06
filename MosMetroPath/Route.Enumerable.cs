using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosMetroPath
{
    public partial class Route
    {
        private class Enumerable<T> : IEnumerable<T>
        {
            private Func<IEnumerator<T>> _factory;

            public Enumerable(Func<IEnumerator<T>> factory)
            {
                _factory = factory;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _factory();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _factory();
            }
        }
    }
}
