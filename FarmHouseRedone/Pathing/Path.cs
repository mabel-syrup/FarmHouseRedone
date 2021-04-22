using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using StardewValley;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.Pathing
{
    public class Path
    {
        public int width;
        public int height;
        public GameLocation location;

        public PathingGrid grid;

        public Path(GameLocation location, int width = -1, int height = -1)
        {
            if (width == -1)
                width = location.map.GetLayer("Back").LayerWidth;
            if (height == -1)
                height = location.map.GetLayer("Back").LayerHeight;
            this.location = location;
            this.width = width;
            this.height = height;
            grid = new PathingGrid(location, width, height);
        }

        public List<Node> FindPath(Vector2 start, Vector2 end)
        {
            var startNode = grid.getNode(start);
            var endNode = grid.getNode(end);

            var openSet = new List<Node>();
            var closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet[0];
                for (var node = 0; node < openSet.Count; node++)
                    if (openSet[node].fCost < currentNode.fCost || openSet[node].fCost == currentNode.fCost &&
                        openSet[node].hCost < currentNode.hCost)
                        currentNode = openSet[node];

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == endNode)
                {
                    Logger.Log("Found a path to " + endNode.position.ToString() + "!");
                    return retracePath(startNode, endNode);
                }

                foreach (var neighbor in grid.getNeighbors(currentNode))
                {
                    if (!neighbor.traversible || closedSet.Contains(neighbor))
                        continue;
                    var newMoveCost = currentNode.gCost + getDistance(currentNode, neighbor) +
                                      (int) (neighbor.weightCost * 10);
                    if (newMoveCost < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newMoveCost;
                        neighbor.hCost = getDistance(neighbor, endNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            Logger.Log("Failed to find a path to " + endNode.position.ToString() + "!",
                StardewModdingAPI.LogLevel.Error);
            return new List<Node>();
        }

        internal List<Node> retracePath(Node start, Node end)
        {
            var path = new List<Node>();
            var currentNode = end;
            while (currentNode != start)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            return path;
        }

        public int getDistance(Node a, Node b)
        {
            return Math.Abs(b.x - a.x + (b.y - a.y));
        }
    }
}