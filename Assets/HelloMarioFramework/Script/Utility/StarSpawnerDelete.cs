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
    public class StarSpawnerDelete : MonoBehaviour
    {

        [Tooltip("Spawn a star after all objects in list are deleted (Place star as child object of this)")]
        [SerializeField]
        private GameObject[] listOfObjects;

        void Start()
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        //Spawn the star after all objects in list are gone
        void FixedUpdate()
        {
            bool b = false;
            for (int i = 0; i < listOfObjects.Length; i++)
            {
                if (listOfObjects[i] != null) b = true;
            }

            if (!b)
            {
                MusicControl.singleton.StarAppears();
                transform.GetChild(0).gameObject.SetActive(true);
                Destroy(this);
            }
        }

    }
}
