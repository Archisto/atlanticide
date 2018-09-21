using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public static class Utils
    {
        public static string GetObjectMissingString(string obj)
        {
            return string.Format("An instance of {0} could not be found in the scene.", obj);
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
    }
}
