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
    public class FadeControl : MonoBehaviour
    {
        public static FadeControl singleton;
        private float alpha = 1.25f;
        private float color = 0f;
        private float delta = 1f;
        private bool fadingIn = false;
        private Image rect;
        
        void Start()
        {
            singleton = this;
            rect = GetComponent<Image>();
        }

        //Fade
        void LateUpdate()
        {
            if (fadingIn)
            {
                alpha += delta * Time.deltaTime;
                rect.color = new Color(color, color, color, Mathf.Min(alpha, 1f));
                if (alpha > 1.1f) fadingIn = false;
            }
            else if (alpha > 0f)
            {
                alpha -= delta * Time.deltaTime;
                rect.color = new Color(color, color, color, Mathf.Min(alpha, 1f));
            }
        }

        public void Fade()
        {
            alpha = 0f;
            color = 0f;
            delta = 2f;
            fadingIn = true;
        }

        public void FadeWhite()
        {
            alpha = 0f;
            color = 1f;
            delta = 2f;
            fadingIn = true;
        }

        public void FadeToLoadingScreen()
        {
            Fade();
            StartCoroutine(LoadScreen());
        }

        private IEnumerator LoadScreen()
        {
            gameObject.AddComponent<LoadingScreen>();
            yield return new WaitForSeconds(0.5f);
            rect.color = new Color(color, color, color, 1f);
            transform.GetChild(0).gameObject.SetActive(true); //First child is loading indicator
            enabled = false;
        }
    }
}
