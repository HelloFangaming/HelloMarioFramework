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
    public class StarSelectDeleter : MonoBehaviour
    {
        [Tooltip("Whether to delete this gameobject if the star name was selected in the file select menu")]
        [SerializeField]
        private bool deleteWhenEqual = false;
        [Tooltip("Name of the star selected in the file select menu to check for")]
        [SerializeField]
        private string starID = "";
        
        void Start()
        {
            if ((PauseMenu.levelSubText == starID) == deleteWhenEqual)
                Destroy(gameObject);
        }
        
    }
}
