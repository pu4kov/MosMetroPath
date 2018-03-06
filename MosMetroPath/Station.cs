using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MosMetroPath
{
    /// <summary>
    /// Станция
    /// </summary>
    [DebuggerDisplay("{Name} ({Line.Name})")]
    public class Station: IId
    {
        public int Id { get; }
        public string Name { get; set; }
        public Line Line { get; }

        internal Station(int id, string name, Line line)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Id = id;
            Name = name;
            Line = line ?? throw new ArgumentNullException(nameof(line));
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is Station other)
                return other.Id == Id;

            return base.Equals(obj);
        }
    }
}
