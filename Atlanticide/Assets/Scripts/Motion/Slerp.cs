using UnityEngine;
using System.Collections;

namespace Atlanticide
{
    public class Slerp : MonoBehaviour
    {
        public Vector3 center;
        public float radius;
        private Vector3 ascendPos;
        private Vector3 descendPos;
        public float orbitTime = 1f;
        public Vector3 centerOffset = new Vector3(0, -1, 0);
        private float elapsedTime;
        private bool reverse;
        public bool lerp;
        public bool addCenterToPos = true;

        private Vector3 ascendRelCenter;
        private Vector3 descendRelCenter;

        void Start()
        {
            ascendPos = center + (transform.position - center).normalized * radius;
            descendPos = center - (transform.position - center).normalized * radius;
        }

        void Update()
        {
            // Center determines the object's movevent arc
            center = (ascendPos + descendPos) * 0.5f; // Halfway between the two positions
            center += centerOffset; // Offset of Vector3.zero makes an arc around the y-axis by default

            ascendRelCenter = ascendPos - center; // Direction vector from center to ascendPos
            descendRelCenter = descendPos - center; // Direction vector from center to descendPos

            Vector3 start = ascendRelCenter;
            Vector3 target = descendRelCenter;

            elapsedTime += World.Instance.DeltaTime;
            float ratio = elapsedTime / (orbitTime * 0.5f);

            if (reverse)
            {
                start = descendRelCenter;
                target = ascendRelCenter;
            }

            transform.position = LerpOrSlerp(start, target, ratio); // Slerp creates an arc around the center

            if (addCenterToPos)
            {
                transform.position += center; // Levels the whole arc neatly with the center
            }

            if (ratio >= 1)
            {
                //Debug.Log("Reached");
                reverse = !reverse;
                centerOffset = -1f * centerOffset; // Makes the object repeat the arc but on the other side of the center
                elapsedTime = 0f;
            }
        }

        private Vector3 LerpOrSlerp(Vector3 start, Vector3 target, float t)
        {
            if (lerp)
            {
                return Vector3.Lerp(start, target, t);
            }
            else
            {
                return Vector3.Slerp(start, target, t);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(center, transform.position);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(ascendPos, descendPos);
        }
    }
}