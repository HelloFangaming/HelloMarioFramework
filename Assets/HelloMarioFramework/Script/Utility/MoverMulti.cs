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
    public class MoverMulti : MonoBehaviour
    {

        [Tooltip("How far to move")]
        [SerializeField]
        private Vector3 offset;
        [Tooltip("How far to move after the first move")]
        [SerializeField]
        private Vector3 offset2;
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
        private int move = 0;
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

        void FixedUpdate()
        {
            if (!wait)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, end, speed * Time.fixedDeltaTime);
                if (transform.localPosition == end)
                {
                    if (move == 0)
                    {
                        end = transform.localPosition + offset2;
                        move++;
                    }
                    else if (move == 1)
                    {
                        end = transform.localPosition - offset;
                        move++;
                    }
                    else if (move == 2)
                    {
                        end = start;
                        move++;
                    }
                    else
                    {
                        end = start + offset;
                        move = 0;
                    }
                    wait = true;
                    StartCoroutine(WaitToMove(pause));
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
