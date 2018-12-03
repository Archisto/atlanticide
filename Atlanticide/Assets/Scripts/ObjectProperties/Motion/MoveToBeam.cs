using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Makes the object move to link beam.
    /// </summary>
    public class MoveToBeam
    {
        public float speed;
        public Vector3 position;
        public Vector3 targetPosition;
        public bool isMoving;

        private LinkBeam linkBeam;
        Vector3 rangeMin;
        Vector3 rangeMax;

        public MoveToBeam(float speed, float rangeCubeRadius)
        {
            this.speed = speed;
            rangeMin = new Vector3(-1 * rangeCubeRadius,
                                   -1 * rangeCubeRadius,
                                   -1 * rangeCubeRadius);
            rangeMax = new Vector3(rangeCubeRadius,
                                   rangeCubeRadius,
                                   rangeCubeRadius);
        }

        public void Update()
        {
            if (isMoving)
            {
                Move();
            }
        }

        /// <summary>
        /// Moves the object.
        /// </summary>
        /// <returns>New position</returns>
        public Vector3 Move()
        {
            if (isMoving && linkBeam != null)
            {
                Vector3 pointOnBeam = linkBeam.BeamCenter +
                    Vector3.Project(position - linkBeam.BeamCenter,
                    (linkBeam.transform.position - linkBeam.BeamCenter));
                Vector3 direction = pointOnBeam - position;
                Vector3 newPosition = position + direction * speed * World.Instance.DeltaTime;
                if (Utils.WithinRangeBox(pointOnBeam, newPosition + rangeMin, newPosition + rangeMax))
                {
                    position = pointOnBeam;
                    StopMoving();
                }
                else
                {
                    position = newPosition;
                }
            }

            return position;
        }

        /// <summary>
        /// Moves the object.
        /// </summary>
        /// <param name="currentPosition">The current position</param>
        /// <returns>New position</returns>
        public Vector3 Move(Vector3 currentPosition)
        {
            position = currentPosition;
            return Move();
        }

        /// <summary>
        /// Starts moving the object.
        /// </summary>
        public void StartMoving(LinkBeam linkBeam, Vector3 currentPosition)
        {
            this.linkBeam = linkBeam;
            position = currentPosition;
            isMoving = true;
        }

        /// <summary>
        /// Stops moving the object.
        /// </summary>
        public void StopMoving()
        {
            linkBeam = null;
            isMoving = false;
        }
    }
}
