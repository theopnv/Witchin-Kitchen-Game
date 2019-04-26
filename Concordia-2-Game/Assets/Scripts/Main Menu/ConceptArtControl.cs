using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace con2.main_menu
{

    public class ConceptArtControl : MonoBehaviour
    {
        public Sprite[] conceptArts;
        private int currentImageIndex = 0;
        public Image conceptArtImage;
        private bool changing = false;

        // Update is called once per frame
        void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            if (!changing && horizontal < -0.9 || Input.GetKeyDown("left"))
            {
                changing = true;
                SwitchPics(-1);
                StartCoroutine(WaitOnChange());
            }
            else if (!changing && horizontal > 0.9 || Input.GetKeyDown("right"))
            {
                changing = true;
                SwitchPics(1);
                StartCoroutine(WaitOnChange());
            }
        }

        private void SwitchPics(int changeDir)
        {
            currentImageIndex += changeDir;
            var len = conceptArts.Length;
            conceptArtImage.sprite = conceptArts[((currentImageIndex % len) + len)%len];
        }

        private IEnumerator WaitOnChange()
        {
            yield return new WaitForSeconds(0.3f);
            changing = false;
        }
    }

}
