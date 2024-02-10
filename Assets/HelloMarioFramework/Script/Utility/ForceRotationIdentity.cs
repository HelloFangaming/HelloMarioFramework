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
    public class ForceRotationIdentity : MonoBehaviour
    {
        //Force the transform to zero world rotation
        void FixedUpdate()
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
