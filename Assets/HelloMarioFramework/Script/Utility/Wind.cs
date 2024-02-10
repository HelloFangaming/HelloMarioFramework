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
    public class Wind : MonoBehaviour
    {

        //Wind force
        [SerializeField]
        private float force = 18f;

        //Blow the player and physics objects
        private void OnTriggerStay(Collider collision)
        {
            Rigidbody r = collision.transform.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.AddForce(transform.forward * force);
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {
                    //Fix ground pound glitch
                    if (p.IsPoundStart() || p.IsPound())
                    {
                        p.UndoPound();
                        r.velocity = Vector3.zero;
                    }
                    p.BreakSpeedCap();
                    p.Knockback(transform.forward * -force);
                }
            }
        }

    }
}
