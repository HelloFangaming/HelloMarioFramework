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
    public class StarCountDeleter : MonoBehaviour
    {
        [Tooltip("Whether to delete this gameobject if the star count is <= or >= to the specfied count")]
        [SerializeField]
        private bool deleteWhenSmaller = false;
        [Tooltip("Number of stars to check for")]
        [SerializeField]
        private int count = 0;
        
        void Start()
        {
            if ((deleteWhenSmaller && SaveData.save.GetStarCount() <= count) || (!deleteWhenSmaller && SaveData.save.GetStarCount() >= count))
                Destroy(gameObject);
        }

    }
}
