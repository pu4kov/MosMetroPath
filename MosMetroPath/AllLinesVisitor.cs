using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MosMetroPath
{
    public class AllLinesVisitor
    {
        public LinkedList<RouteBuilder> Builders { get; } = new LinkedList<RouteBuilder>();
        public LinkedList<IRoute> Results { get; } = new LinkedList<IRoute>();
        private object lockObj = new object();

        private void AddResult(RouteBuilder r)
        {
            if (!r.IsComplete)
                throw new ArgumentException();
            if (r.RoutesCount == 0)
                return;

            if (Results.First != null)
            {
                var timespan = r.RoutesTimespan;
                if (Results.First.Value.Timespan > timespan)
                {
                    Results.Clear();
                }
                else if (Results.First.Value.Timespan < timespan)
                {
                    return;
                }
            }

            foreach (var rr in r.GetRoutes())
                Results.AddLast(rr);
        }

        public void Push(IEnumerable<IRoute> routes)
        {
            Push(new RouteBuilder(routes));
        }

        private void Push(RouteBuilder newBuilder)
        {
            lock (lockObj)
            {
                var node = Builders.First;
                if (node != null)
                {
                    if (node.Value.MinLength == newBuilder.MinLength)
                    {
                        Builders.AddLast(newBuilder);
                    }
                    else if (node.Value.MinLength > newBuilder.MinLength)
                    {
                        Builders.Clear();
                        Builders.AddFirst(newBuilder);
                    }
                }
                else
                {
                    Builders.AddFirst(newBuilder);
                }
            }
        }

        private RouteBuilder Pop()
        {
            if (Interlocked.Equals(Builders.First, null))
                return null;
            lock (lockObj)
            {
                var result = Builders.First.Value;
                Builders.RemoveFirst();
                return result;
            }
        }

        public void Run()
        {
            var r = Pop();
            while (r != null)
            {
                if (r.NextTurn())
                {
                    Push(r);
                }
                else
                {
                    AddResult(r);
                }
                r = Pop();
            }
        }
    }
}
