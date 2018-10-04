using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Controls the object's scale.
    /// </summary>
    public class Scale : MonoBehaviour
    {
        [SerializeField]
        private bool _keepRelativeScale = true;

        private Vector3 _defaultScale;

        public Vector3 DefaultScale
        {
            get { return _defaultScale; }
        }

        public Vector3 Difference
        {
            get { return transform.localScale - _defaultScale; }
        }

        public bool IsInvisible
        {
            get
            {
                return transform.localScale.x == 0f ||
                       transform.localScale.y == 0f ||
                       transform.localScale.z == 0f;
            }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _defaultScale = transform.localScale;
        }

        /// <summary>
        /// Adjusts the object's scale.
        /// </summary>
        /// <param name="x">X-scale adjustment</param>
        /// <param name="x">Y-scale adjustment</param>
        /// <param name="x">Z-scale adjustment</param>
        public void AdjustScale(float x, float y, float z)
        {
            SetScale(new Vector3(
                transform.localScale.x + x,
                transform.localScale.y + y,
                transform.localScale.z + z));
        }

        /// <summary>
        /// Sets the object's scale.
        /// </summary>
        /// <param name="x">The x-scale</param>
        /// <param name="x">The y-scale</param>
        /// <param name="x">The z-scale</param>
        public void SetScale(float x, float y, float z)
        {
            SetScale(new Vector3(x, y, z));
        }

        /// <summary>
        /// Sets the object's scale.
        /// </summary>
        public void SetScale(Vector3 scale)
        {
            if (_keepRelativeScale)
            {
                float differenceX = scale.x - _defaultScale.x;
                scale = new Vector3(
                    scale.x,
                    _defaultScale.y + differenceX,
                    _defaultScale.z + differenceX);
            }

            transform.localScale = scale;
        }

        /// <summary>
        /// Multiplies the object's scale by <paramref name="factor"/>.
        /// </summary>
        /// <param name="factor">Scale factor</param>
        public void MultiplyScale(float factor)
        {
            transform.localScale = _defaultScale * factor;
        }

        /// <summary>
        /// Resets the object's scale back to the default.
        /// </summary>
        public void ResetScale()
        {
            transform.localScale = _defaultScale;
        }
    }
}
