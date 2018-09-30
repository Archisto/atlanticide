using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class SwitchSpawn : MonoBehaviour
    {

        // Switch that activates spawning
        [SerializeField]
        private Switch _switch;

        // Object to be spawned
        [SerializeField]
        private TimedPickup _object;

        // whether object is spawned multiple times or only once
        [SerializeField]
        protected bool _single_use;

        // is object spawned
        protected bool _spawned;

        

        // Use this for initialization
        void Start()
        {
            _spawned = false;
        }

        // Update is called once per frame
        void Update()
        {
            // if object is spawned and it has to be spawned only once, return
            if(_spawned && _single_use)
            {
                return;
            }

            //  if not spawned, and switch is activated, spawn
            if(!_spawned && _switch.Activated)
            {
                _object.ResetObject();
                _spawned = true;

                // if switch is no longer activated, reset _spawned status
            } else if (_spawned && !_switch.Activated)
            {
                _spawned = false;
            }

        }
    }
}