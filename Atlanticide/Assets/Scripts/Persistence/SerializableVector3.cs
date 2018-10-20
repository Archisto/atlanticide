using System;
using UnityEngine;

namespace Atlanticide.Persistence
{
    [Serializable]
    public class SerializableVector3
    {
        public float X;
        public float Y;
        public float Z;

        public SerializableVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public SerializableVector3(Vector3 vector)
            : this(vector.x, vector.y, vector.z)
        {
        }

        public SerializableVector3(Vector2 vector)
            : this(vector.x, vector.y, 0)
        {
        }

        #region Casting

        public static implicit operator SerializableVector3(Vector3 v)
        {
            return new SerializableVector3(v);
        }

        public static implicit operator SerializableVector3(Vector2 v)
        {
            return new SerializableVector3(v);
        }

        public static implicit operator Vector3(SerializableVector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static explicit operator Vector2(SerializableVector3 v)
        {
            return new Vector2(v.X, v.Y);
        }

        #endregion Casting

        #region Operators

        public static SerializableVector3 operator +(
            SerializableVector3 a, SerializableVector3 b)
        {
            return new SerializableVector3(
                a.X + b.X,
                a.Y + b.Y,
                a.Z + b.Z);
        }

        public static SerializableVector3 operator +(
            SerializableVector3 a, Vector3 b)
        {
            return new SerializableVector3(
                a.X + b.x,
                a.Y + b.y,
                a.Z + b.z);
        }

        public static SerializableVector3 operator +(
            Vector3 a, SerializableVector3 b)
        {
            return b + a;
        }

        public static SerializableVector3 operator -(
            SerializableVector3 a, SerializableVector3 b)
        {
            return new SerializableVector3(
                a.X - b.X,
                a.Y - b.Y,
                a.Z - b.Z);
        }

        public static SerializableVector3 operator -(
            SerializableVector3 v)
        {
            return new SerializableVector3(
                -1 * v.X,
                -1 * v.Y,
                -1 * v.Z);
        }

        public static SerializableVector3 operator -(
            SerializableVector3 a, Vector3 b)
        {
            return new SerializableVector3(
                a.X - b.x,
                a.Y - b.y,
                a.Z - b.z);
        }

        public static SerializableVector3 operator -(
            Vector3 a, SerializableVector3 b)
        {
            return b - a;
        }

        #endregion Operators

        /// <summary>
        /// Every class in C# has ToString method and it can be overridden like this.
        /// </summary>
        /// <returns>String representation of the class</returns>
        public override string ToString()
        {
            return string.Format("X: {0}, Y: {1}, Z: {2}", X, Y, Z);
        }
    }
}
