using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow_Maze_Solver;
using Newtonsoft.Json;
using static Arrow_Maze_Solver.Value;

namespace Arrow_Maze_Solver
{
    public class Solver
    {
        //public int hits = 1;
        public Value[,] maze = new Value[8, 8];

        public Coordinates FollowDirection(Coordinates coordinates, ArrowDirection direction)
        {
            var newRow = coordinates.Row;
            var newCol = coordinates.Column;
            //Console.WriteLine($"FollowDirection is hit: {hits} times");
            //hits++;
            switch (direction)
            {
                case ArrowDirection.D:
                    if (coordinates.Row < 7)
                        newRow = coordinates.Row + 1;
                    return new Coordinates { Row = newRow, Column = newCol};
                case ArrowDirection.U:
                    if (coordinates.Row > 0)
                        newRow = coordinates.Row - 1;
                    return new Coordinates { Row = newRow, Column = newCol };
                case ArrowDirection.L:
                    if (coordinates.Column > 0)
                        newCol = coordinates.Column - 1;
                    return new Coordinates { Row = newRow, Column = newCol };
                case ArrowDirection.R:
                    if (coordinates.Column < 7)
                        newCol = coordinates.Column + 1;
                    return new Coordinates { Row = newRow, Column = newCol };
                case ArrowDirection.DL:
                    if (coordinates.Row < 7 && coordinates.Column > 0)
                    {
                        newRow = coordinates.Row + 1;
                        newCol = coordinates.Column - 1;
                    }
                    return new Coordinates { Row = newRow, Column = newCol };
                case ArrowDirection.DR:
                    if (coordinates.Row < 7 && coordinates.Column < 7)
                    {
                        newRow = coordinates.Row + 1;
                        newCol = coordinates.Column + 1;
                    }
                    return new Coordinates { Row = newRow, Column = newCol };
                case ArrowDirection.UL:
                    if (coordinates.Row > 0 && coordinates.Column > 0)
                    {
                        newRow = coordinates.Row - 1;
                        newCol = coordinates.Column - 1;
                    }
                    return new Coordinates { Row = newRow, Column = newCol };
                case ArrowDirection.UR:
                    if (coordinates.Row > 0 && coordinates.Column < 7)
                    {
                        newRow = coordinates.Row - 1;
                        newCol = coordinates.Column + 1;
                    }
                    return new Coordinates { Row = newRow, Column = newCol };
                case ArrowDirection.Empty:
                    return new Coordinates { Row = newRow, Column = newCol };
                default: return new Coordinates();
            }
        }

        public string Solve()
        {
            //Console.WriteLine(JsonConvert.SerializeObject(maze));

            var solved = false;
            while (!solved)
            {
                var currentanswerAmount = CountAnswers();
                var newanswerAmount = 0;
                var noNewNumbers = false;
                
                //Console.WriteLine(currentanswerAmount);
                while (!noNewNumbers)
                {
                    CheckToWalls();
                    newanswerAmount = CountAnswers();
                    
                    if (currentanswerAmount == newanswerAmount)
                        noNewNumbers = true;
                    else currentanswerAmount = newanswerAmount;
                    //Console.WriteLine($"found answers: {newanswerAmount}");
                }


                Console.WriteLine("found " + CountAnswers());

                noNewNumbers = false;
                currentanswerAmount = CountAnswers();

                while (!noNewNumbers)
                {
                    FindDistanceInBetweenNumbers();
                    newanswerAmount = CountAnswers();

                    if (currentanswerAmount == newanswerAmount)
                        noNewNumbers = true;
                    else currentanswerAmount = newanswerAmount;
                    //Console.WriteLine($"found answers: {newanswerAmount}");
                }
                solved = false;
            };

            //Console.WriteLine(JsonConvert.SerializeObject(maze));
            return null;
        }

        int CountAnswers()
        {
            var answerAmount = 0;

            foreach (var value in maze)
            {
                if (value.Answer != 0)
                    answerAmount++;
            }

            return answerAmount;
        }

        void CheckToWalls()
        {
            for(int row = 0; row < 8; row++)
            {
                for(int col = 0; col < 8; col++)
                {
                    if (maze[row, col].Answer != 0)
                    {
                        var possibilities = 0;
                        var samePosition = false;
                        var possiblePosition = new Coordinates();
                        var oldPosition = new Coordinates
                        {
                            Row = maze[row, col].Row,
                            Column = maze[row, col].Col
                        };

                        while (!samePosition)
                        {
                            var coordinates = FollowDirection(oldPosition, maze[row, col].Direction);
                            var newPosition = new Coordinates
                            {
                                Row = coordinates.Row,
                                Column = coordinates.Column
                            };
                            if (newPosition.Column == oldPosition.Column && newPosition.Row == oldPosition.Row)
                                samePosition = true;
                            else
                            {
                                if (maze[newPosition.Row, newPosition.Column].Answer == 0)
                                {
                                    possiblePosition = newPosition;
                                    possibilities++;
                                }
                            }
                            oldPosition = newPosition;
                        }

                        if (possibilities == 1)
                        {
                            maze[possiblePosition.Row, possiblePosition.Column].Answer = maze[row, col].Answer++;
                            //Console.WriteLine($"newfound answer: {maze[row, col].Answer++}");
                        }
                    }
                }
            }
        }

        void FindDistanceInBetweenNumbers()
        {
            var answers = new List<int>();
            var distances = new List<KeyValuePair<int, int>>();

            foreach (var value in maze)
            {
                if (value.Answer != 0)
                {
                    answers.Add(value.Answer);
                }
            }
            answers.Sort();

            for (int i = 0; i < answers.Count() - 1; i++)
            {
                distances.Add(new KeyValuePair<int, int>(answers[i], (answers[i + 1] - answers[i])));
            }

            distances = distances.OrderBy(x => x.Value).ToList();
            
            for (int i = 0; i < distances.Count(); i++)
            {
                if (distances[i].Value < 2)
                    continue;

                var coordinates = new Coordinates();
                foreach (var location in maze)
                {
                    if (location.Answer == distances[i].Key)
                    {
                        coordinates.Column = location.Col;
                        coordinates.Row = location.Row;
                    }
                }
                FindPossibleSequences(coordinates, distances[i].Value);
            }
        }

        void FindPossibleSequences(Coordinates coordinates, int distance)
        {
            var possibleRoutes = 0;
            var direction = maze[coordinates.Row, coordinates.Column].Direction;
            var foundLocation = new Coordinates();
            var intermediateCoordinates = coordinates;
            var endOfDirection = false;
            var previousCoordinates = coordinates;
            while (!endOfDirection)
            {
                intermediateCoordinates = FollowDirection(intermediateCoordinates, direction);
                if (previousCoordinates.Row == intermediateCoordinates.Row && previousCoordinates.Column == intermediateCoordinates.Column)
                {
                    endOfDirection = true;
                    break;
                }
                previousCoordinates = intermediateCoordinates;

                var forkDirection = maze[intermediateCoordinates.Row, intermediateCoordinates.Column].Direction;
                var endOfForkDirectionOrMatch = false;
                var forkCoordinates = intermediateCoordinates;
                var endLocation = new Coordinates();

                while (!endOfForkDirectionOrMatch)
                {
                    endLocation = FollowDirection(forkCoordinates, forkDirection);
                    if ((endLocation.Row == forkCoordinates.Row && endLocation.Column == forkCoordinates.Column))
                    {
                        endOfForkDirectionOrMatch = true;
                    }                    
                    else if (maze[endLocation.Row, endLocation.Column].Answer == maze[coordinates.Row, coordinates.Column].Answer + 2)
                    {
                        endOfForkDirectionOrMatch = true;
                        foundLocation = intermediateCoordinates;
                        possibleRoutes++;
                    }
                    else
                        forkCoordinates = endLocation;
                }
            }
            if (possibleRoutes == 1)
                maze[foundLocation.Row, foundLocation.Column].Answer = maze[coordinates.Row, coordinates.Column].Answer + 1;
        }

        void SequenceBetweenAnswers(Coordinates coordinates, List<Coordinates> sequence, int distacne, int possibilities, int possition, bool endReached, bool succes)
        {

        }
    }
}
