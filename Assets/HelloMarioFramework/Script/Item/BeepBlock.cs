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
    public class BeepBlock : MonoBehaviour
    {

        //Beep block part that is enabled and disabled
        [SerializeField]
        private GameObject block;

        public void Swap()
        {
            block.SetActive(!block.activeSelf);
        }

    }
}