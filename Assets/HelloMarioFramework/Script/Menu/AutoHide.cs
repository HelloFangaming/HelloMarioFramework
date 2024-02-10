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

namespace HelloMarioFramework
{
    public class AutoHide : MonoBehaviour
    {
        
        [Tooltip("Time to display before hiding")]
        [SerializeField]
        public float autoHideAfter = 1f;

        public void ShowMe()
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(HideMe());
        }

        private IEnumerator HideMe()
        {
            yield return new WaitForSeconds(autoHideAfter);
            gameObject.SetActive(false);
        }

    }
}
