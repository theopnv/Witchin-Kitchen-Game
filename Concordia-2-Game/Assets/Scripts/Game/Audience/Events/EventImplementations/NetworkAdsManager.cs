using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProBuilder2.Common;
using UnityEngine;

namespace con2.game
{
    public class NetworkAdsManager : MonoBehaviour
    {
        [SerializeField]
        private Ad _combo1FireBlades;

        [SerializeField]
        private Ad _combo1Apron;

        [SerializeField]
        private Ad _combo1FireBladesUp;

        [SerializeField]
        private Ad _combo1ApronDown;

        [SerializeField]
        private Ad _combo2FireBlades;

        [SerializeField]
        private Ad _combo2Apron;

        [SerializeField]
        private Ad _combo2FireBladesDown;

        [SerializeField]
        private Ad _combo2ApronUp;

        [SerializeField]
        private Ad _combo3FireBlades;

        [SerializeField]
        private Ad _combo3Apron;

        [SerializeField]
        private Ad _combo3FireBladesUp;

        [SerializeField]
        private Ad _combo3ApronDown;

        [SerializeField]
        private Ad _combo4FireBlades;

        [SerializeField]
        private Ad _combo4Apron;

        [SerializeField]
        private Ad _combo4FireBladesDown;

        [SerializeField]
        private Ad _combo4ApronUp;

        private static readonly int[] SuiteNumbers = UniqueRandomNumbers(0, 3);
        private static int _index = 0;
        private const int NumberOfCombos = 4;

        public float m_MessageDelay;
        protected bool _DelayIsPassed = false;

        void MessageDelay(float time)
        {
            m_MessageDelay = time;
        }

        private IEnumerator TimerDelay()
        {
            yield return new WaitForSeconds(m_MessageDelay);
            switch (SuiteNumbers[_index])
            {
                case 0:
                    _combo1FireBlades.gameObject.SetActive(true);
                    _combo1Apron.gameObject.SetActive(true);
                    _combo1FireBladesUp.gameObject.SetActive(true);
                    _combo1ApronDown.gameObject.SetActive(true);
                    break;
                case 1:
                    _combo2FireBlades.gameObject.SetActive(true);
                    _combo2Apron.gameObject.SetActive(true);
                    _combo2FireBladesDown.gameObject.SetActive(true);
                    _combo2ApronUp.gameObject.SetActive(true);
                    break;
                case 2:
                    _combo3FireBlades.gameObject.SetActive(true);
                    _combo3Apron.gameObject.SetActive(true);
                    _combo3FireBladesUp.gameObject.SetActive(true);
                    _combo3ApronDown.gameObject.SetActive(true);
                    break;
                case 3:
                    _combo4FireBlades.gameObject.SetActive(true);
                    _combo4Apron.gameObject.SetActive(true);
                    _combo4FireBladesDown.gameObject.SetActive(true);
                    _combo4ApronUp.gameObject.SetActive(true);
                    break;

            }

            ++_index;
            if (_index == NumberOfCombos)
                _index = 0;
        }

        public static int[] UniqueRandomNumbers(int min, int max)
        {
            var rnd = new System.Random();
            int[] numbers = new int[max - min + 1];
            var toBeInserted = new List<int>();
            for (int i = min; i < NumberOfCombos; i++)
            {
                toBeInserted.Add(i);
            }

            var idx = 0;
            while (toBeInserted.Count != 0)
            {
                var num = rnd.Next(min, max - idx);
                numbers[idx] = toBeInserted.ElementAt(num);
                toBeInserted.RemoveAt(num);
                idx++;
            }

            return numbers;
        }

        // Start is called before the first frame update
        void Start()
        {
            _combo1FireBlades.gameObject.SetActive(false);
            _combo1Apron.gameObject.SetActive(false);
            _combo1FireBladesUp.gameObject.SetActive(false);
            _combo1ApronDown.gameObject.SetActive(false);

            _combo2FireBlades.gameObject.SetActive(false);
            _combo2Apron.gameObject.SetActive(false);
            _combo2FireBladesDown.gameObject.SetActive(false);
            _combo2ApronUp.gameObject.SetActive(false);

            _combo3FireBlades.gameObject.SetActive(false);
            _combo3Apron.gameObject.SetActive(false);
            _combo3FireBladesUp.gameObject.SetActive(false);
            _combo3ApronDown.gameObject.SetActive(false);

            _combo4FireBlades.gameObject.SetActive(false);
            _combo4Apron.gameObject.SetActive(false);
            _combo4FireBladesDown.gameObject.SetActive(false);
            _combo4ApronUp.gameObject.SetActive(false);

            StartCoroutine("TimerDelay");
        }

        // Update is called once per frame
        void Update()
        {
        }
    }

}
