using System;
using System.Collections.Generic;
using System.Linq;
namespace ImperiumAO.Server.Systems.NPCs;

public class PathNode : IComparable<PathNode>
{
    public int X { get; set; }
    public int Y { get; set; }
    public float GCost { get; set; }
    public float HCost { get; set; }
    public float FCost => GCost + HCost;
    public PathNode? Parent { get; set; }

    public PathNode(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int CompareTo(PathNode? other)
    {
        return FCost.CompareTo(other?.FCost ?? 0);
    }

    public float DistanceTo(int x, int y)
    {
        return (float)Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2));
    }

    public override bool Equals(object? obj)
    {
        return obj is PathNode node && X == node.X && Y == node.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}

public interface IPathfinding
{
    List<(int X, int Y)> FindPath(int startX, int startY, int endX, int endY, int maxDistance = 50);
}

public class AStarPathfinding : IPathfinding
{
    public List<(int X, int Y)> FindPath(int startX, int startY, int endX, int endY, int maxDistance = 50)
    {
        var openSet = new SortedSet<PathNode> { new(startX, startY) { GCost = 0, HCost = DistanceHeuristic(startX, startY, endX, endY) } };
        var closedSet = new HashSet<(int, int)>();
        var nodeMap = new Dictionary<(int, int), PathNode>();

        nodeMap[(startX, startY)] = openSet.First();

        while (openSet.Count > 0)
        {
            var current = openSet.Min;
            openSet.Remove(current);

            if (current.X == endX && current.Y == endY)
            {
                return ReconstructPath(current);
            }

            closedSet.Add((current.X, current.Y));

            foreach (var neighbor in GetNeighbors(current.X, current.Y))
            {
                if (closedSet.Contains((neighbor.X, neighbor.Y)))
                    continue;

                var tentativeGCost = current.GCost + current.DistanceTo(neighbor.X, neighbor.Y);

                if (nodeMap.TryGetValue((neighbor.X, neighbor.Y), out var neighborNode))
                {
                    if (tentativeGCost >= neighborNode.GCost)
                        continue;

                    openSet.Remove(neighborNode);
                }

                neighborNode = new(neighbor.X, neighbor.Y)
                {
                    GCost = tentativeGCost,
                    HCost = DistanceHeuristic(neighbor.X, neighbor.Y, endX, endY),
                    Parent = current
                };

                nodeMap[(neighbor.X, neighbor.Y)] = neighborNode;
                openSet.Add(neighborNode);

                if (neighborNode.GCost > maxDistance)
                    openSet.Remove(neighborNode);
            }
        }

        return new();
    }

    private List<PathNode> GetNeighbors(int x, int y)
    {
        var neighbors = new List<PathNode>
        {
            new(x + 1, y),
            new(x - 1, y),
            new(x, y + 1),
            new(x, y - 1),
            new(x + 1, y + 1),
            new(x + 1, y - 1),
            new(x - 1, y + 1),
            new(x - 1, y - 1)
        };
        return neighbors;
    }

    private float DistanceHeuristic(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }

    private List<(int X, int Y)> ReconstructPath(PathNode? node)
    {
        var path = new List<(int X, int Y)>();
        while (node != null)
        {
            path.Add((node.X, node.Y));
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }
}

