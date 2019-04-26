using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class NewtEyeManager : MonoBehaviour
    {
        private PickableObject _pickableObject;

        // Start is called before the first frame update
        void Start()
        {
            _pickableObject = GetComponent<PickableObject>();
            //StartCoroutine(RemoveFromScene());
        }

        //private IEnumerator RemoveFromScene()
        //{
        //    yield return new WaitForSeconds(15f);
        //    if (_pickableObject.IsHeld())
        //    {
        //        yield return RemoveFromScene();
        //    }
        //    else
        //    {
        //        Destroy(gameObject);
        //    }
        //}

    }

}
