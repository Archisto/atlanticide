﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class CameraController : MonoBehaviour
    {
        private Vector3 _startPosition;
        private PlayerCharacter[] _players;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _startPosition = transform.position;
            _players = GameManager.Instance.GetPlayers();
        }

        private void LateUpdate()
        {
            if (GameManager.Instance.PlayReady)
            {
                SetPositionInPlay();
            }
        }

        private void SetPositionInPlay()
        {
            Vector3 newPosition = Vector3.zero;

            for (int i = 0; i < GameManager.Instance.PlayerCount; i++)
            {
                newPosition += _players[i].transform.position;
            }

            newPosition = newPosition / GameManager.Instance.PlayerCount;
            newPosition.y = _startPosition.y;
            newPosition.z = _startPosition.z;
            transform.position = newPosition;
        }

        public Vector3 GetCameraViewPosition(Vector3 camPosOffset)
        {
            Vector3 worldOffset = transform.right * camPosOffset.x +
                                  transform.up * camPosOffset.y +
                                  transform.forward * camPosOffset.z;
            return transform.position + worldOffset;
        }

        public Quaternion GetRotationTowardsCamera()
        {
            return Quaternion.Euler(
                transform.rotation.eulerAngles.x * -1,
                transform.rotation.eulerAngles.y + 180,
                transform.rotation.eulerAngles.z * -1);
        }
    }
}
