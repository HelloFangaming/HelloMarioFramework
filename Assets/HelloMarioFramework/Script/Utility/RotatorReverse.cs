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
    public class RotatorReverse : MonoBehaviour
    {

        [SerializeField]
        public Vector3 rotationSpeed = Vector3.up;
        [SerializeField]
        public float angle = 180f;
        [SerializeField]
        public float delay = 0f;
        [SerializeField]
        public float pause = 1f;

        private bool move = true;
        private bool wait = false;
        private Vector3 totalRotation = Vector3.zero;
        private Quaternion startRotation;

        void Start()
        {
            rotationSpeed *= 100; //Makes this feel more consistent with speeds used in Mover
            startRotation = transform.localRotation;
            if (delay > 0f)
            {
                wait = true;
                StartCoroutine(WaitToMove(delay));
            }
        }

        void FixedUpdate()
        {
            if (!wait)
            {
                if (move)
                {
                    transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
                    totalRotation += rotationSpeed * Time.fixedDeltaTime;
                    if (totalRotation.magnitude >= angle)
                    {
                        move = false;
                        wait = true;
                        transform.Rotate(-rotationSpeed.normalized * (totalRotation.magnitude - angle));
                        totalRotation = Vector3.zero;
                        StartCoroutine(WaitToMove(pause));
                    }
                }
                else
                {
                    transform.Rotate(-rotationSpeed * Time.fixedDeltaTime);
                    totalRotation += rotationSpeed * Time.fixedDeltaTime;
                    if (totalRotation.magnitude >= angle)
                    {
                        move = true;
                        wait = true;
                        transform.localRotation = startRotation;
                        totalRotation = Vector3.zero;
                        StartCoroutine(WaitToMove(pause));
                    }
                }
            }
        }

        public IEnumerator WaitToMove(float w)
        {
            yield return new WaitForSeconds(w);
            wait = false;
        }

    }
}
