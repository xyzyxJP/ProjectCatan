﻿using System;
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

        public Point RightUp() => new(Q + 1, R - 1, S);
        public Point Right() => new(Q + 1, R, S - 1);
        public Point RightDown() => new(Q, R + 1, S - 1);
        public Point LeftDown() => new(Q - 1, R + 1, S);
        public Point Left() => new(Q - 1, R, S + 1);
        public Point LeftUp() => new(Q, R - 1, S + 1);

        public Point RightUp(int distance) => new(Q + distance, R - distance, S);
        public Point Right(int distance) => new(Q + distance, R, S - distance);
        public Point RightDown(int distance) => new(Q, R + distance, S - distance);
        public Point LeftDown(int distance) => new(Q - distance, R + distance, S);
        public Point Left(int distance) => new(Q - distance, R, S + distance);
        public Point LeftUp(int distance) => new(Q, R - distance, S + distance);

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

        public Cell[] GeRoundedCells(Point point) => cells.Where(x => Math.Abs(x.Point.Q - point.Q) == 1 && Math.Abs(x.Point.R - point.R) == 1 && Math.Abs(x.Point.S - point.S) == 1 && x.Resource != 0).ToArray();

        public Cell[] GetVertexCells(Point point, int index)
        {
            return index switch
            {
                0 => new Cell[2] { GetCell(point.LeftUp()), GetCell(point.RightUp()) },
                1 => new Cell[2] { GetCell(point.RightUp()), GetCell(point.Right()) },
                2 => new Cell[2] { GetCell(point.Right()), GetCell(point.RightDown()) },
                3 => new Cell[2] { GetCell(point.RightDown()), GetCell(point.LeftDown()) },
                4 => new Cell[2] { GetCell(point.LeftDown()), GetCell(point.Left()) },
                5 => new Cell[2] { GetCell(point.Left()), GetCell(point.LeftUp()) },
                _ => Array.Empty<Cell>(),
            };
        }

        public Cell GetEdgeCells(Point point, int index, bool reverse = false)
        {
            if (!reverse)
            {
                return index switch
                {
                    0 => GetCell(point.RightUp()),
                    1 => GetCell(point.Right()),
                    2 => GetCell(point.RightDown()),
                    3 => GetCell(point.LeftDown()),
                    4 => GetCell(point.Left()),
                    5 => GetCell(point.LeftUp()),
                    _ => new(),
                };
            }
            return GetEdgeCells(point, index == 0 ? 5 : (index - 1), false);
        }

        public bool canSetSettlement(Point point, int index, Team team)
        {
            Cell[] cells = GetVertexCells(point, index);
            if (cells.Length == 0) { return false; }
            switch (index)
            {

            }
            // *
            return true;
        }
    }
}