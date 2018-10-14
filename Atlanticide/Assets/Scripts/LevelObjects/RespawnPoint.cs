using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class RespawnPoint : Interactable
    {
        [SerializeField]
        private Transform _respawnPoint;

        [SerializeField]
        private Vector3 _respawnRotation;

        [SerializeField]
        private float _respawnTime = 1f;

        [SerializeField]
        private int _defaultEnergyCost = 1;

        private PlayerCharacter _respawningPlayer;
        private float _elapsedTime;

        /// <summary>
        /// Is the respawn point active.
        /// </summary>
        public bool RespawnActive { get; private set; }

        /// <summary>
        /// How close is the respawn to its completion.
        /// </summary>
        public float RespawnProgress { get; private set; }

        /// <summary>
        /// How much time is left until the respawn is complete.
        /// </summary>
        public float RespawnTimeLeft
        {
            get
            {
                if (RespawnActive)
                {
                    return _respawnTime - _elapsedTime;
                }
                else
                {
                    return 0f;
                }
            }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            EnergyCost = _defaultEnergyCost;
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (RespawnActive)
            {
                UpdatePlayerRespawn();
            }
            else
            {
                CheckForPlayerWithinRange();
            }

            base.UpdateObject();
        }

        private void UpdatePlayerRespawn()
        {
            _elapsedTime += World.Instance.DeltaTime;
            RespawnProgress = (_elapsedTime / _respawnTime);
            if (_elapsedTime >= _respawnTime)
            {
                RespawnPlayer();
            }
        }

        private void CheckForPlayerWithinRange()
        {
            if (GameManager.Instance.DeadPlayerCount > 0)
            {
                if (Interactor == null &&
                    GameManager.Instance.DeadPlayerCount < GameManager.Instance.PlayerCount)
                {
                    // Gets a living player
                    PlayerCharacter pc = GameManager.Instance.GetAnyPlayer(false);
                    if (pc != null)
                    {
                        Interactor = pc;
                    }
                }

                if (Interactor != null)
                {
                    if (Interactor.IsDead)
                    {
                        Interactor = null;
                        return;
                    }

                    float distance = Vector3.Distance
                        (transform.position, Interactor.transform.position);
                    _interactorIsValid = (distance <= World.Instance.InteractRange);
                    SetInteractorTarget(_interactorIsValid);
                }
            }
        }

        /// <summary>
        /// Makes the interactor player interact with this object.
        /// </summary>
        /// <returns>Was the interaction successful</returns>
        public override bool Interact()
        {
            if (!RespawnActive)
            {
                PlayerCharacter deadPlayer =
                    GameManager.Instance.GetAnyOtherPlayer(Interactor, true);
                return TryStartPlayerRespawn(deadPlayer);
            }

            return false;
        }

        private bool TryStartPlayerRespawn(PlayerCharacter player)
        {
            if (!RespawnActive && player.IsDead)
            {
                RespawnActive = true;
                _respawningPlayer = player;
                _elapsedTime = 0f;
                return true;
            }

            return false;
        }

        private void RespawnPlayer()
        {
            _respawningPlayer.RespawnPosition = _respawnPoint.position;
            _respawningPlayer.Respawn();
            _respawningPlayer.transform.rotation = Quaternion.Euler(_respawnRotation);
            EndPlayerRespawn();
        }

        public void EndPlayerRespawn()
        {
            RespawnActive = false;
            SetInteractorTarget(false, true);
            _respawningPlayer = null;
        }

        public override void ResetObject()
        {
            EndPlayerRespawn();
            EnergyCost = _defaultEnergyCost;
            base.ResetObject();
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected override void OnDrawGizmos()
        {
            if (!RespawnActive)
            {
                base.OnDrawGizmos();
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position +
                    Vector3.up * 1.5f, 0.5f * (1 - RespawnProgress));
            }
        }
    }
}
