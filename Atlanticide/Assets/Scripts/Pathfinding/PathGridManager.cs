using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class PathGridManager : MonoBehaviour
    {
        public LayerMask obstacleMask;
        public Vector2 gridSize;
        public float halfNodeWidth;
        private Vector3 gridOrigin;

        private Node[,] grid;

        private void Start()
        {
            gridOrigin = Vector3.zero;
            gridOrigin.x = transform.position.x - (gridSize.x / 2);
            gridOrigin.z = transform.position.z - (gridSize.y / 2);

            if (halfNodeWidth <= 0)
            {
                Debug.LogError("HalfNodeWidth must be larger than 0.");
                return;
            }

            int nodesX = (int) (gridSize.x / (2 * halfNodeWidth));
            int nodesY = (int) (gridSize.y / (2 * halfNodeWidth));

            grid = new Node[nodesX, nodesY];

            for (int i = 0; i < nodesX; i++)
            {
                for (int j = 0; j < nodesY; j++)
                {
                    Vector3 position = Vector3.zero;
                    position.x = gridOrigin.x +
                        (i * 2 * halfNodeWidth) + halfNodeWidth;
                    position.z = gridOrigin.z +
                        (j * 2 * halfNodeWidth) + halfNodeWidth;

                    bool blocked = Physics.CheckSphere(position, halfNodeWidth * 0.95f, obstacleMask);

                    grid[i, j] = new Node(blocked, position, i, j);
                }
            }

            foreach (Node node in grid)
            {
                node.neighbours = GetNeighbourNodes(node);
            }

            Debug.Log("Grid width: " + grid.GetLength(0));
        }

        public Node GetNode(int x, int y)
        {
            if (x >= 0 && x < grid.GetLength(0) &&
                y >= 0 && y < grid.GetLength(1))
            {
                return grid[x, y];
            }
            else
            {
                return null;
            }
        }

        public Node[] GetNeighbourNodes(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (!(x == 0 && y == 0))
                    {
                        Node newNode = GetNode(node.gridX + x, node.gridY + y);
                        if (newNode != null)
                        {
                            neighbours.Add(newNode);
                        }
                    }
                }
            }

            return neighbours.ToArray();
        }

        /// <summary>
        /// Gets the node which corresponds with the given position.
        /// </summary>
        /// <param name="position">a world position</param>
        /// <returns>a node in the position</returns>
        public Node GetNodeFromWorldPos(Vector3 position)
        {
            float x = (position.x - gridOrigin.x) / (2 * halfNodeWidth);
            float y = (position.z - gridOrigin.z) / (2 * halfNodeWidth);

            // Decreases negative numbers by 1
            // so they are rounded down
            if (x < 0)
            {
                x--;
            }
            if (y < 0)
            {
                y--;
            }

            // Changes x and y into integers
            int coordX = (int) x;
            int coordY = (int) y;

            // If the coordinates are within the grid,
            // the node in that position is returned
            if (coordX >= 0 && coordY >= 0 &&
                coordX < grid.GetLength(0) &&
                coordY < grid.GetLength(1))
            {
                return grid[coordX, coordY];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the grid's size.
        /// </summary>
        /// <returns>the grid's size</returns>
        public Vector2Int GetGridSize()
        {
            return new Vector2Int(grid.GetLength(0), grid.GetLength(1));
        }

        private void OnDrawGizmos()
        {
            DrawGridBorderGizmo();
            DrawNodeGizmos();
        }

        private void DrawGridBorderGizmo()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));
        }

        private void DrawNodeGizmos()
        {
            if (grid != null)
            {
                foreach (Node node in grid)
                {
                    float nodeHeight = (node.isBlocked ? 1 : 0);
                    node.DrawGizmo(2 * halfNodeWidth, nodeHeight);
                }
            }
        }

        //private void DrawGridMoverGizmos()
        //{
        //    if (gridMovers != null)
        //    {
        //        Gizmos.color = Color.blue;
        //        foreach (GridMover gridMover in gridMovers)
        //        {
        //            if (gridMover.node != null)
        //            {
        //                DrawNodeGizmo(gridMover.node.position, 1);
        //            }
        //        }
        //    }
        //}
    }
}
