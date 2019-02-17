using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class PlayerPunch : MonoBehaviour, IInputConsumer
    {
        private GameObject target;
        private FighterPunch punch;
        private bool canPunch = true;

        // Start is called before the first frame update
        void Start()
        {
            punch = GetComponentInChildren<FighterPunch>();
        }

        public bool ConsumeInput(GamepadAction input)
        {
            if (!canPunch)
                return false;

            if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.PUNCH))
            {
                canPunch = false;
                punch.RequestPunch();

                // Cancel punch after some time
                StartCoroutine(StopPunch());

                // Reload punch after some time
                StartCoroutine(ReloadPunch());
                return true;
            }

            return false;
        }

        IEnumerator StopPunch()
        {
            yield return new WaitForSeconds(GlobalFightState.get().PunchLingerSeconds);
            punch.CancelPunch();
        }

        IEnumerator ReloadPunch()
        {
            yield return new WaitForSeconds(GlobalFightState.get().PunchReloadSeconds);
            canPunch = true;
        }

    }

}
