﻿using System;
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
        public int hits = 1;
        public Value[,] maze = new Value[8, 8];

        public Coordinates FollowDirection(Coordinates coordinates, ArrowDirection direction)
        {
            var newRow = coordinates.Row;
            var newCol = coordinates.Column;
            var endOfDirection = false;
            //Console.WriteLine($"FollowDirection is hit: {hits} times");
            //hits++;
            switch (direction)
            {
                case ArrowDirection.D:
                    if (coordinates.Row < 7)
                    {
                        newRow = coordinates.Row + 1;
                    }
                    if (newRow == 7)
                        endOfDirection = true;

                    return new Coordinates { Row = newRow, Column = newCol, EndOfDirection = endOfDirection};
                case ArrowDirection.U:
                    if (coordinates.Row > 0)
                    {
                        newRow = coordinates.Row - 1;
                    }
                    if (newRow == 0)
                        endOfDirection = true;

                    return new Coordinates { Row = newRow, Column = newCol, EndOfDirection = endOfDirection };
                case ArrowDirection.L:
                    if (coordinates.Column > 0)
                    {
                        newCol = coordinates.Column - 1;
                    }
                    if (newCol == 0)
                        endOfDirection = true;

                    return new Coordinates { Row = newRow, Column = newCol, EndOfDirection = endOfDirection };
                case ArrowDirection.R:
                    if (coordinates.Column < 7)
                    {
                        newCol = coordinates.Column + 1;
                    }
                    if (newCol == 7)
                        endOfDirection = true;

                    return new Coordinates { Row = newRow, Column = newCol, EndOfDirection = endOfDirection };
                case ArrowDirection.DL:
                    if (coordinates.Row < 7 && coordinates.Column > 0)
                    {
                        newRow = coordinates.Row + 1;
                        newCol = coordinates.Column - 1;
                    }
                    if (newRow == 7 || newCol == 0)
                        endOfDirection = true;

                    return new Coordinates { Row = newRow, Column = newCol, EndOfDirection = endOfDirection };
                case ArrowDirection.DR:
                    if (coordinates.Row < 7 && coordinates.Column < 7)
                    {
                        newRow = coordinates.Row + 1;
                        newCol = coordinates.Column + 1;
                    }
                    if (newRow == 7 || newCol == 7)
                        endOfDirection = true;

                    return new Coordinates { Row = newRow, Column = newCol, EndOfDirection = endOfDirection };
                case ArrowDirection.UL:
                    if (coordinates.Row > 0 && coordinates.Column > 0)
                    {
                        newRow = coordinates.Row - 1;
                        newCol = coordinates.Column - 1;
                    }
                    if (newRow == 0 || newCol == 0)
                        endOfDirection = true;

                    return new Coordinates { Row = newRow, Column = newCol, EndOfDirection = endOfDirection };
                case ArrowDirection.UR:
                    if (coordinates.Row > 0 && coordinates.Column < 7)
                    {
                        newRow = coordinates.Row - 1;
                        newCol = coordinates.Column + 1;
                    }
                    if (newRow == 0 || newCol == 7)
                        endOfDirection = true;

                    return new Coordinates { Row = newRow, Column = newCol, EndOfDirection = endOfDirection };
                case ArrowDirection.Empty:
                    return new Coordinates { Row = newRow, Column = newCol, EndOfDirection = true };
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
                //FindPossibleSequences(coordinates, distances[i].Value);
                SequenceBetweenAnswers(new RecursiveSequence
                {
                    FinalAnswer = maze[coordinates.Row, coordinates.Column].Answer + distances[i].Value,
                    CurrentSequence = new List<Coordinates>(),
                    Distance = distances[i].Value,
                    EndReached = false,
                    First = true,
                    Location = coordinates,
                    Possibilities = 0,
                    Possition = 0,
                    Sequence = new List<Coordinates>(),
                    Succes = false
                });
                Console.WriteLine("out of recursion");
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

        RecursiveSequence SequenceBetweenAnswers(RecursiveSequence recursiveSequence)
        {
            var endOfDirection = false;
            var location = recursiveSequence.Location;
            if (location.EndOfDirection == true)
                return recursiveSequence;

            while (!endOfDirection)
            {
                //foreach (var answer in recursiveSequence.CurrentSequence)
                //{
                //    Console.Write($"{answer.Row}, {answer.Column} - ");
                //}
                //Console.WriteLine("|");
                location = FollowDirection(location, maze[recursiveSequence.Location.Row, recursiveSequence.Location.Column].Direction);

                if (location.EndOfDirection == true)
                    endOfDirection = true;

                if (maze[location.Row, location.Column].Answer != 0 || recursiveSequence.CurrentSequence.Any(x => x.Row == location.Row && x.Column == location.Column))
                    continue;

                if (recursiveSequence.Distance == 0 && maze[recursiveSequence.Location.Row, recursiveSequence.Location.Column].Answer == recursiveSequence.FinalAnswer)
                {
                    recursiveSequence.Succes = true;
                    recursiveSequence.Possibilities++;
                    recursiveSequence.Sequence = recursiveSequence.CurrentSequence;
                    //if (recursiveSequence.Sequence.Any())
                    //    recursiveSequence.Sequence.RemoveAt(recursiveSequence.Sequence.Count - 1);

                    return recursiveSequence;
                }

                if (recursiveSequence.Distance == 0 && maze[recursiveSequence.Location.Row, recursiveSequence.Location.Column].Answer != recursiveSequence.FinalAnswer && location.EndOfDirection && !recursiveSequence.First)
                {
                    return recursiveSequence;
                }

                var recursion = recursiveSequence;
                recursion.CurrentSequence.Add(location);
                recursion.First = false;
                recursion.Location = location;
                recursion.Distance--;
                recursion = SequenceBetweenAnswers(recursion);

                recursiveSequence.Possibilities = recursion.Possibilities;
                recursiveSequence.Sequence = recursion.Sequence;
                recursiveSequence.Succes = recursion.Succes;

                if (endOfDirection)
                    return recursiveSequence;
            }


            if(recursiveSequence.Succes && recursiveSequence.Sequence.Count == recursiveSequence.Distance - 1 && recursiveSequence.Possibilities == 1 && recursiveSequence.First)
            {
                for (var i = 0; i < recursiveSequence.Distance; i++)
                {
                    maze[recursiveSequence.Sequence[i].Row, recursiveSequence.Sequence[i].Column].Answer = maze[recursiveSequence.Location.Row, recursiveSequence.Location.Column].Answer + i + 1;
                }
                return null; ;
            }

            return recursiveSequence;
        }
    }
}
