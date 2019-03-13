using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class AdBreakingNews : Ad
    {
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (!_DelayIsPassed)
                return;
            SetDirection();
            _Direction *= MaxSpeed * Time.deltaTime;
            gameObject.transform.Translate(_Direction);
        }
    }
}