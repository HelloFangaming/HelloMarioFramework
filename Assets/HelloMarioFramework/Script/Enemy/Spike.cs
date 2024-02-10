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
    public class Spike : Stompable
    {

        //Components
        private Animator animator;
        private Collider myCollider;

        //Animator hash values
        private static int throwHash = Animator.StringToHash("Throw");
        private static int stompHash = Animator.StringToHash("Stomp");

        //Thing to throw
        [SerializeField]
        private GameObject thingToThrow;

        //Start delay
        [SerializeField]
        private float delay = 0f;
        
        void Start()
        {
            animator = GetComponent<Animator>();
            myCollider = GetComponent<Collider>();
            if (delay > 0) StartCoroutine(DelayThrow());
            else StartCoroutine(Throw());
        }

        //What to do when stomped. Override this.
        protected override void WhenStomped()
        {
            animator.SetBool(stompHash, true);
            myCollider.enabled = false;
        }

        //Throw balls
        private IEnumerator Throw()
        {
            yield return new WaitForSeconds(6.8f);
            animator.SetBool(throwHash, true);
            GameObject o = Instantiate(thingToThrow);
            o.transform.position = transform.position - transform.forward * 0.3f;
            o.transform.rotation = transform.rotation;
            o.AddComponent<SpikeSpawn>();
            yield return new WaitForSeconds(0.2f);
            animator.SetBool(throwHash, false);
            StartCoroutine(Throw());
        }

        private IEnumerator DelayThrow()
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(Throw());
        }

    }
}
