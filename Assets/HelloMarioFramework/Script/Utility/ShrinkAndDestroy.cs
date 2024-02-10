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
    public class ShrinkAndDestroy : MonoBehaviour
    {
        //Resize and destroy self
        private void FixedUpdate()
        {
            if (transform.localScale.y > 0f)
                transform.localScale -= Vector3.one * 5f * Time.fixedDeltaTime;
            else Destroy(gameObject);
        }
    }
}
