using System;
using System.Collections;
using System.Collections.Generic;
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
        private Ad _Pfc;

        [SerializeField]
        private Ad _Ruffles;

        // Start is called before the first frame update
        void Start()
        {
            _BreakingNews.gameObject.SetActive(false);
            _Ketchup.gameObject.SetActive(false);
            _Pfc.gameObject.SetActive(false);
            _Ruffles.gameObject.SetActive(false);
            var rnd = new System.Random();
            var num = rnd.Next(0, 2);
            switch (num)
            {
                case 0:
                    _BreakingNews.gameObject.SetActive(true);
                    break;
                case 1:
                    _Ketchup.gameObject.SetActive(true);
                    break;
                case 2:
                    _Pfc.gameObject.SetActive(true);
                    break;
                case 3:
                    _Ruffles.gameObject.SetActive(true);
                    break;

            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
