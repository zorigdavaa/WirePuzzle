using System.Collections.Generic;

class PathState
{
    public GridNode Node;
    public ulong Mask;
    public PathState Parent;

    public PathState(GridNode node, ulong mask, PathState parent)
    {
        Node = node;
        Mask = mask;
        Parent = parent;
    }
}

