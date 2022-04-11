using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCatan
{
    enum Resource
    {
        Null,
        Wood,
        Brick,
        Sheep,
        Wheat,
        Ore,
        Desert
    }

    enum Vertex
    {
        Null,
        Settlement,
        City
    }

    enum Team
    {
        Null,
        Red,
        Blue,
        Yellow,
        White
    }


    internal class Point : IEquatable<Point>
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

        public Point NorthEast => new(Q + 1, R - 1, S);
        public Point East => new(Q + 1, R, S - 1);
        public Point SouthEast => new(Q, R + 1, S - 1);
        public Point SouthWest => new(Q - 1, R + 1, S);
        public Point West => new(Q - 1, R, S + 1);
        public Point NorthWest => new(Q, R - 1, S + 1);

        public Point Vector(int index)
        {
            return index switch
            {
                0 => NorthEast,
                1 => East,
                2 => SouthEast,
                3 => SouthWest,
                4 => West,
                5 => NorthWest,
                _ => this,
            };
        }

        public override string ToString()
        {
            return $"({Q}, {R}, {S})";
        }

        public bool Equals(Point? other)
        {
            if (other is null) { return false; }
            return Q == other.Q && R == other.R && S == other.S;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Point);
        }

        public override int GetHashCode()
        {
            return Q.GetHashCode() ^ R.GetHashCode() ^ S.GetHashCode();
        }

        public static bool operator ==(Point aPoint, Point bPoint)
        {
            if (aPoint is null) return bPoint is null;
            return aPoint.Equals(bPoint);
        }

        public static bool operator !=(Point aPoint, Point bPoint)
        {
            return !(aPoint == bPoint);
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
        private readonly Team[] roadTeams = new Team[6];

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

        public bool IsRoad(int index, Team team) => roadTeams[index] == team;

        public bool IsVertex(int index) => vertices[index] != 0;
        public bool IsVertex(int index, Team team) => vertexTeams[index] == team;
        public bool IsThieves { get; set; }


        public bool IsNull => Point.Equals(new Point(-1, -1, -1));

        public Team GetRoadTeam(int index) => roadTeams[index];
        public Vertex GetVertex(int index) => vertices[index];
        public Team GetVertexTeam(int index) => vertexTeams[index];

        public void SetRoad(int index, Team team)
        {
            roadTeams[index] = team;
        }

        public void SetVertex(int index, Team team, Vertex vertex)
        {
            vertices[index] = vertex;
            vertexTeams[index] = team;
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

        public override string ToString()
        {
            string value = "";
            for (int i = 0; i < 6; i++)
            {
                value += $"{GetVertex(i).ToString()[..1]}({GetVertexTeam(i).ToString()[..1]})";
                value += $"-{GetRoadTeam(i).ToString()[..1]}-";
            }
            return $"{Point} {value}";
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

        public Cell[] Cells => cells;

        private static int Index(int index, int value) => (index + value + 6) % 6;

        public bool IsRange(Point point) => cells.Where(x => x.Point == point).Any();

        public Cell GetCell(Point point) => IsRange(point) ? cells.Where(x => x.Point == point).ToArray()[0] : new();

        public bool CanSetSettlement(Point point, int index, Team team)
        {
            if (GetCell(point).IsVertex(Index(index, -1))) { return false; }
            if (GetCell(point).IsVertex(Index(index, 1))) { return false; }
            if (!GetCell(point.Vector(index)).IsNull) { if (GetCell(point.Vector(index)).IsVertex(Index(index, -1))) { return false; } }
            if (!GetCell(point.Vector(Index(index, -1))).IsNull) { if (GetCell(point.Vector(Index(index, -1))).IsVertex(Index(index, 1))) { return false; } }
            if (GetCell(point).IsRoad(index, team)) { return true; }
            if (GetCell(point).IsRoad(Index(index, -1), team)) { return true; }
            if (!GetCell(point.Vector(index)).IsNull) { if (GetCell(point.Vector(index)).IsRoad(Index(index, -2), team)) { return true; } }
            if (!GetCell(point.Vector(Index(index, -1))).IsNull) { if (GetCell(point.Vector(Index(index, -1))).IsRoad(Index(index, 2), team)) { return true; } }
            return false;
        }

        public bool CanSetCity(Point point, int index, Team team) => GetCell(point).GetVertex(index) == Vertex.Settlement && GetCell(point).GetVertexTeam(index) == team;

        public bool CanSetRoad(Point point, int index, Team team)
        {
            if (GetCell(point).IsVertex(Index(index, 1), team)) { return true; }
            if (GetCell(point).IsVertex(index, team)) { return true; }
            if (GetCell(point).IsRoad(Index(index, 1), team)) { return true; }
            if (GetCell(point).IsRoad(Index(index, -1), team)) { return true; }
            if (GetCell(point.Vector(index)).IsNull) { return true; }
            else
            {
                if (GetCell(point.Vector(index)).IsRoad(Index(index, -2), team)) { return true; }
                if (GetCell(point.Vector(index)).IsRoad(Index(index, 2), team)) { return true; }
            }
            return false;
        }

        public void SetSettlement(Point point, int index, Team team)
        {
            if (!CanSetSettlement(point, index, team)) { return; }
            GetCell(point).SetVertex(index, team, Vertex.Settlement);
            if (!GetCell(point.Vector(index)).IsNull) { GetCell(point.Vector(index)).SetVertex(Index(index, -2), team, Vertex.Settlement); }
            if (!GetCell(point.Vector(Index(index, -1))).IsNull) { GetCell(point.Vector(Index(index, -1))).SetVertex(Index(index, 2), team, Vertex.Settlement); }
        }

        public void SetRoad(Point point, int index, Team team)
        {
            if (!CanSetRoad(point, index, team)) { return; }
            GetCell(point).SetRoad(index, team);
            if (!GetCell(point.Vector(index)).IsNull) { GetCell(point.Vector(index)).SetRoad(Index(index, 3), team); }
        }

        public void SetThieves(Point point)
        {
            _ = cells.All(x => x.IsThieves = false);
            GetCell(point).IsThieves = true;
        }
    }
}
