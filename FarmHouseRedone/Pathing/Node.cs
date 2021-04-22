using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace FarmHouseRedone.Pathing
{
    public class Node
    {
        public Vector2 position;
        public bool traversible;

        public Node parent;

        public float weightCost;
        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;
        public int x => (int) position.X;

        public int y => (int) position.Y;

        public Node(Vector2 position, bool traversible)
        {
            this.position = position;
            this.traversible = traversible;
        }
    }
}