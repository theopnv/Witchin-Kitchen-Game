using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace con2.lobby
{

    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI CurrentInstruction;

        void Start()
        {
            StartCoroutine(_1_Welcome());
        }

        #region Instructions

        private IEnumerator _1_Welcome()
        {
            CurrentInstruction.text = "Welcome to Witchin' Kitchen, candidates!\r\nToday's crazy show is broadcasted all over the world. It's time to show that you're the best witch or wizard!";
            yield return new WaitForSeconds(10);
            yield return _2_Goal();
        }

        private IEnumerator _2_Goal()
        {
            CurrentInstruction.text = "Your goal is to complete potions. Your cauldron will be your most precious ally.\r\nTry to grab this ingredient over there and to throw it into.";
            yield return null;
        }

        #endregion
    }

}
