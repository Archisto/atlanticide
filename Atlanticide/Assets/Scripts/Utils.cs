using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public static class Utils
    {
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

                value -= drainSpeed * Time.deltaTime;
                if (value <= 0)
                {
                    value = 0;
                    depleted = true;
                }
            }
            // Recharge
            else if (value < 1)
            {
                value += rechargeSpeed * Time.deltaTime;

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
    }
}
