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
    public class LookAtCam : MonoBehaviour
    {

        //Components
        private Transform cam;
        
        void Start()
        {
            cam = Camera.main.transform;
        }

        //Look towards the camera
        void FixedUpdate()
        {
            transform.LookAt(cam.position);
        }

    }
}
