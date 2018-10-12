using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{

    public class Insect : EnemyCharacter
    {

        #region start methods overwritten

        protected override void startNoTarget()
        {
            base.startNoTarget();
        }

        protected override void startTargetOn()
        {
            base.startTargetOn();
        }

        protected override void startAttack()
        {
            base.startAttack();
        }

        #endregion

        #region run methods overwritten

        protected override void noTarget()
        {
            base.noTarget();
        }

        protected override void targetOn()
        {
            base.targetOn();
        }

        protected override void attack()
        {
            base.attack();
        }

        #endregion

        #region end methods overwritten

        protected override void endNoTarget()
        {
            base.endNoTarget();
            
        }

        protected override void endTargetOn()
        {
            base.endTargetOn();
        }

        protected override void endAttack()
        {
            base.endAttack();
        }

        #endregion

    }
}