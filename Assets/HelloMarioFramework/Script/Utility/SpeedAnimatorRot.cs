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
    public class SpeedAnimatorRot : SpeedAnimator
    {

        //Set speed value in animator to the speed this gameobject is rotating
        protected override void FixedUpdate()
        {
            if (prevPosition != transform.position)
            {
                transform.parent.localRotation = Quaternion.LookRotation(transform.position - prevPosition);
            }
            base.FixedUpdate();
        }

    }
}
