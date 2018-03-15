using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Collections;

namespace MosMetroPath
{
    public class AllLinesVisitor
    {
        const int AddBuilderMaxDegreeOfParallelism = 4;
        const int AddBuilderBoundedCapacity = 10000;
        const int SearchRouteMaxDegreeOfParallelism = 4;
        const int SearchRouteBoundedCapacity = 1000;

        public Task Completion { get; private set; }
        private TransformBlock<CreateBlockParam, SearchBlockParam> _addBuilder;
        private ActionBlock<SearchBlockParam> _searchRoute;
        private ResultCollection Results = new ResultCollection();
        public int CurrentTimespan => Results.Timespan;

        public AllLinesVisitor()
        {
            _addBuilder = new TransformBlock<CreateBlockParam, SearchBlockParam>(
                p => new SearchBlockParam
                {
                    Builder = new RouteBuilder(p.Routes),
                    Results = p.Results
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = AddBuilderMaxDegreeOfParallelism, BoundedCapacity = AddBuilderBoundedCapacity });

            _searchRoute = new ActionBlock<SearchBlockParam>(
                (p) =>
                {
                    bool next = true;
                    do
                    {
                        if (p.Builder.NextTurn())
                        {
                            if (!p.Results.CanAdded(p.Builder.MinTimespan))
                            {
                                next = false;
                            }
                        }
                        else
                        {
                            next = false;
                            if (p.Builder.HasResult)
                            {
                                p.Results.Add(p.Builder.GetRoute());
                            }
                        }
                    }
                    while (next);
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = SearchRouteMaxDegreeOfParallelism, BoundedCapacity = SearchRouteBoundedCapacity });

            _addBuilder.LinkTo(_searchRoute);
        }

        public IEnumerable<IRoute> GetResults()
        {
            return Results;
        }

        public void Push(IEnumerable<IRoute> routes)
        {
            _addBuilder.Post(new CreateBlockParam { Routes = routes, Results = Results });
        }

        public void Complete()
        {
            _addBuilder.Complete();

            Completion = CompletionReceive();
        }

        private async Task CompletionReceive()
        {
            await _addBuilder.Completion;
            _searchRoute.Complete();
            await _searchRoute.Completion;
        }
        
        public struct CreateBlockParam
        {
            public IEnumerable<IRoute> Routes;
            public ResultCollection Results;
        }

        public struct SearchBlockParam
        {
            public RouteBuilder Builder;
            public ResultCollection Results;
        }

        public class ResultCollection: IEnumerable<IRoute>
        {
            private List<IRoute> _items = new List<IRoute>();
            private int _timespan = int.MaxValue;
            public int Timespan
            {
                get => _timespan;
                set
                {
                    Interlocked.Exchange(ref _timespan, value);
                }
            }

            private object _lockObj = new object();

            public bool CanAdded(int minTimespan)
            {
                return Timespan >= minTimespan;
            }

            public void Add(IRoute route)
            {
                if (Timespan >= route.Length)
                {
                    lock (_lockObj)
                    {
                        if (Timespan > route.Length)
                        {
                            _items.Clear();
                            _items.Add(route);
                            Timespan = route.Timespan;
                        }
                        else if (Timespan == route.Length)
                        {
                            _items.Add(route);
                        }
                    }
                }
            }

            public IEnumerator<IRoute> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
