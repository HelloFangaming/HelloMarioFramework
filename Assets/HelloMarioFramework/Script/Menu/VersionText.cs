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
using UnityEngine.UI;

namespace HelloMarioFramework
{
    public class VersionText : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Text>().text = "v" + Application.version;
        }
    }
}
