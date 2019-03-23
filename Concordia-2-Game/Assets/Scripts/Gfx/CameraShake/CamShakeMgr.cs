using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    public class CamShakeMgr : MonoBehaviour
    {
        public enum Intensity
        {
            TINY,
            SMALL,
            MEDIUM,
            BIG,
        }


        static protected CamShakeMgr Instance;

        static public CamShakeMgr Get()
        {
            return Instance;
        }


        protected CamShake shake;

        void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            shake = Camera.main.GetComponent<CamShake>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Shake(Intensity intensity)
        {
            if (intensity == Intensity.TINY)
                ShakeTiny();
            else if (intensity == Intensity.SMALL)
                ShakeSmall();
            else if (intensity == Intensity.MEDIUM)
                ShakeMedium();
            else if (intensity == Intensity.BIG)
                ShakeBig();
        }

        public void ShakeTiny()
        {
            shake.ShakeCamera(0.3f, 0.2f);
        }

        public void ShakeSmall()
        {
            shake.ShakeCamera(0.5f, 0.5f);
        }

        public void ShakeMedium()
        {
            shake.ShakeCamera(0.7f, 0.7f);
        }

        public void ShakeBig()
        {
            shake.ShakeCamera(1.0f, 1.3f);
        }
    }
}
