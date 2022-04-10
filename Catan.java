enum Resource {
    Wood,
    Brick,
    Sheep,
    Wheat,
    Ore,
    Desert,
    Ocean
}

enum Vertex {
    Settlement,
    City
}

enum Edge {
    Road,
    Ship
}

enum Team {
    Red,
    Blue,
    Yellow,
    White
}

class Cell {
    public Resource resource;
    public int number;

    public Cell(Resource resource, int number) {
        this.resource = resource;
        this.number = number;
    }

    @Override
    public String toString() {
        return resource.toString() + " " + number;
    }

    public Vertex[] vertexes = new Vertex[6];
    public Team[] vertexTeams = new Team[6];

    public Edge[] edges = new Edge[6];
    public Team[] edgeTeams = new Team[6];

    public boolean isThieves;
    public boolean isPirate;

    private static int minusNumber(int value) {
        return value == 0 ? 5 : value - 1;
    }

    private static int plusNumber(int value) {
        return value == 5 ? 0 : value + 1;
    }

    public boolean canSetSettlement(int index, Team team) {
        if (vertexes[index] != null) {
            return false;
        }
        if ((vertexes[minusNumber(index)] == null && vertexes[plusNumber(index)] == null)
                && (edgeTeams[minusNumber(index)] == team || edgeTeams[plusNumber(index)] == team)) {
            if (isThieves) {
                return false;
            }
            return true;
        }
        return false;
    }

    public boolean canSetEdge(int index, Team team, Edge edge) {
        if (edges[index] != null) {
            return false;
        }
        if (vertexTeams[minusNumber(index)] == team || vertexTeams[plusNumber(index)] == team
                || (edgeTeams[minusNumber(index)] == team
                        && (vertexTeams[minusNumber(index)] == team || vertexTeams[minusNumber(index)] == null)
                        || (edgeTeams[plusNumber(index)] == team && (vertexTeams[plusNumber(index)] == team
                                || vertexTeams[plusNumber(index)] == null)))) {
            if (vertexTeams[minusNumber(index)] == null && vertexTeams[plusNumber(index)] == null
                    && edges[minusNumber(index)] != edge && edges[plusNumber(index)] != edge) {
                return false;
            }
            if (isThieves && edge == Edge.Road) {
                return false;
            }
            if (isPirate && edge == Edge.Ship) {
                return false;
            }
            return true;
        }
        return false;
    }

    public boolean canSetCity(int index, Team team) {
        return (vertexes[index] == Vertex.Settlement && vertexTeams[index] == team);
    }

    public void setSettlement(int index, Team team) {
        if (canSetSettlement(index, team)) {
            vertexes[index] = Vertex.Settlement;
            vertexTeams[index] = team;
        }
    }

    public void setEdge(int index, Team team, Edge edge) {
        if (canSetEdge(index, team, edge)) {
            edges[index] = edge;
            edgeTeams[index] = team;
        }
    }

    public void setCity(int index, Team team) {
        if (canSetCity(index, team)) {
            vertexes[index] = Vertex.City;
        }
    }
}

class Board {
    public int width;
    public Cell[][] cells;

    public Board(int width) {
        this.width = width;
        cells = new Cell[width * 2 - 1][width * 2 - 1];
    }

    private Cell getCell(int x, int y) {
        return cells[x][(x % 2 == 0) ? (y + 1) * 2 : (y + 1) * 2 - 1];
    }

    public Cell[] getRoundedCells(int x, int y) {
        int count = 6;
        if (x == 0) {
            count--;
        }
        if (x == width * 2 - 2) {
            count--;
        }
        if (y == 0) {
            count -= 2;
        }
        if (y == width * 2 - 2) {
            count -= 2;
        }
        Cell[] cells = new Cell[count];
        int index = 0;
        if (x != 0) {
            cells[index] = getCell(x - 1, y);
            index++;
        }
        if (x != width * 2 - 2) {
            cells[index] = getCell(x + 1, y);
            index++;
        }
        if (y != 0) {
            cells[index] = getCell(x + 1, y - 1);
            index++;
            cells[index] = getCell(x - 1, y - 1);
            index++;
        }
        if (y != width * 2 - 2) {
            cells[index] = getCell(x + 1, y + 1);
            index++;
            cells[index] = getCell(x - 1, y + 1);
            index++;
        }
        return cells;
    }

    public boolean canSetSettlement(int x, int y, int index, Team team) {
        Cell[] roundedCells = getRoundedCells(x, y);
        boolean isOceans = true;
        for (Cell cell : roundedCells) {
            if (cell.resource == null) {
                continue;
            }
            if (cell.resource != Resource.Ocean) {
                isOceans = false;
            }
            if (!cell.canSetSettlement(index, team)) {
                return false;
            }
        }
        return isOceans;
    }

    public boolean canSetEdge(int x, int y, int index, Team team, Edge edge) {
        Cell[] roundedCells = getRoundedCells(x, y);
        boolean isOceans = true;
        for (Cell cell : roundedCells) {
            if (cell.resource == null) {
                continue;
            }
            if (cell.resource != Resource.Ocean) {
                isOceans = false;
            }
            if (!cell.canSetEdge(index, team, edge)) {
                return false;
            }
        }
        return isOceans;
    }

    public boolean canSetCity(int x, int y, int index, Team team) {
        Cell[] roundedCells = getRoundedCells(x, y);
        for (Cell cell : roundedCells) {
            if (cell.resource == null) {
                continue;
            }
            if (!cell.canSetCity(index, team)) {
                return false;
            }
        }
        return true;
    }
}