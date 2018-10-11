using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Node : IHeapItem<Node>
    {
        public bool isBlocked;
        public bool isInClosedSet;
        public Vector3 position;

        public int gridX, gridY;

        /// <summary>
        /// The distance from start to this node
        /// </summary>
        public int gCost;

        /// <summary>
        /// The distance from this node to target
        /// </summary>
        public int hCost;

        /// <summary>
        /// The distances from start to this node
        /// and from this node to target combined.
        /// </summary>
        public int FCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public Node prevNode;
        public Node[] neighbours;

        private int heapIndex;

        public Node(bool isBlocked, Vector3 position, int gridX, int gridY)
        {
            this.isBlocked = isBlocked;
            this.position = position;

            this.gridX = gridX;
            this.gridY = gridY;

            neighbours = new Node[8];
        }

        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }

            set
            {
                heapIndex = value;
            }
        }

        public int CompareTo(Node node)
        {
            int comp = FCost.CompareTo(node.FCost);
            if (comp == 0)
            {
                comp = hCost.CompareTo(node.hCost);
            }
            return ~comp;
        }

        private Color NodeColor
        {
            get
            {
                return (isBlocked ? Color.red : Color.white);
            }
        }

        public void DrawGizmo(float nodeWidth, float height)
        {
            Gizmos.color = NodeColor;
            Gizmos.DrawWireCube(new Vector3(position.x, height / 2, position.z),
                new Vector3(nodeWidth, height, nodeWidth));
        }

        public void DrawGizmo(float nodeWidth, float height, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(new Vector3(position.x, height / 2, position.z),
                new Vector3(nodeWidth, height, nodeWidth));
        }
    }
}
