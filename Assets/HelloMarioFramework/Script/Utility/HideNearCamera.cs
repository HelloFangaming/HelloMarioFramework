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
    public class HideNearCamera : MonoBehaviour
    {

        private Transform cam;
        private GameObject hideMe;
        private bool hidden = false;
        private bool actuallyHidden = false;
        private float radius = 1f;
        private float height = 1f;
        private Vector3 originalScale;
        
        void Start()
        {
            cam = Camera.main.transform;
            hideMe = transform.GetChild(0).gameObject;

            //Hide when near this capsule collider (Place this component on a game object with a capsule collider!)
            CapsuleCollider col = GetComponent<CapsuleCollider>();
            radius = col.radius;
            height = col.height;

            originalScale = hideMe.transform.localScale;
        }
        
        void LateUpdate()
        {
            //Check if cam is within y zone
            if (cam.position.y > transform.position.y && cam.position.y < transform.position.y + height)
            {
                Vector2 distance = new Vector2(cam.position.x - transform.position.x, cam.position.z - transform.position.z);
                hidden = (distance.sqrMagnitude < (radius) * 2);
            }
            else hidden = false;

            if (actuallyHidden != hidden)
            {
                actuallyHidden = hidden;
                if (hidden) hideMe.transform.localScale = Vector3.zero;
                else hideMe.transform.localScale = originalScale;
            }
        }

    }
}
