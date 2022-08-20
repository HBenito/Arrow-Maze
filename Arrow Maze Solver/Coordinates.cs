using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow_Maze_Solver
{
    [Serializable]
    public class Coordinates
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool EndOfDirection { get; set; }
    }
}
