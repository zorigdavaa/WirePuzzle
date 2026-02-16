using System.Collections.Generic;

class PathState
{
    public GridNode Node;
    public HashSet<(int,int)> Occupied;
    public PathState Parent;

    public PathState(GridNode node, HashSet<(int,int)> occ, PathState parent)
    {
        Node = node;
        Occupied = occ;
        Parent = parent;
    }
}
