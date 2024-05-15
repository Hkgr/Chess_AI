using MyChess;

class Program
{
    static void Main(string[] args)
    {
        int[][] position = new int[][]
        {
            new [] { 6, 0, 0, 0, 0, 5, 0, 0 },
            new [] { 4, 4, 4, 4, 4, 4, 4, 4 },
            new [] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new [] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new [] { 0, 0, 0, 4, 0, 5, 0, 0 },
            new [] { 1, 1, 1, 1, 1, 1, 1, 1 },
            new [] { 0, 6, 0, 0, 0, 0, 0, 0 },
            new [] { 0, 0, 2, 0, 0, 0, 0, 3 }
        };





        string bestMove = CPE751.Project(position);

        Console.WriteLine("Best Move: " + bestMove);



        PrintBoard(position);

        CPE751.PrintePossibleMoves(position);





    }

    static void PrintBoard(int[][] position)
    {
        Console.Write(position);
    }



}
