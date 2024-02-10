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
    public class CoinBlockAnim : MonoBehaviour
    {

        private float upSpeed = 10f;
        
        void FixedUpdate()
        {
            if (upSpeed > 0f)
            {
                transform.position += Vector3.up * upSpeed * Time.fixedDeltaTime;
                upSpeed -= 17.5f * Time.fixedDeltaTime;
            }
            else
                Destroy(gameObject);
        }

    }
}
