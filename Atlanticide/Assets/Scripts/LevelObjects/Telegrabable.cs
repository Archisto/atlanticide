using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Telegrabable : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 20f)]
        private float _baseTelegrabSpeed = 1;

        [SerializeField]
        private bool _active = true;

        private float _telegrabSpeed;
        private int _telegrabID;
        private Vector3 _telegrabPosition;

        public bool Telegrabbed { get; set; }

        private void Update()
        {
            if (_active)
            {
                UpdateTelegrabState();
                Move();
            }
        }

        private void UpdateTelegrabState()
        {
            Transform[] telegrabs = GameManager.Instance.GetTelegrabs();
            if (Telegrabbed)
            {
                // The telegrab stopped
                if (telegrabs[_telegrabID] == null)
                {
                    Telegrabbed = false;
                }
                else
                {
                    _telegrabPosition = telegrabs[_telegrabID].position;
                }
            }
            else
            {
                for (int i = 0; i < telegrabs.Length; i++)
                {
                    if (telegrabs[i] != null &&
                        Vector3.Distance(transform.position, telegrabs[i].position) <= World.Instance.telegrabRadius)
                    {
                        Telegrabbed = true;
                        _telegrabID = i;
                        _telegrabPosition = telegrabs[i].position;
                        break;
                    }
                }
            }
        }

        private void Move()
        {
            if (Telegrabbed)
            {
                _telegrabSpeed = _baseTelegrabSpeed + Vector3.Distance(transform.position, _telegrabPosition) * 3f;

                transform.position =
                    Vector3.MoveTowards(transform.position, _telegrabPosition, _telegrabSpeed * World.Instance.DeltaTime);
            }
        }

        public void SetActive(bool active)
        {
            if (!active && Telegrabbed)
            {
                Telegrabbed = false;
            }

            _active = active;
        }
    }
}
