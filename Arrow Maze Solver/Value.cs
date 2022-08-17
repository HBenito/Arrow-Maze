using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow_Maze_Solver
{
    public class Value
    {
        public int Col { get; set; }
        public int Row { get; set; }
        public int Answer { get; set; }
        public enum ArrowDirection { U, D, L, R, UL, UR, DL, DR, Empty}
        public ArrowDirection Direction { get; set; }
    }
}
