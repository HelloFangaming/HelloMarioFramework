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
    public class Snowball : MonoBehaviour
    {

        //Components
        private Rigidbody myRigidBody;

        private Vector3 position;
        private float delta = 0f;

        private bool onGround = true;
        private int collisionCount = 1;
        
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody>();
            position = transform.position;
        }
        
        void FixedUpdate()
        {

            //Collision delay
            if (collisionCount > 0)
            {
                collisionCount--;
                if (collisionCount == 0)
                {
                    onGround = false;
                }
            }

            //Calculate how much it moved on ground
            if (onGround) delta += (position - transform.position).magnitude;
            position = transform.position;

            //If it moved
            if (delta > 0.01f)
            {
                delta *= 0.05f;
                transform.localPosition += Vector3.up * delta;
                transform.localScale += Vector3.one * delta;
                myRigidBody.mass += delta;

                //Limit snowball size
                if (myRigidBody.mass > 4f) Destroy(this);
                else delta = 0f;
            }

        }

        private void OnCollisionStay(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.6f)
                {
                    if (!onGround)
                    {
                        onGround = true;
                    }
                    collisionCount = 8;
                    break;
                }
            }
        }

    }
}
