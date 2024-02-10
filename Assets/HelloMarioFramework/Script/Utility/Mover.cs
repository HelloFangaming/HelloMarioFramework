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
    public class Mover : MonoBehaviour
    {
        
        [Tooltip("How far to move")]
        [SerializeField]
        private Vector3 offset;
        [Tooltip("Speed to move at")]
        [SerializeField]
        private float speed = 1f;
        [Tooltip("Delay before moving")]
        [SerializeField]
        private float delay;
        [Tooltip("Wait time before moving again")]
        [SerializeField]
        private float pause = 1f;

        private Vector3 start;
        private Vector3 end;
        private bool move = true;
        private bool wait = false;

        void Start()
        {
            start = transform.localPosition;
            end = start + offset;
            if (delay > 0f)
            {
                wait = true;
                StartCoroutine(WaitToMove(delay));
            }
        }

        //Moving platforms
        void FixedUpdate()
        {
            if (!wait)
            {
                if (move)
                {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, end, speed * Time.fixedDeltaTime);
                    if (transform.localPosition == end)
                    {
                        move = false;
                        wait = true;
                        StartCoroutine(WaitToMove(pause));
                    }
                }
                else
                {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, start, speed * Time.fixedDeltaTime);
                    if (transform.localPosition == start)
                    {
                        move = true;
                        wait = true;
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
