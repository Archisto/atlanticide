using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class KeyPickup : Pickup
    {
        public int keyCode;

        [SerializeField]
        private bool _allowDuplicateKeyCodes;

        public override void Collect(PlayerCharacter character)
        {
            World.Instance.TryActivateNewKeyCode(keyCode, _allowDuplicateKeyCodes);
            base.Collect(character);

            SFXPlayer.Instance.Play(Sound.Item_Pickup);
        }
    }
}
