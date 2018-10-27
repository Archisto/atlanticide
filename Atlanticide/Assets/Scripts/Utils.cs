using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Atlanticide.Persistence;

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

        /// <summary>
        /// Returns string "{objectName} is not set."
        /// </summary>
        /// <param name="obj">An object</param>
        /// <returns>A string</returns>
        public static string GetFieldNullString(string obj)
        {
            return string.Format("{0} is not set.", obj);
        }

        /// <summary>
        /// Returns string
        /// "An instance of {objectName} could not be found in the scene."
        /// </summary>
        /// <param name="obj">An object</param>
        /// <returns>A string</returns>
        public static string GetObjectMissingString(string obj)
        {
            return string.Format("An instance of {0} could not be found in the scene.", obj);
        }

        /// <summary>
        /// Returns string
        /// "Component {componentName} could not be found in the object."
        /// </summary>
        /// <param name="comp">A component</param>
        /// <returns>A string</returns>
        public static string GetComponentMissingString(string comp)
        {
            return string.Format("Component {0} could not be found in the object.", comp);
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

        public static void TrySetData<TSavable, TSaveData>(TSavable[] savables,
                                                           List<TSaveData> dataList,
                                                           bool checkID)
            where TSavable : ISavable
            where TSaveData : ISaveData
        {
            if (savables.Length == 0 || dataList.Count == 0 ||
                savables.Length != dataList.Count)
            {
                Debug.LogWarning(string.Format
                    ("Savables: {0}. Data list count: {1}.",
                    savables.Length,
                    dataList.Count));
                return;
            }
            else if (savables[0].GetSaveDataType() != typeof(TSaveData))
            {
                Debug.LogError(string.Format
                    ("Incompatible save data types: {0} and {1}",
                    savables[0].GetSaveDataType(),
                    typeof(TSaveData)));
                return;
            }

            for (int i = 0; i < dataList.Count; i++)
            {
                TSavable savable = savables[i];
                ISaveData data = dataList[i];

                if (checkID)
                {
                    savable = savables.FirstOrDefault
                        (sav => sav.ID == data.ID);

                    if (savable != null)
                    {
                        savable.SetData(data);
                    }
                }
                else
                {
                    savable.SetData(data);
                }
            }
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

        /// <summary>
        /// Returns how many values does an enumerator have.
        /// </summary>
        /// <param name="enumType">An enum type</param>
        /// <returns>The length of the enum</returns>
        public static int GetEnumLength(Type enumType)
        {
            if (enumType == typeof(Enum))
            {
                return Enum.GetValues(enumType).Length;
            }
            else
            {
                return -1;
            }
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        public static bool Between(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        public static float DistanceTo(this MonoBehaviour obj,
                                       MonoBehaviour target)
        {
            return Vector3.Distance
                (obj.transform.position, target.transform.position);
        }

        public static bool WithinRangeBox(Vector3 position,
                                          Vector3 boxCorner1,
                                          Vector3 boxCorner2)
        {
            return Between(position.x, boxCorner1.x, boxCorner2.x) &&
                   Between(position.y, boxCorner1.y, boxCorner2.y) &&
                   Between(position.z, boxCorner1.z, boxCorner2.z);
        }

        public static void DrawBoxGizmo(Vector3 boxCorner1, Vector3 boxCorner2)
        {
            Vector3 p1 = new Vector3(boxCorner2.x, boxCorner1.y, boxCorner1.z);
            Vector3 p2 = new Vector3(boxCorner1.x, boxCorner2.y, boxCorner1.z);
            Vector3 p3 = new Vector3(boxCorner1.x, boxCorner1.y, boxCorner2.z);
            Vector3 p4 = new Vector3(boxCorner2.x, boxCorner2.y, boxCorner1.z);
            Vector3 p5 = new Vector3(boxCorner2.x, boxCorner1.y, boxCorner2.z);
            Vector3 p6 = new Vector3(boxCorner1.x, boxCorner2.y, boxCorner2.z);
            Gizmos.DrawLine(boxCorner1, p1);
            Gizmos.DrawLine(boxCorner1, p2);
            Gizmos.DrawLine(boxCorner1, p3);
            Gizmos.DrawLine(p1, p4);
            Gizmos.DrawLine(p1, p5);
            Gizmos.DrawLine(p2, p4);
            Gizmos.DrawLine(p2, p6);
            Gizmos.DrawLine(p3, p5);
            Gizmos.DrawLine(p3, p6);
            Gizmos.DrawLine(p4, boxCorner2);
            Gizmos.DrawLine(p5, boxCorner2);
            Gizmos.DrawLine(p6, boxCorner2);
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

        public static void DrawDotGizmos(Vector3 position, int dots, int maxDots, Color color)
        {
            Gizmos.color = color;
            float spacing = 0.8f;

            float lineLength = 2f;
            Vector3 lineStart = position +
                new Vector3(-0.5f * lineLength * spacing, -0.4f * spacing, 0); 
            Gizmos.DrawLine(lineStart, lineStart + Vector3.right * lineLength * spacing);
        
            position.x -= 0.5f * (maxDots - 1) * spacing;
            for (int i = 0; i < dots; i++)
            {
                Gizmos.DrawSphere(position, 0.2f);
                position.x += spacing;
            }
        }

        [Serializable]
        public struct IntVector2
        {
            public IntVector2(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
            public int x, y;
        }

    }
}
