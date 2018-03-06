using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosMetroPath
{
    /// <summary>
    /// Схема метро
    /// </summary>
    public class Scheme
    {
        /// <summary>
        /// Ветки метро
        /// </summary>
        private ISet<Line> Lines { get; } = new HashSet<Line>();
        /// <summary>
        /// Станции
        /// </summary>
        private ISet<Station> Stations { get; } = new HashSet<Station>();
        /// <summary>
        /// Переходы между ветками
        /// </summary>
        private IDictionary<Line, ICollection<Station>> LineRelationStations { get; } = new Dictionary<Line, ICollection<Station>>();
        /// <summary>
        /// Связи станции
        /// </summary>
        private IDictionary<Station, ICollection<StationRelation>> StationRelations { get; } = new Dictionary<Station, ICollection<StationRelation>>();

        public IEnumerable<Line> GetLines()
        {
            return Lines;
        }

        public IEnumerable<Line> GetLinesExclude(IEnumerable<Line> excluded)
        {
            var result = new HashSet<Line>(Lines);
            result.ExceptWith(excluded);
            return result;
            //return Lines.ExceptWith.Except(excluded).ToArray();
        }

        public IEnumerable<Station> GetStations()
        {
            return Stations;
        }

        public Line AddLine(string name)
        {
            var result = new Line(this, Lines.Count, name);
            Lines.Add(result);
            return result;
        }

        private void AddStationRelation(Station station, StationRelation relation)
        {
            ICollection<StationRelation> r;
            if (!StationRelations.TryGetValue(station, out r))
            {
                r = new List<StationRelation>();
                StationRelations.Add(station, r);
            }
            r.Add(relation);
        }

        private void AddLineRelation(StationRelation relation)
        {
            if (relation.From.Line == relation.To.Line)
                throw new ArgumentException();

            ICollection<Station> r1;
            if (!LineRelationStations.TryGetValue(relation.From.Line, out r1))
            {
                r1 = new List<Station>();
                LineRelationStations.Add(relation.From.Line, r1);
            }
            r1.Add(relation.From);

            ICollection<Station> r2;
            if (!LineRelationStations.TryGetValue(relation.To.Line, out r2))
            {
                r2 = new List<Station>();
                LineRelationStations.Add(relation.To.Line, r2);
            }
            r2.Add(relation.To);
        }

        public StationRelation AddRelation(Station from, Station to, int timespan)
        {
            if (from == to)
            {
                throw new ArgumentException($"Argument \"{nameof(from)}\" is equals to argument \"{nameof(to)}\"");
            }
            if (from.Line.Scheme != this ||
                to.Line.Scheme != this)
            {
                throw new ArgumentException();
            }

            var result = new StationRelation(from, to, timespan);

            // Добавляется пересадка между линиями метро
            if (from.Line != to.Line)
            {
                AddLineRelation(result);
            }

            // Все связи считаются двусторонними, добавляются дважды в коллекцию StationRelations
            AddStationRelation(from, result);
            AddStationRelation(to, result);

            return result;
        }

        public Station AddStation(Line line, string name)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var result = new Station(Stations.Count, name, line);
            Stations.Add(result);

            return result;
        }

        public IEnumerable<StationRelation> GetStationRelations(Station station)
        {
            if (StationRelations.TryGetValue(station, out var result))
            {
                return result;
            }
            
            throw new KeyNotFoundException();
        }

        public IEnumerable<Station> GetAllLineRelationStations()
        {
            return LineRelationStations.Values.SelectMany(v => v).Distinct();
        }

        public IEnumerable<Station> GetLinesRelationStations(ISet<Line> lines)
        {
            var result = LineRelationStations.Where(i => lines.Contains(i.Key)).SelectMany(i => i.Value).Distinct();

            return result;
        }

        public IEnumerable<Station> GetLineRelationStations(Line line)
        {
            if (LineRelationStations.TryGetValue(line, out var result))
            {
                return result;
            }

            throw new KeyNotFoundException();
        }
    }
}
