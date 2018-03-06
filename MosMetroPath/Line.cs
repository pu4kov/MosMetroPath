using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MosMetroPath
{
    /// <summary>
    /// Ветка метро
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class Line
    {
        public int Id { get; }
        public string Name { get; set; }
        public Scheme Scheme { get; }

        internal Line(Scheme scheme, int id, string name)
        {
            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            Id = id;
            Name = name;
        }
        
        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is Line other)
                return other.Id == Id;

            return base.Equals(obj);
        }
    }
}
