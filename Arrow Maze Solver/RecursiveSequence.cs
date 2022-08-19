using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow_Maze_Solver
{
    public  class RecursiveSequence
    {
        public Coordinates Location { get; set; }
        public List<Coordinates> Sequence { get; set; }
        public List<Coordinates> CurrentSequence { get; set; } = new List<Coordinates>();
        public int FinalAnswer { get; set; }
        public int Distance { get; set; }
        public int Possibilities { get; set; }
        public int Possition { get; set; }
        public bool EndReached { get; set; }
        public bool Succes { get; set; }
        public bool First { get; set; }
    }
}
