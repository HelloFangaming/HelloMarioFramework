/*
 *  Copyright (c) 2024 Hello Fangaming
 *
 *  Use of this source code is governed by an MIT-style
 *  license that can be found in the LICENSE file or at
 *  https://opensource.org/licenses/MIT.
 *  
 * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HelloMarioFramework
{
    public class SpriteBlinker : MonoBehaviour
    {
        //Components
        private Image image;
        [SerializeField]
        public float delay = 0.5f;
        
        void Start()
        {
            image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            StartCoroutine(Animate());
        }

        //Make the sprite blink
        private IEnumerator Animate()
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                image.enabled = false;
                yield return new WaitForSeconds(delay);
                image.enabled = true;
            }
        }

    }
}
