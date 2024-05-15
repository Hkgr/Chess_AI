using MyChess;

class Program
{
    static void Main(string[] args)
    {
        int[][] position = new int[][]
            {      //a  b  c  d  e  f  g  h
            new [] { 6, 0, 0, 0, 0, 5, 0, 0 }, //8
            new [] { 4, 4, 4, 4, 4, 4, 4, 4 }, //7
            new [] { 0, 0, 0, 0, 0, 0, 0, 0 }, //6
            new [] { 0, 0, 0, 0, 0, 0, 0, 0 }, //5
            new [] { 0, 0, 0, 0, 0, 0, 0, 0 }, //4
            new [] { 0, 0, 0, 0, 0, 0, 0, 0 }, //3
            new [] { 1, 1, 1, 1, 1, 1, 1, 1 }, //2 
            new [] { 0, 0, 2, 0, 0, 0, 0, 3 }  //1
            };     //a  b  c  d  e  f  g  h





        //string bestMove = CPE751.Project(position);
        //Console.WriteLine("Best Move: " + bestMove);
        // PrintBoard(position);
        // CPE751.PrintePossibleMoves(position);





    }

    static void PrintBoard(int[][] position)
    {
        Console.Write(position);
    }



}
