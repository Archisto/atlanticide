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
    }
}
