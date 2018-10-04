using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atlanticide.UI;

namespace Atlanticide
{
    public class LerpToCameraView : MonoBehaviour
    {
        public bool active;
        public Vector3 camPosOffset;
        public float lerpTime = 1f;

        private bool _targetReached;
        private float _elapsedTime;
        private Vector3 _camRotOffset = Vector3.zero;
        private Vector3 _startPos;
        private Quaternion _startRot;
        private Vector3 _targetPos;
        private Quaternion _targetRot;
        private CameraController _cam;
        private MeshRenderer[] rends;

        // Use this for initialization
        void Start()
        {
            _cam = FindObjectOfType<CameraController>();
            rends = GetComponentsInChildren<MeshRenderer>();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (active)
            {
                SetPositionAndRotation();
            }
        }

        private void SetPositionAndRotation()
        {
            // TODO: Figure out of to use _camRotOffset without the lerped object
            // turning in wrong directions when the camera's rotation changes.

            _targetPos = _cam.GetCameraViewPosition(camPosOffset);
            _targetRot = _cam.GetRotationTowardsCamera();

            if (!_targetReached)
            {
                _elapsedTime += World.Instance.DeltaTime;
                float ratio = (_elapsedTime / lerpTime);
                if (ratio >= 1f)
                {
                    ratio = 1f;
                    _targetReached = true;
                }

                transform.position = Vector3.Lerp(_startPos, _targetPos, ratio);
                transform.rotation = Quaternion.Lerp(_startRot, _targetRot, ratio);
            }
            else
            {
                transform.position = _targetPos;
                transform.rotation = _targetRot;
            }
        }

        public void StartLerp()
        {
            active = true;
            _startPos = transform.position;
            _startRot = transform.rotation;
            _targetReached = false;
            _elapsedTime = 0f;
            SetShadowsActive(false);
        }

        public void ResetLerp()
        {
            active = false;
            _targetReached = false;
            SetShadowsActive(true);
        }

        private void SetShadowsActive(bool activate)
        {
            foreach (MeshRenderer mr in rends)
            {
                mr.shadowCastingMode = (activate ?
                    UnityEngine.Rendering.ShadowCastingMode.On :
                    UnityEngine.Rendering.ShadowCastingMode.Off);
            }
        }
    }
}
