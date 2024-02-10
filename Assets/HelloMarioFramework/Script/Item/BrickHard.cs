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
    public class BrickHard : MonoBehaviour
    {

        [SerializeField]
        private GameObject breakAnimation;

        public void BreakBrick()
        {
            GameObject o = Instantiate(breakAnimation);
            o.transform.position = transform.position;
            o.transform.rotation = transform.rotation;
            o.transform.localScale = transform.localScale;
            Destroy(gameObject);
        }

    }
}
