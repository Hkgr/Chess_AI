using System;
using System.Collections.Generic;
using System.Data.Common;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MyChess
{
    class CPE751
    {
        private const int Empty = 0;
        private const int WhitePawn = 1;
        private const int WhiteBishop = 2;
        private const int WhiteRook = 3;
        private const int BlackPawn = 4;
        private const int BlackBishop = 5;
        private const int BlackRook = 6;


        private static readonly bool[,] PawnMoved = new bool[8, 8]; 

        private static readonly Dictionary<int, Func<int[][], int, int, List<string>>> PieceMoveFunctions = new Dictionary<int, Func<int[][], int, int, List<string>>>()
        {
            { WhitePawn, (position, row, column) => GetPossiblePawnMoves(position, row, column) },
            { WhiteBishop, (position, row, column) => GetPossibleBishopMoves(position, row, column) },
            { WhiteRook, (position, row, column) => GetPossibleRookMoves(position, row, column) },
            { BlackPawn, (position, row, column) => GetPossiblePawnMoves(position, row, column) },
            { BlackBishop, (position, row, column) => GetPossibleBishopMoves(position, row, column) },
            { BlackRook, (position, row, column) => GetPossibleRookMoves(position, row, column) }
        };


        public static List<string> GetPossibleMoves(int[][] position, int row, int column)
        {
            int piece = position[row][column];
            if (piece == Empty) return new List<string>();

            var possibleMoves = PieceMoveFunctions[piece](position, row, column);
            List<string> updatedMoves = new List<string>();

            foreach (var move in possibleMoves)
            {
                if (IsCaptureMove(position, move))
                {
                    updatedMoves.Add(move);
                }
                else
                {
                    updatedMoves.Add(move); 
                }
            }

            return updatedMoves;
        }


        private static void WhoAmI(int[][] position)
        {
            int[][] defaultPosition = new int[][]
            {
            new [] { 6, 0, 0, 0, 0, 5, 0, 0 },
            new [] { 4, 4, 4, 4, 4, 4, 4, 4 },
            new [] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new [] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new [] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new [] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new [] { 1, 1, 1, 1, 1, 1, 1, 1 },
            new [] { 0, 0, 2, 0, 0, 0, 0, 3 }
            };

            string color = "white";

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (position[i][j] != defaultPosition[i][j])
                    {
                        if (position[i][j] == 6 || position[i][j] == 4 || position[i][j] == 5)
                        {
                            color = "white";
                            break;
                        }
                        else if (position[i][j] == 1 || position[i][j] == 2 || position[i][j] == 3)
                        {
                            color = "black";
                            break;
                        }
                    }
                }
            }

            string filePath = "output.xml";
            if (!File.Exists(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(string));
                using (TextWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, color);
                }
            }
        }
        private static List<string> GetPossiblePawnMoves(int[][] position, int row, int column)
        {
            int piece = position[row][column];
            List<string> possibleMoves = new List<string>();

            int forwardDirection = (piece == WhitePawn) ? -1 : 1;

            int newRow = row + forwardDirection;
            if ((IsValidPosition(newRow, column) && position[newRow][column] == Empty))
            {
                possibleMoves.Add($"{GetChessNotation(row, column)}{GetChessNotation(newRow, column)}");

                if (PawnMoved[row, column])
                {
                    int nextRow = newRow + forwardDirection;
                    if (IsValidPosition(nextRow, column) && position[nextRow][column] != Empty)
                    {
                        possibleMoves.Add($"{GetChessNotation(row, column)}{GetChessNotation(nextRow, column)}");
                    }
                }
            }

            int[] captureColumns = { column - 1, column + 1 };
            foreach (int captureColumn in captureColumns)
            {
                if (IsValidPosition(newRow, captureColumn))
                {
                    int capturedPiece = position[newRow][captureColumn];
                    if ((capturedPiece == Empty || Math.Sign(capturedPiece) != Math.Sign(position[row][column])))
                    {
                        continue;
                    }
                    possibleMoves.Add($"{GetChessNotation(row, column)}{GetChessNotation(newRow, captureColumn)}");
                }
            }
            int[][] shootingDirections = { new int[] { forwardDirection, 1 }, new int[] { forwardDirection, -1 } };
            foreach (var direction in shootingDirections)
            {
                int shootingRow = row + direction[0];
                int shootingColumn = column + direction[1];
                if (IsValidPosition(shootingRow, shootingColumn) && position[shootingRow][shootingColumn] == WhitePawn)
                {
                    possibleMoves.Add($"{GetChessNotation(row, column)}{GetChessNotation(shootingRow, shootingColumn)}");
                }
            }
            if (!PawnMoved[row, column])
            {
                PawnMoved[row, column] = true;
            }

            return possibleMoves;
        }
        private static List<string> GetPossibleBishopMoves(int[][] position, int row, int column)
        {
            List<string> possibleMoves = new List<string>();

            int piece = position[row][column];
            int myColor = Math.Sign(piece);

            for (int dx = -1; dx <= 1; dx += 2)
            {
                for (int dy = -1; dy <= 1; dy += 2)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        int newRow = row + i * dx;
                        int newColumn = column + i * dy;

                        if (!IsValidPosition(newRow, newColumn)) break;

                        int newPositionPiece = position[newRow][newColumn];
                        int newPositionColor = Math.Sign(newPositionPiece);

                        if (newPositionPiece != Empty && newPositionColor == myColor)
                        {
                            possibleMoves.Add($"{GetChessNotation(row, column)}{GetChessNotation(newRow, newColumn)}");
                        }

                        if (newPositionPiece != Empty) break;
                    }
                }
            }

            return possibleMoves;
        }


        private static List<string> GetPossibleRookMoves(int[][] position, int row, int column)
        {
            List<string> possibleMoves = new List<string>();

            foreach (int[] direction in new int[][] { new int[] { -1, 0 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { 0, 1 } })
            {
                int dx = direction[0];
                int dy = direction[1];

                for (int i = 1; i < 8; i++)
                {
                    int newRow = row + i * dx;
                    int newColumn = column + i * dy;

                    if (!IsValidPosition(newRow, newColumn)) break;

                    if (position[newRow][newColumn] != Empty)
                    {
                        if (Math.Abs(position[newRow][newColumn]) != Math.Abs(position[row][column]))
                        {
                            possibleMoves.Add($"{GetChessNotation(row, column)}{GetChessNotation(newRow, newColumn)}");
                        }
                        break;
                    }

                    else
                    {
                        possibleMoves.Add($"{GetChessNotation(row, column)}{GetChessNotation(newRow, newColumn)}");
                    }
                }
            }

            return possibleMoves;
        }

        private static bool IsValidPosition(int row, int column)
        {
            return row >= 0 && row < 8 && column >= 0 && column < 8;
        }
        private static bool IsValidMoveColor(int[][] position, string move)
        {
            int fromRow = '8' - move[1];
            int fromColumn = move[0] - 'a';
            int toRow = '8' - move[3];
            int toColumn = move[2] - 'a';

            int fromPiece = position[fromRow][fromColumn];
            int toPiece = position[toRow][toColumn];


            string getFromPieceColor = GetPieceColor(fromPiece);


            string getToPieceColor = GetPieceColor(toPiece);

            if (getFromPieceColor == getToPieceColor)
            {
                return true;
            }
            else { return false; }
        }
        private static int IsPieceMove(int[][] position, string move)
        {
            int fromRow = '8' - move[1];
            int fromColumn = move[0] - 'a';
            int toRow = '8' - move[3];
            int toColumn = move[2] - 'a';

            int fromPiece = position[fromRow][fromColumn];
            int toPiece = position[toRow][toColumn];

            string getFromPieceType = GetPieceType(fromPiece);
            string getFromPieceColor = GetPieceColor(fromPiece);

            string getToPieceType = GetPieceType(toPiece);
            string getToPieceColor = GetPieceColor(toPiece);

            if (getToPieceType == "Rook")
            { return 1000; }
            else if (getToPieceType == "Pawn" && getFromPieceType == "Pawn") { return 200; }
            else if (getToPieceType == "Bishop") { return 500; }
            else { return 0; }
        }
        private static bool IsPawnMove(int[][] position, string move)
        {
            int fromRow = '8' - move[1];
            int fromColumn = move[0] - 'a';


            int fromPiece = position[fromRow][fromColumn];

            string getFromPieceType = GetPieceType(fromPiece);


            if (getFromPieceType == "Pawn") { return true; }
            else { return false; }
        }
        private static int isSamePlace(int[][] position, string move)
        {
            int[][] DefaultPosition = new int[][]
{
        new [] { 6, 0, 0, 0, 0, 5, 0, 0 },
        new [] { 4, 4, 4, 4, 4, 4, 4, 4 },
        new [] { 0, 0, 0, 0, 0, 0, 0, 0 },
        new [] { 0, 0, 0, 0, 0, 0, 0, 0 },
        new [] { 0, 0, 0, 0, 0, 0, 0, 0 },
        new [] { 0, 0, 0, 0, 0, 0, 0, 0 },
        new [] { 1, 1, 1, 1, 1, 1, 1, 1 },
        new [] { 0, 0, 2, 0, 0, 0, 0, 3 }
                               };
            int fromRow = '8' - move[1];
            int fromColumn = move[0] - 'a';


            int fromPiece = position[fromRow][fromColumn];
            string getToPieceColor = GetPieceColor(fromPiece);

            if (getToPieceColor == "white")
            {
                if (DefaultPosition[fromRow][fromColumn] == 1)


                {
                    return 10000;
                }
                else { return 0; }
            }
            else if (getToPieceColor == "black")
            {
                if (DefaultPosition[fromRow][fromColumn] == 4)


                {
                    return 10000;
                }
            }

            else { return 0; }

            return 0;

        }

        private static bool IsAttackMove(int[][] position, string move)
        {
            int fromRow = '8' - move[1];
            int fromColumn = move[0] - 'a';
            int toRow = '8' - move[3];
            int toColumn = move[2] - 'a';

            int fromPiece = position[fromRow][fromColumn];
            int toPiece = position[toRow][toColumn];

            string getFromPieceType = GetPieceType(fromPiece);
            string getFromPieceColor = GetPieceColor(fromPiece);

            string getToPieceType = GetPieceType(toPiece);
            string getToPieceColor = GetPieceColor(toPiece);

            if (getFromPieceColor != getToPieceColor && getToPieceColor != "Empty")
            {
                return true;
            }

            return false;
        }


        public static string GetChessNotation(int row, int column)
        {
            return $"{(char)('a' + column)}{8 - row}"; 
        }



        private static string GetPieceType(int piece)
        {
            switch (Math.Abs(piece))
            {
                case WhitePawn:
                    return "Pawn";
                case WhiteBishop:
                    return "Bishop";
                case WhiteRook:
                    return "Rook";
                case BlackPawn:
                    return "Pawn";
                case BlackBishop:
                    return "Bishop";
                case BlackRook:
                    return "Rook";
                default:
                    return "Unknown";
            }
        }


        private static string GetPieceColor(int piece)
        {
            switch (piece)
            {
                case 1:
                case 2:
                case 3:
                    return "white";
                case 4:
                case 5:
                case 6:
                    return "black";
                case 0:
                    return "Empty";
                default:
                    return "Empty";
            }
        }

        private static string RetrieveColorFromXML(string filePath)
        {
            string color = "";

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(string));
                using (TextReader reader = new StreamReader(filePath))
                {
                    color = (string)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving color from XML: " + ex.Message);
            }

            return color;
        }
        //public static void PrintePossibleMoves(int[][] position)

        //{
        //    string color = RetrieveColorFromXML("output.xml");
        //    Console.WriteLine(color);
        //    for (int row = 0; row < 8; row++)
        //    {
        //        for (int column = 0; column < 8; column++)
        //        {
        //            int piece = position[row][column];

        //            if (piece != 0) 
        //            {
        //                string pieceColor = GetPieceColor(piece); 
        //                string pieceType = GetPieceType(piece); 

        //                Console.WriteLine($"Moves for {pieceColor} {pieceType} at position {GetChessNotation(row, column)}: "); // طباعة لون القطعة بدون سطر جديد

        //                List<string> possibleMoves = GetPossibleMoves(position, row, column);
        //                foreach (var move in possibleMoves)
        //                {
        //                    string pieceType1 = GetPieceTypeFromMove(position, move);
        //                    int moveValue = GetMoveValue(position, move);
        //                    Console.WriteLine($"{move} - Value: {moveValue} - type : {pieceType1}"); 


        //                }
        //            }
        //        }
        //    }
        //}

        public static string Project(int[][] position)
        {
            WhoAmI(position);
            int bestMoveValue = int.MinValue;
            string bestMove = "";

            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    int piece = position[row][column];

                    if (piece != 0) 
                    {
                        List<string> possibleMoves = GetPossibleMoves(position, row, column);
                        foreach (var move in possibleMoves)
                        {
                            int moveValue = GetMoveValue(position, move);

                            if (moveValue > bestMoveValue)
                            {
                                bestMoveValue = moveValue;
                                bestMove = move;
                            }
                        }
                    }
                }

            }



            return bestMove;
        }

        public static string GetPieceTypeFromMove(int[][] position, string move)
        {
            int fromRow = '8' - move[1];
            int fromColumn = move[0] - 'a';
            int piece = position[fromRow][fromColumn];

            return GetPieceType(piece);
        }



        private static int GetMoveValue(int[][] position, string move)
        {
            int fromRow = '8' - move[1]; 
            int fromColumn = move[0] - 'a';
            int toRow = '8' - move[3]; 
            int toColumn = move[2] - 'a';

            double distance = Math.Sqrt(Math.Pow(toRow - fromRow, 2) + Math.Pow(toColumn - fromColumn, 2));

            int curPiece = position[fromRow][fromColumn];
            string getMoveColor = GetPieceColor(curPiece);
            string getPlayerColor = RetrieveColorFromXML("output.xml");

            int getMouveValuebyPiece = IsPieceMove(position, move);

            int moveValue = 0;




            if (IsPawnMove(position, move) == true)
            {
                moveValue += isSamePlace(position, move);
            }


            moveValue += getMouveValuebyPiece;
            moveValue += (int)Math.Round(distance);



            if (getMoveColor != getPlayerColor)
            {
                moveValue = 0;
            }

            if (IsAttackMove(position, move) == true)
            {
                moveValue *= 3;
            }

            if (IsValidMoveColor(position, move) == true)
            {
                moveValue = 0;
            }



            return moveValue;
        }

        private static bool IsCaptureMove(int[][] position, string move)
        {
            int fromRow = '8' - move[1];
            int fromColumn = move[0] - 'a';
            int toRow = '8' - move[3];
            int toColumn = move[2] - 'a';

            int fromPiece = position[fromRow][fromColumn];
            int toPiece = position[toRow][toColumn];

            if (fromPiece != Empty && toPiece == Empty)
            {
                return false; 
            }

            if (fromPiece != Empty && toPiece != Empty)
            {
                if (Math.Sign(fromPiece) == 1 && Math.Sign(toPiece) == -1)
                {
                    return true;
                }

                if (Math.Sign(fromPiece) == -1 && Math.Sign(toPiece) == 1)
                {
                    return true;
                }
            }

            return false;
        }




    }
}