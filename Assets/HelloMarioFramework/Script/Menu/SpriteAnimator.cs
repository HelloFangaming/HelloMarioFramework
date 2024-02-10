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
    public class SpriteAnimator : MonoBehaviour
    {
        //Components
        private Image image;
        private int i;

        //Sprites
        [SerializeField]
        private Sprite[] sprites;
        [SerializeField]
        private float delay = 0f;

        // Start is called before the first frame update
        void Start()
        {
            image = GetComponent<Image>();
            if (delay > 0f) StartCoroutine(Delay());
            else StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            yield return new WaitForSeconds(0.1f);
            i++;
            if (i >= sprites.Length) i = 0;
            image.sprite = sprites[i];
            StartCoroutine(Animate());
        }

        private IEnumerator Delay()
        {
            image.enabled = false;
            yield return new WaitForSeconds(delay);
            image.enabled = true;
            StartCoroutine(Animate());
        }

    }
}
