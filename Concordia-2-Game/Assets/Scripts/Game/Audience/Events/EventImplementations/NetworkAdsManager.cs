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
        private Ad _BreakingNews;

        [SerializeField]
        private Ad _Ketchup;

        [SerializeField]
        private Ad _FireBlades;

        [SerializeField]
        private Ad _Apron;

        private static int[] _suiteNumbers = UniqueRandomNumbers(0, 3);
        private static int _index = 0;
        private static int _numberOfAds = 4; 

        public static int[] UniqueRandomNumbers(int min, int max)
        {
            var rnd = new System.Random();
            int[] numbers = new int[max - min + 1];
            var toBeInserted = new List<int>();
            for (int i = min; i <= max; i++)
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
            _BreakingNews.gameObject.SetActive(false);
            _Ketchup.gameObject.SetActive(false);
            _FireBlades.gameObject.SetActive(false);
            _Apron.gameObject.SetActive(false);

            switch (_suiteNumbers[_index])
            {
                case 0:
                    _BreakingNews.gameObject.SetActive(true);
                    break;
                case 1:
                    _Ketchup.gameObject.SetActive(true);
                    break;
                case 2:
                    _FireBlades.gameObject.SetActive(true);
                    break;
                case 3:
                    _Apron.gameObject.SetActive(true);
                    break;

            }

            ++_index;
            if (_index == _numberOfAds)
                _index = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
