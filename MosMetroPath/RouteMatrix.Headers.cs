using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosMetroPath
{
    public partial class RouteMatrix
    {
        /// <summary>
        /// Заголовки колонок и столбцов матрицы поиска
        /// </summary>
        private class MatrixHeaders
        {
            private List<Line> Columns { get; }
            private List<Line> Rows { get; }

            public int ColumnsCount => Columns.Count;
            public int RowsCount => Rows.Count;

            public IEnumerable<Line> GetLines()
            {
                return Columns.Union(Rows);
            }

            public MatrixHeaders(IEnumerable<IRoute> routes)
            {
                Columns = new List<Line>();
                Rows = new List<Line>();
                var lines = new HashSet<Line>();
                foreach (var r in routes)
                {
                    if (lines.Add(r.From.Line))
                        AddLine(r.From.Line);
                    if (lines.Add(r.To.Line))
                        AddLine(r.To.Line);
                }
            }

            private MatrixHeaders(MatrixHeaders other)
            {
                Columns = new List<Line>(other.Columns);
                Rows = new List<Line>(other.Rows);
            }

            private void AddLine(Line line)
            {
                Columns.Add(line);
                Rows.Add(line);
            }

            /// <summary>
            /// Получить номера столбца и строки маршрута между двумя ветками
            /// </summary>
            /// <param name="colLine">Ветка метро, находящаяся в колонке</param>
            /// <param name="rowLine">Ветка метро, находящаяся в строке</param>
            /// <returns>Номера колонки и строки соответствующей ячейки матрицы</returns>
            public MatrixCoord GetCoord(Line colLine, Line rowLine)
            {
                return
                    new MatrixCoord(GetColByLine(colLine), GetRowByLine(rowLine));
            }

            public int GetColByLine(Line line)
            {
                for (int col = 0; col < Columns.Count; ++col)
                {
                    if (Columns[col] == line)
                    {
                        return col;
                    }
                }

                return -1;
            }
            
            public int GetRowByLine(Line line)
            {
                for (int row = 0; row < Rows.Count; ++row)
                {
                    if (Rows[row] == line)
                    {
                        return row;
                    }
                }

                return -1;
            }

            public Line GetLineByCol(int col)
            {
                return Columns[col];
            }

            public Line GetLineByRow(int row)
            {
                return Rows[row];
            }

            public bool TryGetCoord(Line colLine, Line rowLine, out MatrixCoord coord)
            {
                try
                {
                    coord = GetCoord(colLine, rowLine);
                }
                catch
                {
                    coord = default(MatrixCoord);
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Создать копию объекта, удалив колонку и строку
            /// </summary>
            /// <param name="col">Индекс колонки</param>
            /// <param name="row">Индекс строки</param>
            /// <returns>Новый объект заголовка</returns>
            public MatrixHeaders CopyWithoutColRow(int col, int row)
            {
                var result = new MatrixHeaders(this);

                result.Columns.RemoveAt(col);
                result.Rows.RemoveAt(row);

                return result;
            }
        }
    }
}
