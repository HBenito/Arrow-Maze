using Arrow_Maze_Solver;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

Foo:

var _solver = new Solver();


var validInput = false;
var exit = false;
string mazeInput = "";

var mazeArray = new string[64];


while (!validInput)
{
    Console.WriteLine($"Please enter the arrow _solver.maze you want to solve");
    mazeInput = Console.ReadLine();

    if (mazeInput == null || mazeInput.Length == 0)
        Console.WriteLine("not a valid input");
    else
    {
        try
        {
            mazeArray = JsonConvert.DeserializeObject<string[]>(mazeInput);
            validInput = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("not a valid input");
            validInput = false;
        }
    }
}

var errors = new List<string>();
var k = 0;
for (int row = 0; row < 8; row++)
{
    for (int col = 0; col < 8; col++)
    {
        _solver.maze[row, col] = new Value();
        var number = Regex.Match(mazeArray[k], @"\d+").Value;

        if (number.Length != 0)
            _solver.maze[row, col].Answer = Int32.Parse(number);

        _solver.maze[row, col].Row = row;
        _solver.maze[row, col].Col = col;

        var direction = Regex.Replace(mazeArray[k], @"\d+", string.Empty);
        switch (direction)
        {
            case "D":
                _solver.maze[row, col].Direction = Value.ArrowDirection.D;
                break;
            case "U":
                _solver.maze[row, col].Direction = Value.ArrowDirection.U;
                break;
            case "L":
                _solver.maze[row, col].Direction = Value.ArrowDirection.L;
                break;
            case "R":
                _solver.maze[row, col].Direction = Value.ArrowDirection.R;
                break;
            case "DR":
                _solver.maze[row, col].Direction = Value.ArrowDirection.DR;
                break;
            case "DL":
                _solver.maze[row, col].Direction = Value.ArrowDirection.DL;
                break;
            case "UL":
                _solver.maze[row, col].Direction = Value.ArrowDirection.UL;
                break;
            case "UR":
                _solver.maze[row, col].Direction = Value.ArrowDirection.UR;
                break;
            case "":
                _solver.maze[row, col].Direction = Value.ArrowDirection.Empty;
                break;
            default:
                errors.Add(k.ToString());
                break;
        }
        k++;
    }
    
    if (errors.Count > 0)
    {
        Console.Write("You have invalid inputs at: ");
        foreach (var error in errors) { Console.WriteLine(error + ", "); };
        Console.WriteLine("please try again");
        goto Foo;
    }
}

var solution = _solver.Solve();
Console.WriteLine(solution);




