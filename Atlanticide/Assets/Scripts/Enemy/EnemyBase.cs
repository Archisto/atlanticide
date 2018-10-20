using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    /// <summary>
    /// Base class for enemies
    /// </summary>
    public abstract class EnemyBase : GameCharacter
    {

        #region References

        // HitCast class for enemy, takes care of attack hit detection
        [SerializeField]
        private HitCast _HitCast;

        #endregion

        #region Getters

        // Get _HitCast
        protected HitCast Hitcast
        {
            get { return _HitCast; }
        }

        #endregion

        #region Variables

        // Target of the actions of EnemyBase
        protected GameCharacter Target
        {
            get;
            set;
        }

        // Time passed from the instantiation of the (IEnemyState) CurrentState
        protected float StateTimeElapsed
        {
            get; private set;
        }

        #endregion

        #region EnemyState

        // State of the enemy, what actions are performed
        protected IEnemyState CurrentState
        {
            get; set;
        }

        #endregion

        #region Core Methods

        // Use this for initialization
        protected sealed override void Start()
        {
            base.Start();
            CurrentState = CreateBirthState();
            CurrentState.Instantiate();
            StateTimeElapsed = 0f;
        }

        // Update is called once per frame
        protected sealed override void Update()
        {
            base.Update();

            // act
            IEnemyState state = CurrentState.Conditions(StateTimeElapsed);
            if(state != null)
            {
                CurrentState.Finish();
                CurrentState = state;
                CurrentState.Instantiate();
                StateTimeElapsed = 0;
            }
            CurrentState.Action(StateTimeElapsed);
            StateTimeElapsed += Time.deltaTime;
        }

        /// <summary>
        /// Does everything GameCharacter Kill() does, but also sets DeathState as CurrentState
        /// </summary>
        public override void Kill()
        {
            base.Kill();
            CurrentState = CreateDeathState();
            CurrentState.Instantiate();
            StateTimeElapsed = 0;
        }

        /// <summary>
        /// Called on Start() to get new birth state
        /// </summary>
        protected abstract IEnemyState CreateBirthState();

        /// <summary>
        /// Called on Kill() to get new death state
        /// </summary>
        protected abstract IEnemyState CreateDeathState();

        #endregion

        #region Utility Methods

        /// <summary>
        /// Moves object towards target with given speed.
        /// </summary>
        /// <param name="target">Vector3 position</param>
        /// <param name="look">should object look at the target</param>
        /// <param name="rotate">should object rotate towards the target</param>
        /// <param name="rise">character is raised by this amount</param>
        /// <param name="speed">character moves with this speed * Time.deltaTime</param>
        protected void MoveTowardsTarget(Vector3 target, bool look, bool rotate, float rise, float speed)
        {
            // modify target y
            target.Set(target.x, transform.position.y, target.z);

            // calculate move step
            Vector3 move = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            // look or rotate towards target
            if (look)
            {
                LookTowards(target);
            } else if (rotate)
            {
                RotateTowards(target);
            }

            // move
            transform.position = move;

            // rise
            _groundCollider.onGround = false;
            _groundCollider.Rise(rise);
        }

        /// <summary>
        /// Checks whether distance between given vectors is bigger/smaller than given value.
        /// Y axis of vectors can be neutralized from the calculation.
        /// </summary>
        /// <param name="bigger">is distance compared by bigger or equal to</param>
        /// <param name="start">start vector</param>
        /// <param name="end">end vector</param>
        /// <param name="distance">to what distance the distance between vectors is compared to</param>
        /// <param name="negateY">Are y axis values neutralized from the calculation</param>
        /// <returns></returns>
        protected bool Distance(bool bigger, Vector3 start, Vector3 end, float distance, bool negateY)
        {
            // remove y values from the calculation
            if (negateY)
            {
                start.Set(start.x, 0, start.z);
                end.Set(end.x, 0, end.z);
            }

            // compare
            if (bigger)
            {
                return Vector3.Distance(start, end) >= distance;
            }
            else
            {
                return Vector3.Distance(start, end) <= distance;
            }
        }

        #endregion

    }
}