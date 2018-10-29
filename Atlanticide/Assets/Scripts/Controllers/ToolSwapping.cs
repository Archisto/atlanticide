using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Atlanticide.UI;

namespace Atlanticide
{
    /// <summary>
    /// Controls swapping tools between players.
    /// </summary>
    public class ToolSwapping : MonoBehaviour
    {
        [SerializeField, Range(0.5f, 20f)]
        private float _swapRequestDuration = 5f;

        [SerializeField, Range(1f, 50f)]
        private float _swapRange = 5f;

        [SerializeField, Range(0f, 10f)]
        private float _iconDistFromPlayerHead = 1f;

        [SerializeField]
        private Color _swapIconOffColor = Color.gray;

        [SerializeField]
        private Color _swapIconOnColor = Color.yellow;

        private PlayerCharacter[] _players;
        private PlayerCharacter _requestingPlayer;
        private UIController _ui;
        private Image _swapIcon;
        private float _elapsedTime;

        /// <summary>
        /// Is a swap request active.
        /// </summary>
        public bool SwapRequestActive { get; private set; }

        /// <summary>
        /// How close is the swap request to its expiration.
        /// </summary>
        public float SwapRequestExpirationProgress { get; private set; }

        /// <summary>
        /// How much time is left until the swap request expires.
        /// </summary>
        public float SwapRequestTimeLeft
        {
            get
            {
                if (SwapRequestActive)
                {
                    return _swapRequestDuration - _elapsedTime;
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
            _players = GameManager.Instance.GetPlayers();
            _ui = FindObjectOfType<UIController>();
            _swapIcon = _ui.swapIcon;
            if (_swapIcon != null)
            {
                _swapIcon.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (SwapRequestActive)
            {
                UpdateSwapRequest();
            }
        }

        /// <summary>
        /// Updates the object after Update.
        /// </summary>
        private void LateUpdate()
        {
            if (SwapRequestActive)
            {
                UpdateSwapRequestIcon();
            }
        }

        /// <summary>
        /// Updates the swap request timer.
        /// If the time is up, the request expires.
        /// </summary>
        private void UpdateSwapRequest()
        {
            _elapsedTime += World.Instance.DeltaTime;
            SwapRequestExpirationProgress = (_elapsedTime / _swapRequestDuration);
            if (_elapsedTime >= _swapRequestDuration)
            {
                EndSwapRequest();
            }
        }

        /// <summary>
        /// Updates the swap UI icon's look and position.
        /// </summary>
        private void UpdateSwapRequestIcon()
        {
            if (_swapIcon != null)
            {
                PlayerCharacter otherPlayer = GameManager.Instance.
                    GetAnyOtherPlayer(_requestingPlayer, false);

                if (otherPlayer != null)
                {
                    if (GameManager.Instance.PlayersAreWithinRangeOfEachOther
                        (otherPlayer, _requestingPlayer, _swapRange))
                    {
                        _swapIcon.color = _swapIconOnColor;
                    }
                    else
                    {
                        _swapIcon.color = _swapIconOffColor;
                    }
                }

                UpdateSwapRequestIconPosition();
            }
        }

        /// <summary>
        /// Updates the position of the swap UI icon. The icon is always
        /// above the head of the player who initiated the swap request.
        /// </summary>
        private void UpdateSwapRequestIconPosition()
        {
            Vector3 swapIconPosition = _requestingPlayer.transform.position +
                Vector3.up * (_requestingPlayer.Size.y / 2 + _iconDistFromPlayerHead);
            _ui.MoveUIObjToWorldPoint(_swapIcon, swapIconPosition, Vector2.zero);
        }

        /// <summary>
        /// Initiates a swap request if one is not already active.
        /// If one is, cancels the request or accepts it
        /// depending on which player gave the input.
        /// A swap request can only be accepted if the
        /// players are within a defined range of each other.
        /// </summary>
        /// <param name="player">Which player initiated the request</param>
        /// <returns>Were tools swapped</returns>
        public bool InitiateSwapRequest(PlayerCharacter player)
        {
            // Initiates a swap request
            if (!SwapRequestActive)
            {
                SwapRequestActive = true;
                _requestingPlayer = player;
                _elapsedTime = 0f;
                if (_swapIcon != null)
                {
                    _swapIcon.gameObject.SetActive(true);
                    UpdateSwapRequestIcon();
                }
            }
            else
            {
                // Accepts a swap request
                if (player != _requestingPlayer)
                {
                    if (GameManager.Instance.PlayersAreWithinRangeOfEachOther
                        (player, _requestingPlayer, _swapRange))
                    {
                        SwapTools();
                        EndSwapRequest();
                        return true;
                    }
                }
                // Cancels a swap request
                else
                {
                    EndSwapRequest();
                }
            }

            return false;
        }

        /// <summary>
        /// Ends the swap request.
        /// </summary>
        public void EndSwapRequest()
        {
            SwapRequestActive = false;
            _requestingPlayer = null;
            if (_swapIcon != null)
            {
                _swapIcon.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Swaps the tools between the players.
        /// </summary>
        public void SwapTools()
        {
            PlayerTool pt1 = _players[0].Tool;
            PlayerTool pt2 = _players[1].Tool;
            GameManager.Instance.SetPlayerTool(_players[0], pt2);
            GameManager.Instance.SetPlayerTool(_players[1], pt1);

            Debug.Log(string.Format("Swapped tools - {0} gets {2} and {1} gets {3}",
                _players[0].name, _players[1].name, pt2, pt1));

            GameManager.Instance.SavePlayerTools();
        }

        /// <summary>
        /// Returns whether the swap request was initiated by the given player.
        /// </summary>
        /// <param name="player">A player</param>
        /// <returns>Was the swap request initiated by the player</returns>
        public bool RequestInitiatedBy(PlayerCharacter player)
        {
            return (SwapRequestActive && _requestingPlayer != null
                && _requestingPlayer == player);
        }
    }
}
