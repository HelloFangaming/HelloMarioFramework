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
using Cinemachine;

namespace HelloMarioFramework
{
    public class CameraTriggerActivator : MonoBehaviour
    {

        //Virtual camera to enable/disable
        [SerializeField]
        private CinemachineVirtualCamera cam;

        //If the player is in view
        private bool inView = false;
        
        void Start()
        {
            cam.m_Priority = 0;
        }

        //Collision with player
        private void OnTriggerEnter(Collider collision)
        {
            if (!inView)
            {
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {
                    inView = true;
                    cam.m_Priority = 15;
                }
            }
        }

        //Collision with player
        private void OnTriggerExit(Collider collision)
        {
            if (inView)
            {
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {
                    inView = false;
                    cam.m_Priority = 0;
                }
            }
        }

    }
}
