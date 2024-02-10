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
    public class HammerSpawn : MonoBehaviour
    {

        //Components
        private Rigidbody myRigidBody;

        //Game
        private bool grow = true;
        private bool full = false;
        
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody>();
            myRigidBody.isKinematic = true;
            transform.localScale = Vector3.zero;
        }
        
        void FixedUpdate()
        {

            //Grow
            if (grow)
            {
                if (!full)
                {
                    transform.localScale += Vector3.one * 3f * Time.fixedDeltaTime;
                    transform.position += Vector3.up * 3f * Time.fixedDeltaTime;
                    if (transform.localScale.y >= 1)
                    {
                        full = true;
                        transform.localScale = Vector3.one;
                        StartCoroutine(Throw());
                    }
                }
            }

            //Shrink
            else
            {
                transform.localScale -= Vector3.one * 1f * Time.fixedDeltaTime;// 0.02f;
                if (transform.localScale.y <= 0) Destroy(gameObject);
            }

        }

        private IEnumerator Throw()
        {
            //Throw
            yield return new WaitForSeconds(0.3f);
            myRigidBody.isKinematic = false;
            myRigidBody.velocity = transform.forward * myRigidBody.mass * 10f + Vector3.up * 5f;
            myRigidBody.angularVelocity = Vector3.right * 6f;


            //Despawn
            yield return new WaitForSeconds(4f);
            grow = false;

        }

    }
}
