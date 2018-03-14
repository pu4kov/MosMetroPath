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
        private class Enumerator<TItem> : IEnumerator<TItem>
        {
            private RouteNode _start;
            protected RouteNode CurrentNode { get; private set; }
            protected IEnumerator<TItem> CurrentEnumerator { get; private set; }
            private Func<RouteNode, IEnumerator<TItem>> _func;

            public bool IsReverse { get; }
            public TItem Current => CurrentEnumerator.Current;

            object IEnumerator.Current => Current;

            public Enumerator(RouteNode start, bool reverse, Func<RouteNode, IEnumerator<TItem>> func)
            {
                _start = start;
                IsReverse = reverse;
                _func = func;

                Reset();
            }

            public void Dispose()
            {
                
            }

            private bool MoveNextNode()
            {
                var nextNode = (IsReverse) ? CurrentNode.Prior : CurrentNode.Next;
                if (nextNode == null)
                    return false;

                CurrentNode = nextNode;

                return true;
            }

            protected virtual void OnMoveNextNode()
            {
                CurrentEnumerator = _func(CurrentNode);
            }

            public bool MoveNext()
            {
                while (!CurrentEnumerator.MoveNext())
                {
                    if (!MoveNextNode())
                        return false;
                    OnMoveNextNode();
                }

                return true;
            }

            public void Reset()
            {
                CurrentNode = _start;
                CurrentEnumerator = _func(CurrentNode);
            }
        }
    }
}
