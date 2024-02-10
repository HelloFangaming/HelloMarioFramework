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
    public class TransformSwapper : MonoBehaviour
    {
        //Transforms
        public GameObject[] listObjs;

        //Swap between different gameobjects
        public void Change(int index)
        {
            for (int i = 0; i < listObjs.Length; i++)
            {
                listObjs[i].SetActive(i == index);
            }
        }

    }
}
