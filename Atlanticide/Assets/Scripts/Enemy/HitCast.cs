using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class HitCast : MonoBehaviour
    {

        #region Public Parameters

        // Range of the hit check
        [SerializeField]
        private float _Range;

        // Damage of the hit
        [SerializeField]
        private int _Damage;

        #endregion

        #region Getters for Public Parameters

        // Get _Range
        public float Range
        {
            get { return _Range; }
        }

        // Get _Damage
        public int Damage
        {
            get { return _Damage; }
        }

        #endregion

        #region Variables

        // Whether hit detection is checked
        public bool Hitting
        {
            get;
            private set;
        }

        // What is hit
        public enum HitType
        {
            NONE, SHIELD, PLAYER
        }

        // what this HitCast hit
        private HitType _HitType;

        // Return _HitType and set it as NONE
        public HitType GetHitType()
        {
            HitType temp = _HitType;
            _HitType = HitType.NONE;
            return temp;
        }

        #endregion

        #region Methods

        // Use this for initialization
        void Start()
        {
            Hitting = false;
            _HitType = HitType.NONE;
        }

        // Update is called once per frame
        void Update()
        {
            if (Hitting)
            {
                CheckHit();
            }
        }

        #region Hitting

        /// <summary>
        /// Checks whether enemy hits something
        /// </summary>
        private void CheckHit()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Range))
            {
                // Check whether shield or character has been hit and react accordingly
                Shield shield = hit.collider.gameObject.GetComponent<Shield>();
                PlayerCharacter character;

                if (shield != null && shield.BlocksDamage)
                {
                    HitShield(shield);
                }
                else
                {
                    character = hit.collider.gameObject.GetComponent<PlayerCharacter>();
                    if (character != null && !character.IsDead)
                    {
                        HitCharacter(character);
                    } else
                    {
                        _HitType = HitType.NONE;
                    }
                }
            }
        }

        /// <summary>
        /// What happens when enemy hits a shield
        /// </summary>
        /// <param name="shield">shield script on the object that is hit</param>
        protected virtual void HitShield(Shield shield)
        {
            shield.Hit();
            _HitType = HitType.SHIELD;
            Debug.Log("shield hit");
        }

        /// <summary>
        /// What happens when enemy hits a character
        /// </summary>
        /// <param name="character">GameCharacter script on the object that is hit</param>
        protected virtual void HitCharacter(GameCharacter character)
        {
            character.TakeDamage(Damage);
            _HitType = HitType.PLAYER;
            Debug.Log("character hit");
        }

        /// <summary>
        /// Draws the HitRay red if enemy is hitting, yellow otherwise
        /// </summary>
        private void OnDrawGizmos()
        {
            if (Hitting)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.yellow;
            }
            Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * Range);
        }

        #endregion

        #endregion

    }
}