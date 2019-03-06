using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class Ad : MonoBehaviour
    {
        public enum ScrollDirection
        {
            up,
            down,
            left,
            right,
        }

        public float Speed = 100f;

        public float Delay = 0f;

        public ScrollDirection ScrollDir = ScrollDirection.left;

        private Vector3 _Direction;
        private bool _DelayIsPassed = false;

        void Start()
        {
            // A bit ugly but efficient and simple
            switch (ScrollDir)
            {
                case ScrollDirection.left:
                    _Direction = Vector3.left;
                    break;
                case ScrollDirection.right:
                    _Direction = Vector3.right;
                    break;
                case ScrollDirection.up:
                    _Direction = Vector3.up;
                    break;
                case ScrollDirection.down:
                    _Direction = Vector3.down;
                    break;
            }

            _Direction *= Speed * Time.deltaTime;
            StartCoroutine("TimerDelay");
        }

        private IEnumerator TimerDelay()
        {
            yield return new WaitForSeconds(Delay);
            _DelayIsPassed = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (_DelayIsPassed)
            {
                gameObject.transform.Translate(_Direction);
            }
        }
    }

}
