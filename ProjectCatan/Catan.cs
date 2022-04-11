using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCatan
{

    internal class Catan
    {

    }

    enum Resource
    {
        Wood,
        Brick,
        Sheep,
        Wheat,
        Ore,
        Desert
    }

    enum Vertex
    {
        Settlement,
        City
    }

    enum Team
    {
        Red,
        Blue,
        Yellow,
        White
    }

    internal class Point
    {
        //https://www.redblobgames.com/grids/hexagons/

        private readonly int q;
        private readonly int r;
        private readonly int s;

        public int Q => q;
        public int R => r;
        public int S => s;

        public Point()
        {
            q = 0;
            r = 0;
            s = 0;
        }

        public Point(int q, int r, int s)
        {
            this.q = q;
            this.r = r;
            this.s = s;
        }

        public Point RightUp(int distance = 1) => new(Q + distance, R - distance, S);
        public Point Right(int distance = 1) => new(Q + distance, R, S - distance);
        public Point RightDown(int distance = 1) => new(Q, R + distance, S - distance);
        public Point LeftDown(int distance = 1) => new(Q - distance, R + distance, S);
        public Point Left(int distance = 1) => new(Q - distance, R, S + distance);
        public Point LeftUp(int distance = 1) => new(Q, R - distance, S + distance);

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType()) { return false; }
            Point point = (Point)obj;
            return Q == point.Q && R == point.R && S == point.S;
        }

        public override int GetHashCode()
        {
            return Q.GetHashCode() ^ R.GetHashCode() ^ S.GetHashCode();
        }

        public override string ToString()
        {
            return $"({Q}, {R}, {S})";
        }

        public static Point operator +(Point aPoint, Point bPoint)
        {
            return new Point(aPoint.Q + bPoint.Q, aPoint.R + bPoint.R, aPoint.S + bPoint.S);
        }
        public static Point operator -(Point aPoint, Point bPoint)
        {
            return new Point(aPoint.Q - bPoint.Q, aPoint.R - bPoint.R, aPoint.S - bPoint.S);
        }
    }

    internal class Cell
    {
        private readonly Point point;
        private readonly Resource resource;
        private readonly int number;

        private readonly Vertex[] vertices = new Vertex[6];
        private readonly Team[] vertexTeams = new Team[6];
        private readonly bool[] roads = new bool[6];
        private readonly Team[] roadTeams = new Team[6];

        private readonly bool isThieves;

        public Cell()
        {
            point = new Point(-1, -1, -1);
        }

        public Cell(int q, int r, int s, Resource resource, int number)
        {
            point = new Point(q, r, s);
            this.resource = resource;
            this.number = number;
        }

        public Cell(Point point, Resource resource, int number)
        {
            this.point = point;
            this.resource = resource;
            this.number = number;
        }
        public Point Point => point;
        public Resource Resource => resource;
        public int Number => number;

        public bool IsRoad(int index) => roads[index];
        public bool IsRoad(int index, Team team) => roadTeams[index] == team;

        public bool IsVertex(int index) => vertices[index] != 0;
        public bool IsVertex(int index, Team team) => vertexTeams[index] == team;
        public bool IsThieves => isThieves;

        public bool IsNull => Point == new Point(-1, -1, -1);

        public bool GetRoad(int index) => roads[index];
        public Team GetRoadTeam(int index) => roadTeams[index];
        public Vertex GetVertex(int index) => vertices[index];
        public Team GetVertexTeam(int index) => vertexTeams[index];

        public void SetRoad(int index, Team team)
        {
            roads[index] = true;
            roadTeams[index] = team;
        }
        public void SetVertex(int index, Team team, Vertex vertex)
        {
            vertices[index] = vertex;
            vertexTeams[index] = team;
        }

        public int ConvertIndex(int oldIndex, Point oldPoint)
        {
            if (Point == oldPoint.RightUp())
            {
                if (oldIndex == 0) { return 4; }
                if (oldIndex == 1) { return 3; }
            }
            if (Point == oldPoint.Right())
            {
                if (oldIndex == 1) { return 5; }
                if (oldIndex == 2) { return 4; }
            }
            if (Point == oldPoint.RightDown())
            {
                if (oldIndex == 2) { return 0; }
                if (oldIndex == 3) { return 5; }
            }
            if (Point == oldPoint.LeftDown())
            {
                if (oldIndex == 3) { return 1; }
                if (oldIndex == 4) { return 0; }
            }
            if (Point == oldPoint.Left())
            {
                if (oldIndex == 4) { return 2; }
                if (oldIndex == 5) { return 1; }
            }
            if (Point == oldPoint.LeftUp())
            {
                if (oldIndex == 5) { return 3; }
                if (oldIndex == 0) { return 2; }
            }
            return -1;
        }

        public int Count(Team team)
        {
            if (IsThieves) { return 0; }
            int count = 0;
            for (int i = 0; i < 6; i++)
            {
                if (vertexTeams[i] == team)
                {
                    switch (vertices[i])
                    {
                        case Vertex.Settlement:
                            count++;
                            break;
                        case Vertex.City:
                            count += 2;
                            break;
                    }
                }
            }
            return count;
        }
    }

    internal class Board
    {
        private readonly Cell[] cells;

        public Board()
        {
            cells = new Cell[19];
            cells[0] = new Cell(0, 0, 0, Resource.Wheat, 11);
            cells[1] = new Cell(1, -1, 0, Resource.Brick, 5);
            cells[2] = new Cell(2, -2, 0, Resource.Wheat, 9);
            cells[3] = new Cell(-1, 1, 0, Resource.Sheep, 10);
            cells[4] = new Cell(-2, 2, 0, Resource.Ore, 5);
            cells[5] = new Cell(0, -1, 1, Resource.Ore, 6);
            cells[6] = new Cell(0, -2, 2, Resource.Wood, 11);
            cells[7] = new Cell(0, 1, -1, Resource.Sheep, 9);
            cells[8] = new Cell(0, 2, -2, Resource.Wood, 6);
            cells[9] = new Cell(1, 0, -1, Resource.Wood, 4);
            cells[10] = new Cell(2, 0, -2, Resource.Wheat, 8);
            cells[11] = new Cell(-1, 0, 1, Resource.Wood, 3);
            cells[12] = new Cell(-2, 0, 2, Resource.Desert, 0);
            cells[13] = new Cell(1, 1, -2, Resource.Ore, 3);
            cells[14] = new Cell(-1, 2, -1, Resource.Wheat, 2);
            cells[15] = new Cell(-2, 1, 1, Resource.Brick, 8);
            cells[16] = new Cell(-1, -1, 2, Resource.Brick, 4);
            cells[17] = new Cell(1, -2, 1, Resource.Sheep, 12);
            cells[18] = new Cell(2, -1, -1, Resource.Sheep, 10);
        }

        public bool IsRange(Point point) => cells.Where(x => x.Point == point).Any();

        public Cell GetCell(Point point) => IsRange(point) ? cells.Where(x => x.Point == point).ToArray()[0] : new();

        public Cell[] GeRoundedCells(Point point) => cells.Where(x => Math.Abs(x.Point.Q - point.Q) == 1 && Math.Abs(x.Point.R - point.R) == 1 && Math.Abs(x.Point.S - point.S) == 1 && !x.IsNull).ToArray();

        public void GetRoundedVertex(Point point, int index, out Vertex vertex, out Team vertexTeam)
        {
            vertex = 0;
            vertexTeam = 0;
            switch (index)
            {
                case 0:
                    if (!GetCell(point.LeftUp()).IsNull)
                    {
                        vertex = GetCell(point.LeftUp()).GetVertex(1);
                        vertexTeam = GetCell(point.LeftUp()).GetVertexTeam(1);
                        return;
                    }
                    if (!GetCell(point.RightUp()).IsNull)
                    {
                        vertex = GetCell(point.RightUp()).GetVertex(5);
                        vertexTeam = GetCell(point.RightUp()).GetVertexTeam(5);
                        return;
                    }
                    break;
                case 1:
                    if (!GetCell(point.RightUp()).IsNull)
                    {
                        vertex = GetCell(point.RightUp()).GetVertex(2);
                        vertexTeam = GetCell(point.RightUp()).GetVertexTeam(2);
                        return;
                    }
                    if (!GetCell(point.RightUp()).IsNull)
                    {
                        vertex = GetCell(point.Right()).GetVertex(0);
                        vertexTeam = GetCell(point.Right()).GetVertexTeam(0);
                        return;
                    }
                    break;
                case 2:
                    if (!GetCell(point.Right()).IsNull)
                    {
                        vertex = GetCell(point.Right()).GetVertex(3);
                        vertexTeam = GetCell(point.Right()).GetVertexTeam(3);
                        return;
                    }
                    if (!GetCell(point.RightDown()).IsNull)
                    {
                        vertex = GetCell(point.RightDown()).GetVertex(1);
                        vertexTeam = GetCell(point.RightDown()).GetVertexTeam(1);
                        return;
                    }
                    break;
                case 3:
                    if (!GetCell(point.RightDown()).IsNull)
                    {
                        vertex = GetCell(point.RightDown()).GetVertex(4);
                        vertexTeam = GetCell(point.RightDown()).GetVertexTeam(4);
                        return;
                    }
                    if (!GetCell(point.LeftDown()).IsNull)
                    {
                        vertex = GetCell(point.LeftDown()).GetVertex(2);
                        vertexTeam = GetCell(point.LeftDown()).GetVertexTeam(2);
                        return;
                    }
                    break;
                case 4:
                    if (!GetCell(point.LeftDown()).IsNull)
                    {
                        vertex = GetCell(point.LeftDown()).GetVertex(5);
                        vertexTeam = GetCell(point.LeftDown()).GetVertexTeam(5);
                        return;
                    }
                    if (!GetCell(point.Left()).IsNull)
                    {
                        vertex = GetCell(point.Left()).GetVertex(3);
                        vertexTeam = GetCell(point.Left()).GetVertexTeam(3);
                        return;
                    }
                    break;
                case 5:
                    if (!GetCell(point.Left()).IsNull)
                    {
                        vertex = GetCell(point.Left()).GetVertex(0);
                        vertexTeam = GetCell(point.Left()).GetVertexTeam(0);
                        return;
                    }
                    if (!GetCell(point.LeftUp()).IsNull)
                    {
                        vertex = GetCell(point.LeftUp()).GetVertex(4);
                        vertexTeam = GetCell(point.LeftUp()).GetVertexTeam(4);
                        return;
                    }
                    break;
            }
        }

        public void GetRoundedRoad(Point point, int index, out bool road, out Team roadTeam, bool reverse = false)
        {
            if (!reverse)
            {
                switch (index)
                {
                    case 0:
                        road = GetCell(point.RightUp()).GetRoad(4);
                        roadTeam = GetCell(point.RightUp()).GetRoadTeam(4);
                        return;
                    case 1:
                        road = GetCell(point.Right()).GetRoad(5);
                        roadTeam = GetCell(point.Right()).GetRoadTeam(5);
                        return;
                    case 2:
                        road = GetCell(point.RightDown()).GetRoad(0);
                        roadTeam = GetCell(point.RightDown()).GetRoadTeam(0);
                        return;
                    case 3:
                        road = GetCell(point.LeftDown()).GetRoad(1);
                        roadTeam = GetCell(point.LeftDown()).GetRoadTeam(1);
                        return;
                    case 4:
                        road = GetCell(point.Left()).GetRoad(2);
                        roadTeam = GetCell(point.Left()).GetRoadTeam(2);
                        return;
                    case 5:
                        road = GetCell(point.LeftUp()).GetRoad(3);
                        roadTeam = GetCell(point.LeftUp()).GetRoadTeam(3);
                        return;
                }
            }
            GetRoundedRoad(point, index == 0 ? 5 : (index - 1), out road, out roadTeam);
            return;
        }

        public bool canSetSettlement(Point point, int index, Team team)
        {
            GetRoundedVertex(point, index, out Vertex vertex, out Team _);
            if (GetCell(point).GetVertex(index == 0 ? 5 : (index - 1)) != 0) { return false; }
            if (GetCell(point).GetVertex(index == 5 ? 0 : (index + 1)) != 0) { return false; }
            if (vertex != 0) { return false; }
            GetRoundedRoad(point, index, out bool _, out Team roadTeam);
            if (GetCell(point).GetRoadTeam(index == 0 ? 5 : (index - 1)) != team && GetCell(point).GetRoadTeam(index == 5 ? 0 : (index + 1)) != team && roadTeam != team) { return false; }
            return true;
        }



    }
}
