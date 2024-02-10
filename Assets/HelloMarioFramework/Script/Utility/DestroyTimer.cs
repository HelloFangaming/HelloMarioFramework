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
    public class DestroyTimer : MonoBehaviour
    {

        [SerializeField]
        private float destroyTime;

        void Start()
        {
            StartCoroutine(WaitAndDestroy());
        }

        //Destroy after x seconds
        private IEnumerator WaitAndDestroy()
        {
            yield return new WaitForSeconds(destroyTime);
            Destroy(gameObject);
        }

    }
}
