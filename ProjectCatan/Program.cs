using System;

namespace ProjectCatan
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Board board = new();
            //foreach (var item in board.Cells)
            //{
            //    Console.WriteLine(item);
            //}
            //Console.ReadLine();
            board.GetCell(new Point()).SetVertex(0, Team.Red, Vertex.Settlement);
            board.GetCell(new Point().NorthEast).SetVertex(4, Team.Red, Vertex.Settlement);
            board.SetRoad(new Point(), 0, Team.Red);
            Console.WriteLine(board.GetCell(new Point(0, 0, 0)));
            Console.WriteLine(board.GetCell(new Point().NorthEast));
            //board.SetRoad(new Point(), 1, Team.Red);
            //Console.WriteLine(board.GetCell(new Point()));
            //Console.WriteLine(board.GetCell(new Point().East));
            //board.SetSettlement(new Point(), 3, Team.Red);
            //Console.WriteLine(board.GetCell(new Point()));
            //Console.WriteLine(board.GetCell(new Point().SouthEast));
            //Console.WriteLine(board.GetCell(new Point().SouthWest));
        }
    }
}