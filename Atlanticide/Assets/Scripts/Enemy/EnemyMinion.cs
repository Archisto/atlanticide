using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Small, fast enemy
    /// </summary>
    public class EnemyMinion : EnemyBase
    {

        #region Behaviour Variables for Inspector

        [Header("Minion behaviour")]

        // The area around Minion where it wanders without target
        [SerializeField]
        private float _IdleWanderRange;

        // Get _IdleRange
        public float IdleWanderRange
        {
            get { return _IdleWanderRange; }
        }

        // The area around Minion where it searches targets from
        [SerializeField]
        private float _SearchRange;

        // Get _SearchRange
        public float SearchRange
        {
            get { return _SearchRange; }
        }

        // The distance to Target where Minion starts its attack from
        [SerializeField]
        private float _AttackRange;

        // Get _AttackRange
        public float AttackRange
        {
            get { return _AttackRange; }
        }

        // The max distance between target and enemy when enemy still approaches target
        [SerializeField]
        private float _ChaseRange;

        // Get _ChaseRange
        public float ChaseRange
        {
            get { return _ChaseRange; }
        }

        #endregion

        #region Behaviour Variables, fixed

        // Anchor point for IdleWander
        public Vector3 IdleWanderAnchor
        {
            get;
            set;
        }

        #endregion

        protected override IEnemyState CreateBirthState()
        {
            return new BirthFromStart();
        }

        protected override IEnemyState CreateDeathState()
        {
            return new DeathFreeze();
        }

    }
}