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
    public class JumpThroughPlatform : MonoBehaviour
    {

        //Components
        private Animator animator;

        //Animator hash values
        private static int jumpThroughHash = Animator.StringToHash("Floof");

        //Game
        private bool trigger = false;
        
        void Start()
        {
            animator = transform.GetComponent<Animator>();
        }

        void OnTriggerExit()
        {
            if (!trigger) StartCoroutine(Triggered());
        }

        private IEnumerator Triggered()
        {
            trigger = true;
            animator.SetBool(jumpThroughHash, true);

            yield return new WaitForSeconds(0.5f);

            trigger = false;
            animator.SetBool(jumpThroughHash, false);
        }

    }
}
