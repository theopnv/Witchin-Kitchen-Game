using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

}
