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

        public float MaxSpeed;
        public float MinSpeed;

        public float Delay;

        public ScrollDirection ScrollDir = ScrollDirection.left;

        protected Vector3 _Direction;
        protected bool _DelayIsPassed = false;

        protected float _Speed;
        protected float _Increment = 15f;

        protected float _CenterSquare = 60f;

        protected virtual void Start()
        {
            SetDirection();
            _Speed = MinSpeed;
            _Direction *= _Speed * Time.deltaTime;
            StartCoroutine("TimerDelay");
        }

        private IEnumerator TimerDelay()
        {
            yield return new WaitForSeconds(Delay);
            _DelayIsPassed = true;
        }


        protected void SetDirection()
        {
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
        }

        private bool InMiddleVerticalDir()
        {
            var height = GetComponent<RectTransform>().rect.height;
            return transform.localPosition.y < _CenterSquare && transform.localPosition.y > -_CenterSquare;
        }

        private bool InMiddleHorizontalDir()
        {
            var width = GetComponent<RectTransform>().rect.width;
            if (transform.localPosition.x < _CenterSquare && transform.localPosition.x > -_CenterSquare)
                return true;
            return false;
        }

        protected bool InMiddle()
        {
            switch (ScrollDir)
            {
                case ScrollDirection.left:
                case ScrollDirection.right:
                    return InMiddleHorizontalDir();
                case ScrollDirection.up:
                case ScrollDirection.down:
                    return InMiddleVerticalDir();
                default:
                    return false;
            }
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (_DelayIsPassed)
            {
                SetDirection();
                if (InMiddle())
                {
                    _Increment = 15f;
                    _Speed = MinSpeed;
                }
                else
                    _Speed += _Increment;
                _Speed = Mathf.Clamp(_Speed, MinSpeed, MaxSpeed);
                _Direction *= _Speed * Time.deltaTime;
                gameObject.transform.Translate(_Direction);
            }
        }
    }

}
