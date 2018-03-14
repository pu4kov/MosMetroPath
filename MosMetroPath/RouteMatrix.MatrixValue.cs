using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MosMetroPath
{
    public partial class RouteMatrix
    {
        [DebuggerDisplay("{Value}")]
        private struct MatrixValue
        {
            public IRoute Route { get; }
            private int _value;
            public int Value
            {
                get
                {
                    if (IsInfinity)
                        return int.MaxValue;
                    return _value;
                }
                set
                {
                    if (IsInfinity)
                        throw new Exception();
                    _value = value;
                }
            }
            public bool IsInfinity { get; set; }

            public void SetInfinity() { IsInfinity = true; }

            public MatrixValue(IRoute route)
            {
                Route = route;
                _value = route?.Timespan ?? 0;
                IsInfinity = false;
            }

            public MatrixValue(MatrixValue v)
            {
                Route = v.Route;
                _value = v._value;
                IsInfinity = v.IsInfinity;
            }

            public void Subtract(MatrixValue value)
            {
                if (!IsInfinity)
                {
                    if (!value.IsInfinity)
                        Value -= value.Value;
                }
            }

            public static MatrixValue Min(MatrixValue v1, MatrixValue v2)
            {
                if (v1.IsInfinity)
                    return v2;
                if (v2.IsInfinity)
                    return v1;
                return (v1.Value < v2.Value) ? v1 : v2;
            }

            static public MatrixValue Infinity { get; } = new MatrixValue { IsInfinity = true };
        }

        public struct MatrixCoord
        {
            public int Col;
            public int Row;

            public MatrixCoord(int col, int row)
            {
                Col = col;
                Row = row;
            }
        }
    }
}
