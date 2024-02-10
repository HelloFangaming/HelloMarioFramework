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
    public class StarClearDeleter : MonoBehaviour
    {
        [Tooltip("Whether to delete this gameobject if the star was collected or not")]
        [SerializeField]
        private bool deleteWhenClear = false;
        [Tooltip("Name of the star to check for")]
        [SerializeField]
        private string starID = "";
        
        void Start()
        {
            if (SaveData.save.CheckCollection(starID) == deleteWhenClear)
                Destroy(gameObject);
        }
    }
}
