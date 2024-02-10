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
    public class WarpBoxLocked : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] listOfObjects;
        [SerializeField]
        private GameObject smoke;
        
        void FixedUpdate()
        {
            bool b = false;
            for (int i = 0; i < listOfObjects.Length; i++)
            {
                if (listOfObjects[i] != null) b = true;
            }

            if (!b)
            {
                //0th object is locked box
                transform.GetChild(0).gameObject.SetActive(false);

                //1th object is warp box nested prefab
                transform.GetChild(1).gameObject.SetActive(true);

                //Smoke
                GameObject o = Instantiate(smoke);
                o.transform.position = transform.position + new Vector3(0f, 0.762f, 0f);
                o.transform.localScale = Vector3.one * 2f;

                Destroy(this);
            }
        }

    }
}
