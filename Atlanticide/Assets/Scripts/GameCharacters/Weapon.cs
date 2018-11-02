using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        private GameObject _projectilePrefab;

        [SerializeField]
        private float _fireRate = 0.5f;

        [SerializeField]
        private float _projectileSpeed = 3f;

        [SerializeField]
        private int _maxProjectiles = 5;

        [SerializeField]
        private float _range = 10;

        private bool _fired;
        private float _elapsedFiringTime;

        private GameObject[] _projectiles;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _projectiles = new GameObject[_maxProjectiles];

            for (int i = 0; i < _maxProjectiles; i++)
            {
                _projectiles[i] = Instantiate(_projectilePrefab);
                _projectiles[i].SetActive(false);
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (_fired)
            {
                _elapsedFiringTime += World.Instance.DeltaTime;
                if (_elapsedFiringTime >= _fireRate)
                {
                    _elapsedFiringTime = 0;
                    _fired = false;
                }
            }
        }

        /// <summary>
        /// Launches a projectile.
        /// </summary>
        public void Fire()
        {
            if (!_fired)
            {
                GameObject projectile = Utils.GetFirstActiveOrInactiveObject(_projectiles, false) as GameObject;
                if (projectile != null)
                {
                    _fired = true;
                    projectile.SetActive(true);
                    projectile.transform.position = transform.position;
                    Move projectileMover = projectile.GetComponent<Move>();
                    projectileMover.TopSpeed = transform.forward * _projectileSpeed;
                    projectileMover.DecayDistance = _range;
                    projectileMover.StartMoving(true);

                    //SFXPlayer.Instance.Play(Sound.Projectile_1);
                }
            }
        }
    }
}
