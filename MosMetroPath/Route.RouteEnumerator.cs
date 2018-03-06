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
        private class RouteEnumerator : IEnumerator<IRoute>
        {
            private Route _route;
            private RouteNode _current;

            public bool IsReverse { get; }
            public IRoute Current => _current.Route;

            object IEnumerator.Current => Current;

            public RouteEnumerator(Route route, bool reverse)
            {
                _route = route;
                IsReverse = reverse;

                Reset();
            }

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                var next = (IsReverse)
                            ? _current.Prior
                            : _current.Next;

                if (next != null)
                {
                    _current = next;

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                var start = new RouteNode
                {
                    Route = null,
                    IsReversed = false
                };

                if (!IsReverse)
                {
                    start.Next = _route.First;
                    _current = start;
                }
                else
                {
                    start.Prior = _route.Last;
                    _current = start;
                }
            }
        }
    }
}
