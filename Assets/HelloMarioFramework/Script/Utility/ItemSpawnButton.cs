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
    public class ItemSpawnButton : MonoBehaviour
    {
        [SerializeField]
        private ButtonHandler button;
        [SerializeField]
        private GameObject smoke;

        private bool active = false;
        protected Vector3 start;
        protected Quaternion startRot;
        protected Rigidbody myRigidBody;

        void Start()
        {
            start = transform.localPosition;
            startRot = transform.localRotation;
            myRigidBody = GetComponent<Rigidbody>();
#if UNITY_EDITOR
            if (button == null)
                Debug.Log("Hello Mario Framework: ItemSpawnButton at " + transform.position + " is missing a button!");
#endif
        }

        void FixedUpdate()
        {
            if (!active && button.IsActive())
            {
                active = true;

                //Create smoke on old position
                CreateSmoke();

                //Return to inital position
                ResetPosition();

                //Create smoke on new position
                CreateSmoke();

            }
            else if (active && !button.IsActive())
            {
                active = false;
            }
        }

        private void CreateSmoke()
        {
            GameObject o = Instantiate(smoke);
            o.transform.position = transform.position;
            o.transform.rotation = transform.rotation;
            o.transform.localScale = o.transform.localScale * transform.localScale.x * 6f;
        }

        //Reset inital position. Override this.
        protected virtual void ResetPosition()
        {
            transform.localPosition = start;
            transform.localRotation = startRot;
            myRigidBody.velocity = Vector3.zero;
            myRigidBody.angularVelocity = Vector3.zero;
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Exclamation.png", true);
        }

    }
}
