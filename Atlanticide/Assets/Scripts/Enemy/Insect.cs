using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class Insect : EnemyCharacter
    {

        #region start methods overwritten

        protected override void StartNoTarget()
        {
            base.StartNoTarget();
        }

        protected override void StartTargetOn()
        {
            base.StartTargetOn();
        }

        protected override void StartAttack()
        {
            base.StartAttack();
        }

        #endregion

        #region run methods overwritten

        protected override void NoTarget()
        {
            base.NoTarget();
        }

        protected override void TargetOn()
        {
            base.TargetOn();
        }

        protected override void Attack()
        {
            base.Attack();
        }

        #endregion

        #region end methods overwritten

        protected override void EndNoTarget()
        {
            base.EndNoTarget();
            
        }

        protected override void EndTargetOn()
        {
            base.EndTargetOn();
        }

        protected override void EndAttack()
        {
            base.EndAttack();
        }

        #endregion

    }
}