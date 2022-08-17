using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow_Maze_Solver
{
    public  class RecursiveSequence
    {
        public Coordinates coordinates { get; set; }
        public List<Coordinates> sequence { get; set; }
        public int distance { get; set; }
        public int possibilities { get; set; }
        public int possition { get; set; }
        public bool endReached { get; set; }
        public bool succes { get; set; }
    }
}
