using System.Collections;
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
            Vector3 newPosition = Vector3.zero;

            foreach (PlayerCharacter player in _players)
            {
                newPosition += player.transform.position;
            }

            newPosition = newPosition / _players.Length;
            newPosition.y = _startPosition.y;
            newPosition.z = _startPosition.z;
            transform.position = newPosition;
        }
    }
}
