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
    public class SpeedAnimator : MonoBehaviour
    {

        //Components
        protected Animator animator;

        //Animator hash values
        protected static int speedHash = Animator.StringToHash("Speed");

        //Previous position
        protected Vector3 prevPosition;
        
        void Start()
        {
            animator = GetComponent<Animator>();
            prevPosition = transform.position;
        }

        //Set speed value in animator to the speed this gameobject is moving
        protected virtual void FixedUpdate()
        {
            animator.SetFloat(speedHash, (transform.position - prevPosition).magnitude / Time.fixedDeltaTime);
            prevPosition = transform.position;
        }

    }
}
