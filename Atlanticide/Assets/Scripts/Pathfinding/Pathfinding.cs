using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Pathfinding : MonoBehaviour
    {
        private PathGridManager grid;
        private int totalNodes;

        /// <summary>
        /// Use this for initialization.
        /// </summary>
        private void Start()
        {
            grid = FindObjectOfType<PathGridManager>();

            if (grid == null)
            {
                Debug.LogError("Could not find a PathGridManager in the scene.");
            }
            else
            {
                // Gets the total number of nodes in the grid
                totalNodes = grid.GetGridSize().x * grid.GetGridSize().y;
            }
        }

        /// <summary>
        /// Finds a path from start to end.
        /// </summary>
        /// <param name="start">a start position</param>
        /// <param name="target">a target position</param>
        /// <returns></returns>
        public List<Node> FindPath(Vector3 start, Vector3 target)
        {
            Heap<Node> openSet = new Heap<Node>(totalNodes);
            List<Node> closedSet = new List<Node>();
            List<Node> path = new List<Node>();

            Node startNode = grid.GetNodeFromWorldPos(start);
            Node targetNode = grid.GetNodeFromWorldPos(target);

            // Returns an empty path if either of the ends is invalid
            if (startNode == null || targetNode == null ||
                startNode.isBlocked || targetNode.isBlocked)
            {
                return path;
            }

            startNode.gCost = 0;
            startNode.hCost = GetDistance(startNode, targetNode);
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);
                currentNode.isInClosedSet = true;

                if (currentNode == targetNode)
                {
                    path = RetracePath(startNode, currentNode);
                    break;
                }
                else
                {
                    foreach (Node neighbourNode in currentNode.neighbours)
                    {
                        if (!neighbourNode.isBlocked &&
                            !neighbourNode.isInClosedSet)
                        {
                            int newMovementCost =
                                currentNode.gCost +
                                GetDistance(currentNode, neighbourNode);

                            bool openListContainsNeighbour =
                                openSet.Contains(neighbourNode);

                            if (newMovementCost < neighbourNode.gCost ||
                                !openListContainsNeighbour)
                            {
                                neighbourNode.gCost = newMovementCost;
                                neighbourNode.hCost =
                                    GetDistance(neighbourNode, targetNode);
                                neighbourNode.prevNode = currentNode;

                                if (!openListContainsNeighbour)
                                {
                                    openSet.Add(neighbourNode);
                                }
                                else
                                {
                                    openSet.UpdateItem(neighbourNode);
                                }
                            }
                        }
                    }
                }
            }

            ResetClosedSet(closedSet);

            return path;
        }

        /// <summary>
        /// Gets the path tracing back from the last node, reverses it and returns it.
        /// </summary>
        /// <param name="start">the path's first node</param>
        /// <param name="target">the path's last node</param>
        /// <returns>path from start to target</returns>
        private List<Node> RetracePath(Node start, Node target)
        {
            List<Node> path = new List<Node>();

            Node currentNode = target;

            while (currentNode != start)
            {
                path.Add(currentNode);
                currentNode = currentNode.prevNode;
            }

            path.Reverse();

            return path;
        }

        private int GetDistance(Node start, Node target)
        {
            int x = System.Math.Abs(target.gridX - start.gridX);
            int y = System.Math.Abs(target.gridY - start.gridY);

            if (x > y)
            {
                return 14 * y + 10 * (x - y);
            }
            else
            {
                return 14 * x + 10 * (y - x);
            }
        }

        /// <summary>
        /// Removes every node from the given closed set.
        /// </summary>
        /// <param name="closedSet">set of checked nodes</param>
        private void ResetClosedSet(List<Node> closedSet)
        {
            foreach (Node node in closedSet)
            {
                node.isInClosedSet = false;
            }
        }
    }
}
