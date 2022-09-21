using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        public List<KeyValuePair<Value, Value>> matches = new List<KeyValuePair<Value, Value>>();

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
                default: return new Coordinates { Row = newRow, Column = newCol, EndOfDirection = true };

            }
        }

        int iteration = 0;

        public string Solve()
        {
            //Console.WriteLine(JsonConvert.SerializeObject(maze));
            //matches.Add(new KeyValuePair<Value, Value>(maze[0, 4], maze[0, 3]));
            var solved = false;
            while (!solved)
            {
                iteration++;
                var currentanswerAmount = CountAnswers();
                var newanswerAmount = 0;
                var noNewNumbers = false;

                MatchLocations();
                Console.WriteLine($"Locations matched {iteration}: ");
                PrintMatches();
                
                //Console.WriteLine(currentanswerAmount);
                //while (!noNewNumbers)
                //{
                //    CheckToWalls();
                //    newanswerAmount = CountAnswers();
                    
                //    if (currentanswerAmount == newanswerAmount)
                //        noNewNumbers = true;
                //    else currentanswerAmount = newanswerAmount;
                //    //Console.WriteLine($"found answers: {newanswerAmount}");
                //}
                //Console.WriteLine($"Checked to walls {iteration}: ");
                //PrintMatches();

                //Console.WriteLine("found " + CountAnswers());

                noNewNumbers = false;
                currentanswerAmount = CountAnswers();

                while (!noNewNumbers)
                {
                    FindDistanceInBetweenNumbers();
                    newanswerAmount = CountAnswers();

                    if (currentanswerAmount == newanswerAmount)
                        noNewNumbers = true;
                    else currentanswerAmount = newanswerAmount;
                }
                Console.WriteLine($"Solved between numbers {iteration}: ");
                PrintMatches();

                ReverseFind();
                Console.WriteLine($"ReverseFind {iteration}: ");
                PrintMatches();


                if (CountAnswers() == 64)
                    solved = true;

                Console.WriteLine($"Answers: {CountAnswers()}");
                Console.WriteLine($"matches: {matches.Count()}");

                PrintUnmatchedAnswers();

                PrintMaze();
            };

            //Console.WriteLine(JsonConvert.SerializeObject(maze));
            Console.WriteLine("Complete Solve:");
            PrintMaze();
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
                            var amount = from Value value in maze.Cast<Value>()
                                         where value.Answer == maze[possiblePosition.Row, possiblePosition.Column].Answer
                                         select value;

                            if(amount.Count() == 0)
                            {
                                maze[possiblePosition.Row, possiblePosition.Column].Answer = maze[row, col].Answer++;
                                matches.Add(new KeyValuePair<Value, Value>(maze[row, col], maze[possiblePosition.Row, possiblePosition.Column]));
                            }
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
                //Console.WriteLine(maze[coordinates.Row, coordinates.Column].Answer + distances[i].Value);
                SequenceBetweenAnswers(new RecursiveSequence
                {
                    FinalAnswer = maze[coordinates.Row, coordinates.Column].Answer + distances[i].Value,
                    CurrentSequence = new List<Coordinates>(),
                    Distance = distances[i].Value - 1,
                    EndReached = false,
                    First = true,
                    Location = coordinates,
                    Possibilities = 0,
                    Possition = 0,
                    Sequence = new List<Coordinates>(),
                    Succes = false
                });
                //Console.WriteLine("out of recursion");
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
            //Console.WriteLine(recursiveSequence.Distance + ": " + recursiveSequence.Location.Column + ", " + recursiveSequence.Location.Row);
            var endOfDirection = false;
            var location = recursiveSequence.Location;

            while (!endOfDirection)
            {
                //foreach (var answer in recursiveSequence.CurrentSequence)
                //{
                //    Console.Write($"{answer.Row}, {answer.Column} - ");
                //}
                //Console.WriteLine("|");
                var cor = new Coordinates { Column = location.Column, Row = location.Row };
                location = FollowDirection(location, maze[recursiveSequence.Location.Row, recursiveSequence.Location.Column].Direction);

                if (location.EndOfDirection == true)
                    endOfDirection = true;

                if(matches.Any(x => x.Value.Col == location.Column && x.Value.Row == location.Row))
                {
                    var match = (matches.Where(x => x.Value.Col == location.Column && x.Value.Row == location.Row)).First();
                    if ((match.Value.Col == location.Column && match.Value.Row == location.Row) && (match.Key.Col != cor.Column || match.Key.Row != cor.Row))
                        continue;
                }

                if (recursiveSequence.Distance == 0 && maze[location.Row, location.Column].Answer == recursiveSequence.FinalAnswer)
                {
                    if (!recursiveSequence.PossibleLastLocations.Any(x => x.Column == recursiveSequence.Location.Column && x.Row == recursiveSequence.Location.Row))
                        recursiveSequence.PossibleLastLocations.Add(recursiveSequence.Location);

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

                //if (matches.Select(x => x.Value).Any(x => x.Col == location.Column && x.Row == location.Row) &&
                //    matches.Select(x => x.Key).Any(x => x.Col == recursiveSequence.Location.Column && x.Row == recursiveSequence.Location.Row))
                //    continue;

                if (maze[location.Row, location.Column].Answer != 0 || recursiveSequence.CurrentSequence.Any(x => x.Row == location.Row && x.Column == location.Column))
                    continue;

                if (recursiveSequence.Distance != 0)
                {
                    var recursion = recursiveSequence.DeepClone();
                    recursion.CurrentSequence.Add(location);
                    recursion.First = false;
                    recursion.Location = location;
                    recursion.Distance--;
                    recursion = SequenceBetweenAnswers(recursion);

                    recursiveSequence.Possibilities = recursion.Possibilities;
                    recursiveSequence.Sequence = recursion.Sequence;
                    recursiveSequence.Succes = recursion.Succes;
                    recursiveSequence.PossibleLastLocations = recursion.PossibleLastLocations;
                }

                if (endOfDirection)
                    continue;
            }


            if(recursiveSequence.Succes && recursiveSequence.Sequence.Count == recursiveSequence.Distance && recursiveSequence.Possibilities == 1 && recursiveSequence.First)
            {
                for (var i = 0; i < recursiveSequence.Distance; i++)
                {
                    maze[recursiveSequence.Sequence[i].Row, recursiveSequence.Sequence[i].Column].Answer = maze[recursiveSequence.Location.Row, recursiveSequence.Location.Column].Answer + i + 1;
                    if (i > 0)
                    {
                        if (!matches.Any(x => x.Key.Col == recursiveSequence.Sequence[i - 1].Column && x.Key.Row == recursiveSequence.Sequence[i - 1].Row))
                            matches.Add(new KeyValuePair<Value, Value>(maze[recursiveSequence.Sequence[i - 1].Row, recursiveSequence.Sequence[i - 1].Column], maze[recursiveSequence.Sequence[i].Row, recursiveSequence.Sequence[i].Column]));
                    }
                    else if (i == 0)
                    {
                        if (!matches.Any(x => x.Key.Col == recursiveSequence.Location.Column && x.Key.Row == recursiveSequence.Location.Row))
                            matches.Add(new KeyValuePair<Value, Value>(maze[recursiveSequence.Location.Row, recursiveSequence.Location.Column], maze[recursiveSequence.Sequence[i].Row, recursiveSequence.Sequence[i].Column]));
                    }
                }
                return null;
            }

            if (recursiveSequence.Succes && recursiveSequence.Sequence.Count == recursiveSequence.Distance && recursiveSequence.Possibilities > 1 && recursiveSequence.First && recursiveSequence.PossibleLastLocations.Count == 1)
            {
                if (!matches.Any(x => x.Key.Col == recursiveSequence.PossibleLastLocations[0].Column && x.Key.Row == recursiveSequence.PossibleLastLocations[0].Row))
                {
                    var endLocation = (from Value value in maze.Cast<Value>()
                                where value.Answer == recursiveSequence.FinalAnswer
                                select value).FirstOrDefault();

                    if(endLocation != null)
                    {
                        maze[recursiveSequence.PossibleLastLocations[0].Row, recursiveSequence.PossibleLastLocations[0].Column].Answer = recursiveSequence.FinalAnswer - 1;
                        matches.Add(new KeyValuePair<Value, Value>(maze[recursiveSequence.PossibleLastLocations[0].Row, recursiveSequence.PossibleLastLocations[0].Column], endLocation));
                    }
                }
            }

            return recursiveSequence;
        }

        void MatchLocations()
        {
            foreach(var location in maze)
            {
                if (matches.Any(x => x.Key.Col == location.Col && x.Key.Row == location.Row))
                    continue;

                if (location.Answer != 0)
                {
                    var next = (from Value value in maze.Cast<Value>()
                                where value.Answer == location.Answer + 1
                                select value).FirstOrDefault();

                    if(next != null)
                    {
                        matches.Add(new KeyValuePair<Value, Value>(location, next));
                        continue;
                    }

                }

                var previousLocation = new Coordinates { Column = location.Col, Row = location.Row };
                var sequence = new List<Coordinates>();
                var endReached = false;

                while (!endReached)
                {
                    var nextLocation = FollowDirection(previousLocation, location.Direction);
                    if (nextLocation.EndOfDirection)
                        endReached = true;

                    if (nextLocation.Row == previousLocation.Row && nextLocation.Column == previousLocation.Column)
                        continue;

                    if (maze[nextLocation.Row, nextLocation.Column].Answer == 1)
                        continue;

                    previousLocation.Row = nextLocation.Row;
                    previousLocation.Column = nextLocation.Column;

                    if (!matches.Any(x => x.Value.Col == nextLocation.Column && x.Value.Row == nextLocation.Row))
                        sequence.Add(nextLocation);
                }

                if (sequence.Count == 1)
                    matches.Add(new KeyValuePair<Value, Value>(location, maze[sequence[0].Row, sequence[0].Column]));

                //var endLocation = FollowDirection(new Coordinates { Column = location.Col, Row = location.Row }, location.Direction);
                //if (maze[endLocation.Row, endLocation.Column] == location)
                //    continue;

                //if (endLocation.EndOfDirection && !matches.Any(x => x.Key.Row == location.Row && x.Key.Col == location.Col))
                //{
                //    matches.Add(new KeyValuePair<Value, Value>(location, maze[endLocation.Row, endLocation.Column]));
                //}
            }

            foreach (var match in matches)
            {
                if (match.Key.Answer != 0)
                    match.Value.Answer = match.Key.Answer + 1;

                if (match.Value.Answer != 0)
                    match.Key.Answer = match.Value.Answer - 1;
            }
        }

        void ReverseFind()
        {
            foreach (var location in maze)
            {
                var possibleLocations = new List<Value>();
                if (/*location.Answer == 0 ||*/ location.Answer == 1)
                    continue;

                if (matches.Any(x => x.Value.Col == location.Col && x.Value.Row == location.Row))
                    continue;

                var previous = (from Value value in maze.Cast<Value>()
                             where value.Answer == location.Answer - 1
                             select value).FirstOrDefault();

                if (previous != null && previous.Answer != 0)
                {
                    matches.Add(new KeyValuePair<Value, Value>(previous, maze[location.Row, location.Col]));
                    continue;
                }

                var endOfDirection = false;

                var newLocation = new Coordinates { Column = location.Col, Row = location.Row };
                var previousLocation = new Coordinates { Column = location.Col, Row = location.Row };

                while (!endOfDirection)
                {
                    newLocation = FollowDirection(new Coordinates { Column = newLocation.Column, Row = newLocation.Row }, ArrowDirection.U);
                    if (newLocation.EndOfDirection)
                        endOfDirection = true;

                    if (maze[newLocation.Row, newLocation.Column].Direction != ArrowDirection.D)
                        continue;

                    if ((maze[newLocation.Row, newLocation.Column].Answer != 0 && maze[location.Row, location.Col].Answer != 0))
                        continue;

                    if (previousLocation.Row == newLocation.Row && previousLocation.Column == newLocation.Column)
                        continue;

                    if (!matches.Any(x => ((x.Key.Col == newLocation.Column && x.Key.Row == newLocation.Row))))
                        possibleLocations.Add(maze[newLocation.Row, newLocation.Column]);

                    previousLocation.Row = newLocation.Row;
                    previousLocation.Column = newLocation.Column;
                }

                newLocation = new Coordinates { Column = location.Col, Row = location.Row };
                previousLocation = new Coordinates { Column = location.Col, Row = location.Row };

                endOfDirection = false;
                while (!endOfDirection)
                {
                    newLocation = FollowDirection(new Coordinates { Column = newLocation.Column, Row = newLocation.Row }, ArrowDirection.D);
                    if (newLocation.EndOfDirection)
                        endOfDirection = true;

                    if (maze[newLocation.Row, newLocation.Column].Direction != ArrowDirection.U)
                        continue;

                    if ((maze[newLocation.Row, newLocation.Column].Answer != 0 && maze[location.Row, location.Col].Answer != 0) && (maze[location.Row, location.Col].Answer - maze[newLocation.Row, newLocation.Column].Answer != 1))
                        continue;

                    if (previousLocation.Row == newLocation.Row && previousLocation.Column == newLocation.Column)
                        continue;

                    if (!matches.Any(x => ((x.Key.Col == newLocation.Column && x.Key.Row == newLocation.Row))))
                        possibleLocations.Add(maze[newLocation.Row, newLocation.Column]);

                    previousLocation.Row = newLocation.Row;
                    previousLocation.Column = newLocation.Column;
                }

                newLocation = new Coordinates { Column = location.Col, Row = location.Row };
                previousLocation = new Coordinates { Column = location.Col, Row = location.Row };

                endOfDirection = false;
                while (!endOfDirection)
                {
                    newLocation = FollowDirection(new Coordinates { Column = newLocation.Column, Row = newLocation.Row }, ArrowDirection.L);
                    if (newLocation.EndOfDirection)
                        endOfDirection = true;

                    if (maze[newLocation.Row, newLocation.Column].Direction != ArrowDirection.R)
                        continue;

                    if ((maze[newLocation.Row, newLocation.Column].Answer != 0 && maze[location.Row, location.Col].Answer != 0) && (maze[location.Row, location.Col].Answer - maze[newLocation.Row, newLocation.Column].Answer != 1))
                        continue;

                    if (previousLocation.Row == newLocation.Row && previousLocation.Column == newLocation.Column)
                        continue;

                    if (!matches.Any(x => ((x.Key.Col == newLocation.Column && x.Key.Row == newLocation.Row))))
                        possibleLocations.Add(maze[newLocation.Row, newLocation.Column]);

                    previousLocation.Row = newLocation.Row;
                    previousLocation.Column = newLocation.Column;
                }

                newLocation = new Coordinates { Column = location.Col, Row = location.Row };
                previousLocation = new Coordinates { Column = location.Col, Row = location.Row };

                endOfDirection = false;
                while (!endOfDirection)
                {
                    newLocation = FollowDirection(new Coordinates { Column = newLocation.Column, Row = newLocation.Row }, ArrowDirection.R);
                    if (newLocation.EndOfDirection)
                        endOfDirection = true;

                    if (maze[newLocation.Row, newLocation.Column].Direction != ArrowDirection.L)
                        continue;

                    if ((maze[newLocation.Row, newLocation.Column].Answer != 0 && maze[location.Row, location.Col].Answer != 0) && (maze[location.Row, location.Col].Answer - maze[newLocation.Row, newLocation.Column].Answer != 1))
                        continue;

                    if (previousLocation.Row == newLocation.Row && previousLocation.Column == newLocation.Column)
                        continue;

                    if (!matches.Any(x => ((x.Key.Col == newLocation.Column && x.Key.Row == newLocation.Row))))
                        possibleLocations.Add(maze[newLocation.Row, newLocation.Column]);

                    previousLocation.Row = newLocation.Row;
                    previousLocation.Column = newLocation.Column;
                }

                newLocation = new Coordinates { Column = location.Col, Row = location.Row };
                previousLocation = new Coordinates { Column = location.Col, Row = location.Row };

                endOfDirection = false;
                while (!endOfDirection)
                {
                    newLocation = FollowDirection(new Coordinates { Column = newLocation.Column, Row = newLocation.Row }, ArrowDirection.UR);
                    if (newLocation.EndOfDirection)
                        endOfDirection = true;

                    if (maze[newLocation.Row, newLocation.Column].Direction != ArrowDirection.DL)
                        continue;

                    if ((maze[newLocation.Row, newLocation.Column].Answer != 0 && maze[location.Row, location.Col].Answer != 0) && (maze[location.Row, location.Col].Answer - maze[newLocation.Row, newLocation.Column].Answer != 1))
                        continue;

                    if (previousLocation.Row == newLocation.Row && previousLocation.Column == newLocation.Column)
                        continue;

                    if (!matches.Any(x => ((x.Key.Col == newLocation.Column && x.Key.Row == newLocation.Row))))
                        possibleLocations.Add(maze[newLocation.Row, newLocation.Column]);

                    previousLocation.Row = newLocation.Row;
                    previousLocation.Column = newLocation.Column;
                }

                newLocation = new Coordinates { Column = location.Col, Row = location.Row };
                previousLocation = new Coordinates { Column = location.Col, Row = location.Row };

                endOfDirection = false;
                while (!endOfDirection)
                {
                    newLocation = FollowDirection(new Coordinates { Column = newLocation.Column, Row = newLocation.Row }, ArrowDirection.UL);
                    if (newLocation.EndOfDirection)
                        endOfDirection = true;

                    if (maze[newLocation.Row, newLocation.Column].Direction != ArrowDirection.DR)
                        continue;

                    if ((maze[newLocation.Row, newLocation.Column].Answer != 0 && maze[location.Row, location.Col].Answer != 0) && (maze[location.Row, location.Col].Answer - maze[newLocation.Row, newLocation.Column].Answer != 1))
                        continue;

                    if (previousLocation.Row == newLocation.Row && previousLocation.Column == newLocation.Column)
                        continue;

                    if (!matches.Any(x => ((x.Key.Col == newLocation.Column && x.Key.Row == newLocation.Row))))
                        possibleLocations.Add(maze[newLocation.Row, newLocation.Column]);

                    previousLocation.Row = newLocation.Row;
                    previousLocation.Column = newLocation.Column;
                }

                newLocation = new Coordinates { Column = location.Col, Row = location.Row };
                previousLocation = new Coordinates { Column = location.Col, Row = location.Row };

                endOfDirection = false;
                while (!endOfDirection)
                {
                    newLocation = FollowDirection(new Coordinates { Column = newLocation.Column, Row = newLocation.Row }, ArrowDirection.DR);
                    if (newLocation.EndOfDirection)
                        endOfDirection = true;

                    if (maze[newLocation.Row, newLocation.Column].Direction != ArrowDirection.UL)
                        continue;

                    if ((maze[newLocation.Row, newLocation.Column].Answer != 0 && maze[location.Row, location.Col].Answer != 0) && (maze[location.Row, location.Col].Answer - maze[newLocation.Row, newLocation.Column].Answer != 1))
                        continue;

                    if (previousLocation.Row == newLocation.Row && previousLocation.Column == newLocation.Column)
                        continue;

                    if (!matches.Any(x => ((x.Key.Col == newLocation.Column && x.Key.Row == newLocation.Row))))
                        possibleLocations.Add(maze[newLocation.Row, newLocation.Column]);

                    previousLocation.Row = newLocation.Row;
                    previousLocation.Column = newLocation.Column;
                }

                newLocation = new Coordinates { Column = location.Col, Row = location.Row };
                previousLocation = new Coordinates { Column = location.Col, Row = location.Row };

                endOfDirection = false;
                while (!endOfDirection)
                {
                    newLocation = FollowDirection(new Coordinates { Column = newLocation.Column, Row = newLocation.Row }, ArrowDirection.DL);
                    if (newLocation.EndOfDirection)
                        endOfDirection = true;

                    if (maze[newLocation.Row, newLocation.Column].Direction != ArrowDirection.UR)
                        continue;

                    if ((maze[newLocation.Row, newLocation.Column].Answer != 0 && maze[location.Row, location.Col].Answer != 0) && (maze[location.Row, location.Col].Answer - maze[newLocation.Row, newLocation.Column].Answer != 1))
                        continue;

                    if (previousLocation.Row == newLocation.Row && previousLocation.Column == newLocation.Column)
                        continue;

                    if (!matches.Any(x => ((x.Key.Col == newLocation.Column && x.Key.Row == newLocation.Row))))
                        possibleLocations.Add(maze[newLocation.Row, newLocation.Column]);

                    previousLocation.Row = newLocation.Row;
                    previousLocation.Column = newLocation.Column;
                }

                if (possibleLocations.Count == 1)
                {
                    //if(maze[location.Row, location.Col].Answer != 0)
                    //    maze[possibleLocations[0].Row, possibleLocations[0].Col].Answer = location.Answer - 1;

                    matches.Add(new KeyValuePair<Value, Value>(maze[possibleLocations[0].Row, possibleLocations[0].Col], maze[location.Row, location.Col]));
                }
            }
        }

        void PrintMaze()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Console.Write($"\"{maze[row, col].Answer}{maze[row, col].Direction}\",   \t");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            PrintMultipleNumbers();
            Console.WriteLine();
            Console.WriteLine();

        }

        void PrintMultipleNumbers()
        {
            var multiples = 0;
            var duplicates = 0;
            for (int i = 1; i <= 64; i++)
            {
                var amount = from Value value in maze.Cast<Value>()
                             where value.Answer == i
                             select value;

                if (amount.Count() > 1)
                {
                    Console.WriteLine($"{i} = {amount.Count()}");
                    duplicates = duplicates + amount.Count();
                    multiples++;
                }
            }
            Console.WriteLine($"{multiples} multiples");
            Console.WriteLine($"{duplicates} duplicates");
        }

        void PrintMatches()
        {
            //matches.Sort((x, y) => (x.Key.Answer.CompareTo(y.Key.Answer)));
            Console.WriteLine();
            foreach (var pair in matches)
            {
                Console.WriteLine($"match 1: Key = {pair.Key.Row},{pair.Key.Col}: {pair.Key.Answer}, value = {pair.Value.Row},{pair.Value.Col}: {pair.Value.Answer}");
            }
        }

        void PrintUnmatchedAnswers()
        {
            foreach (var location in maze)
            {
                if (location.Answer == 0)
                    continue;
                if(!matches.Any(x => (x.Value.Answer == location.Answer) || (x.Key.Answer == location.Answer)))
                    Console.WriteLine($"{location.Row},{location.Col}: {location.Answer}");
            }
        }
    }
}
