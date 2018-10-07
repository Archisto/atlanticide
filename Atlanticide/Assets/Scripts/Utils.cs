using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public static class Utils
    {
        public enum Axis
        {
            X,
            Y,
            Z
        }

        public static string GetFieldNullString(string obj)
        {
            return string.Format("{0} is not set.", obj);
        }

        public static string GetObjectMissingString(string obj)
        {
            return string.Format("An instance of {0} could not be found in the scene.", obj);
        }

        public static string GetComponentMissingString(string comp)
        {
            return string.Format("Component {0} could not be found in the object.", comp);
        }

        public static float DrainOrRecharge(float value,
                                            bool drain,
                                            float drainSpeed,
                                            float rechargeSpeed,
                                            float minRecharge,
                                            bool depletedBefore,
                                            out bool depleted)
        {
            depleted = depletedBefore;

            // Drain
            if (drain)
            {
                depleted = false;

                value -= drainSpeed * World.Instance.DeltaTime;
                if (value <= 0)
                {
                    value = 0;
                    depleted = true;
                }
            }
            // Recharge
            else if (value < 1)
            {
                value += rechargeSpeed * World.Instance.DeltaTime;

                if (depleted && value >= minRecharge)
                {
                    depleted = false;
                }
                if (value > 1)
                {
                    value = 1;
                }
            }

            return value;
        }

        public static object GetFirstActiveOrInactiveObject(object[] array, bool active)
        {
            if (array.Length == 0)
            {
                return null;
            }

            bool objIsGameObject = array[0] is GameObject;

            foreach (object obj in array)
            {
                GameObject go = null;
                if (objIsGameObject)
                {
                    go = obj as GameObject;
                }
                else
                {
                    Component c = obj as Component;
                    if (c != null)
                    {
                        go = c.gameObject;
                    }
                }

                if (go != null && go.activeSelf == active)
                {
                    return obj;
                }
            }

            return null;
        }

        public static float Ratio(float value, float lowBound, float highBound)
        {
            if (value <= lowBound)
            {
                return 0;
            }
            else if (value >= highBound)
            {
                return 1;
            }
            else
            {
                return ((value - lowBound) / (highBound - lowBound));
            }
        }

        public static float ReverseRatio(float value, float lowBound, float highBound)
        {
            return (1 - Ratio(value, lowBound, highBound));
        }

        public static float WeighValue(float value, float heavyValue, float amount)
        {
            return value + (heavyValue - value) * amount;
        }

        /// <summary>
        /// Invokes an action on each item of the array.
        /// </summary>
        /// <param name="array">An array</param>
        /// <param name="action">An action</param>
        /// <typeparam name="T">A type</typeparam>
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            foreach (T obj in array)
            {
                action(obj);
            }
        }

        /// <summary>
        /// Sets the value of the preference identified by key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, (value ? 1 : 0));
        }

        /// <summary>
        /// Returns the value corresponding to the key in the preference file if it exists.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="defaultValue">The default value</param>
        public static bool GetBool(string key, bool defaultValue)
        {
            int value = PlayerPrefs.GetInt(key, (defaultValue ? 1 : 0));
            return (value == 1);
        }

        public static bool AddIfNew<T>(this List<T> list, T itemToAdd)
        {
            foreach (T item in list)
            {
                if (item.Equals(itemToAdd))
                {
                    return false;
                }
            }

            list.Add(itemToAdd);
            return true;
        }

        public static Vector3 GetRotationOnAxis(Axis axis, float angle)
        {
            Vector3 result = Vector3.zero;

            switch (axis)
            {
                case Axis.X:
                {
                    result.x = angle;
                    break;
                }
                case Axis.Y:
                {
                    result.y = angle;
                    break;
                }
                case Axis.Z:
                {
                    result.z = angle;
                    break;
                }
            }

            return result;
        }

        public static float GetAngleOnAxis(Axis axis, Vector3 rotation)
        {
            switch (axis)
            {
                case Axis.X:
                {
                    return rotation.x;
                }
                case Axis.Y:
                {
                    return rotation.y;
                }
                case Axis.Z:
                {
                    return rotation.z;
                }
            }

            return 0;
        }

        public static Vector3 GetCurvePoint(Vector3 p0, Vector3 p1, Vector3 p2,
                                            float t)
        {
            // Bézier curve:
            // B(t) = (1 - t) * [(1 - t) * p0 + t * p1] + t * [(1 - t) * p1 + t * p2]
            // 0 <= t <= 1

            return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
        }

        public static Vector3[] Get4CardinalDirections()
        {
            Vector3[] cardinalDirs = new Vector3[4];
            cardinalDirs[0] = Vector3.forward;
            cardinalDirs[1] = Vector3.back;
            cardinalDirs[2] = Vector3.left;
            cardinalDirs[3] = Vector3.right;
            return cardinalDirs;
        }

        public static Quaternion RotateTowards(Quaternion currentRotation, Vector3 direction, float turningSpeed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            Quaternion newRotation = Quaternion.Lerp(currentRotation, targetRotation, turningSpeed);
            return newRotation;
        }

        public static void DrawProgressBarGizmo(Vector3 position, float progress, Color barColor, Color indicatorColor)
        {
            Gizmos.color = barColor;
            float length = 2;
            float height = 0.5f;
            Vector3 pos1 = position + Vector3.right * -0.5f * length;
            Vector3 pos2 = position + Vector3.right * 0.5f * length;
            Vector3 pos3 = position + new Vector3(length * (progress - 0.5f), 0.5f * height, 0);
            Vector3 pos4 = position + new Vector3(length * (progress - 0.5f), -0.5f * height, 0);
            Gizmos.DrawLine(pos1, pos2);

            Gizmos.color = indicatorColor;
            Gizmos.DrawLine(pos3, pos4);
        }
    }
}
