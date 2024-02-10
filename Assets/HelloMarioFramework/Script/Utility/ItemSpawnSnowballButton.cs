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
    public class ItemSpawnSnowballButton : ItemSpawnButton
    {

        //Reset inital position. Override this.
        protected override void ResetPosition()
        {
            transform.localPosition = start;
            transform.localRotation = startRot;
            myRigidBody.velocity = Vector3.zero;
            myRigidBody.angularVelocity = Vector3.zero;

            //Special support for resetting snowballs
            transform.localScale = Vector3.one;
            myRigidBody.mass = 1f;

            Snowball snow = GetComponent<Snowball>();
            if (snow != null) Destroy(snow);
            gameObject.AddComponent<Snowball>();
        }

    }
}
