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
    public class HazardCSG : MonoBehaviour
    {

        [Tooltip("Whether to use burn voice")]
        [SerializeField]
        private bool burn;

        //Find the RealtimeCSG model's mesh collider, and add the hazard component to it
        void Start()
        {
            GetComponentInChildren<MeshCollider>().gameObject.AddComponent<Hazard>().burn = burn;
        }

    }
}
