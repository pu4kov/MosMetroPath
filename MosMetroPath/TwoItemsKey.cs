using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MosMetroPath
{
    internal class TwoItemsKey<T>
        where T: class, IId
    {
        public T Item1 { get; }
        public T Item2 { get; }

        public TwoItemsKey(T v1, T v2)
        {
            Item1 = v1;
            Item2 = v2;
        }

        public bool Equals(TwoItemsKey<T> other)
        {
            return
                ((Item1 == other.Item1 && Item2 == other.Item2)
                || (Item2 == other.Item1 && Item1 == other.Item2));
        }

        public override int GetHashCode()
        {
            return Item1.Id + Item2.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is TwoItemsKey<T> other)
                return Equals(other);

            return false;
        }
    }
}
