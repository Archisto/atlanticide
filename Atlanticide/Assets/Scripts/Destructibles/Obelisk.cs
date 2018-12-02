using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Obelisk : MonoBehaviour
    {
        public GameObject obeliskIntact,
            obeliskBroken,
            obeliskBaseBroken;

        public ObeliskDirectionalCollider[] obeliskDirectionalColliders;

        private Rigidbody obeliskBrokenRigidBody;

        public Vector3 westTorque,
            eastTorque,
            northTorque,
            southTorque;

        public ForceMode forceMode;

        private bool _falling = false;

        private ObeliskDirectionalCollider.Type _fallingDirection;

        public ParticleSystem dustParticleSystem;

        // Use this for initialization
        void Start()
        {
            obeliskBrokenRigidBody = obeliskBroken.GetComponent<Rigidbody>();
            obeliskBrokenRigidBody.centerOfMass = obeliskBroken.transform.localPosition;
        }

        private void Update()
        {
            if (_falling)
            {
                switch (_fallingDirection)
                {
                    case ObeliskDirectionalCollider.Type.West:
                        //Debug.Log("Appplying west torque.");
                        obeliskBrokenRigidBody.AddRelativeTorque(westTorque, forceMode);
                        break;
                    case ObeliskDirectionalCollider.Type.East:
                        //Debug.Log("Appplying east torque.");
                        obeliskBrokenRigidBody.AddRelativeTorque(eastTorque, forceMode);
                        break;
                    case ObeliskDirectionalCollider.Type.North:
                        //Debug.Log("Appplying north torque.");
                        obeliskBrokenRigidBody.AddRelativeTorque(northTorque, forceMode);
                        break;
                    case ObeliskDirectionalCollider.Type.South:
                        //Debug.Log("Appplying south torque.");
                        obeliskBrokenRigidBody.AddRelativeTorque(southTorque, forceMode);
                        break;
                }
            }
        }

        public void ObeliskDirectionalColliderCollision (ObeliskDirectionalCollider.Type type)
        {
            obeliskIntact.SetActive(false);
            obeliskBroken.SetActive(true);
            obeliskBaseBroken.SetActive(true);
            _falling = true;
            _fallingDirection = type;
            dustParticleSystem.gameObject.SetActive(true);
            foreach (ObeliskDirectionalCollider odc in obeliskDirectionalColliders)
            {
                odc.gameObject.SetActive(false);
            }
        }

        public void StopFallingObelisk()
        {
            _falling = false;
            obeliskBrokenRigidBody.velocity = Vector3.zero;
            obeliskBrokenRigidBody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}