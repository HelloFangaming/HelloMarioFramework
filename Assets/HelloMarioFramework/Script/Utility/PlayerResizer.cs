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
    public class PlayerResizer : MonoBehaviour
    {

        //Game
        private bool shrink = true;

        //Resize player (Used by warp box)
        private void FixedUpdate()
        {
            if (shrink)
            {
                if (transform.localScale.y > 0f)
                    transform.localScale -= Vector3.one * 5f * Time.fixedDeltaTime;
                else transform.localScale = Vector3.zero;
            }
            else
            {
                if (transform.localScale.y < 1f)
                    transform.localScale += Vector3.one * 5f * Time.fixedDeltaTime;
                else
                {
                    transform.localScale = Vector3.one;
                    Destroy(this);
                }
            }
        }

        public void UndoShrink()
        {
            transform.localScale = Vector3.zero;
            shrink = false;
        }

    }
}
