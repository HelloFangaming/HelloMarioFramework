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
    public class AndroidInstantiatePrefab : MonoBehaviour
    {

        [SerializeField]
        private GameObject prefab;
        
#if (UNITY_ANDROID || UNITY_IOS)
        //Instantiate on mobile only
        void Start()
        {
            Instantiate(prefab, transform);
        }
#endif

    }
}
