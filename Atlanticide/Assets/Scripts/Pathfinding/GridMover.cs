using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class GridMover : MonoBehaviour
    {
        public PathGridManager pathGridManager;
        public Node node;

        protected virtual void Update()
        {
            node = pathGridManager.GetNodeFromWorldPos(transform.position);
        }

        protected virtual void OnDrawGizmos()
        {
            if (node != null)
            {
                node.DrawGizmo(2 * pathGridManager.halfNodeWidth, 1f, Color.green);
            }
        }
    }
}
