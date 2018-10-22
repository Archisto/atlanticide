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
        public HitCast Hitcast
        {
            get { return _HitCast; }
        }

        // Get Speed
        public float Speed() { return _speed; }

        // Add new GizmoDraw
        public void AddGizmoAction(GizmoAction gizmo)
        {
            GizmoActions.Add(gizmo);
        }

        #endregion

        #region Variables

        // Target of the actions of EnemyBase
        public GameCharacter Target
        {
            get;
            set;
        }

        // Time passed from the instantiation of the (IEnemyState) CurrentState
        protected float StateTimeElapsed
        {
            get; private set;
        }

        // List of GizmoDraws iterated in OnDrawGizmos
        protected List<GizmoAction> GizmoActions;

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
            GizmoActions = new List<GizmoAction>();
            SetState(CreateBirthState());
        }

        // Update is called once per frame
        protected sealed override void Update()
        {

            if (World.Instance.GamePaused)
            {
                return;
            }

            base.Update();

            // Check conditions for changing state
            IEnemyState state = CurrentState.Conditions(StateTimeElapsed);

            // if state should be changed, do it
            if (state != null)
            {
                CurrentState.Finish();
                SetState(state);
            }

            // Action
            CurrentState.Action(StateTimeElapsed);

            // Draw Gizmos
            CurrentState.DrawGizmos();

            // elapse time
            StateTimeElapsed += World.Instance.DeltaTime;
        }

        /// <summary>
        /// Sets new IEnemyState, instantiates it and resets StateTimeElapsed
        /// </summary>
        /// <param name="state">new IEnemyState</param>
        private void SetState(IEnemyState state)
        {
            CurrentState = state;
            CurrentState.Instantiate(this);
            StateTimeElapsed = 0;
        }

        /// <summary>
        /// Does everything GameCharacter Kill() does, but also sets DeathState as CurrentState
        /// </summary>
        public override void Kill()
        {
            base.Kill();
            SetState(CreateDeathState());
        }

        /// <summary>
        /// Called on Start() to get new birth state
        /// </summary>
        protected abstract IEnemyState CreateBirthState();

        /// <summary>
        /// Called on Kill() to get new death state
        /// </summary>
        protected abstract IEnemyState CreateDeathState();

        // Draws a List of Gizmos
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            // go through GizmoActions
            if(GizmoActions == null)
            {
                return;
            }
            foreach (GizmoAction gizmo in GizmoActions)
            {
                Gizmos.color = gizmo.color;
                switch (gizmo.gizmoType)
                {
                    case "line":
                        Gizmos.DrawLine(gizmo.vector, gizmo.vector2);
                        break;
                    case "wire_cube":
                        Gizmos.DrawWireCube(gizmo.vector, gizmo.vector2);
                        break;
                    case "wire_sphere":
                        Gizmos.DrawWireSphere(gizmo.vector, gizmo.range);
                        break;
                }
            }
            GizmoActions.Clear();
        }

        #region Struct for Gizmos

        /// <summary>
        /// Contains drawing information to draw Gizmos
        /// </summary>
        public struct GizmoAction
        {
            public GizmoAction(string t, Color c, Vector3 v, Vector3 v2, float r)
            {
                gizmoType = t;
                color = c;
                vector = v;
                vector2 = v2;
                range = r;
            }

            public string gizmoType;
            public Color color;
            public Vector3 vector;
            public Vector3 vector2;
            public float range;
        }

        #endregion

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
        public void MoveTowardsTarget(Vector3 target, bool look, bool rotate, float rise, float speed)
        {
            // Draw Gizmos
            Vector3 right = Vector3.right * 3;
            Vector3 up = Vector3.up * 3;
            AddGizmoAction(new GizmoAction("line", Color.green, target - up, target + up, 0));
            AddGizmoAction(new GizmoAction("line", Color.green, target - right, target + right, 0));


            // modify target y
            target.Set(target.x, transform.position.y, target.z);

            // calculate move step
            Vector3 move = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            // look or rotate towards target
            if (look)
            {
                transform.LookAt(target);
                //LookTowards(target);
            }
            else if (rotate)
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
        public bool Distance(bool bigger, Vector3 start, Vector3 end, float distance, bool negateY)
        {
            // Draw line
            AddGizmoAction(new GizmoAction("line", Color.white, start, end, 0));

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

        /// <summary>
        /// Searches for players inside the Range
        /// </summary>
        /// <param name="Range">Range of the search</param>
        /// <returns>true if target found</returns>
        public bool SearhForTarget(float Range)
        {
            AddGizmoAction(new GizmoAction("wire_sphere", Color.yellow, transform.position, Vector3.zero, Range));

            GameCharacter[] players = GameManager.Instance.GetPlayersWithinRange(transform.position, Range);

            if (players[0] != null)
            {
                Target = players[0];
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if Target exists, is still alive and withing MaxDistance
        /// </summary>
        /// <param name="MaxDistance">Maximum allowed distance between EnemyBase and Target</param>
        /// <returns>true if Target is still valid</returns>
        public bool ValidateTarget(float MaxDistance)
        {
            return Target != null && !Target.IsDead && Distance(false, transform.position, Target.transform.position, MaxDistance, false);
        }

        #endregion

    }
}