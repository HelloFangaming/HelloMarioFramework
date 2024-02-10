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
    public class Brick : MonoBehaviour
    {

        [SerializeField]
        private GameObject breakAnimation;

        //Break on contact
        private void OnCollisionEnter(Collision collision)
        {
            Player p = collision.transform.GetComponent<Player>();
            if (p != null)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    if ((Vector3.Dot(contact.normal, Vector3.up) > 0.9f) || (p.IsPound() && Vector3.Dot(contact.normal, Vector3.down) > 0.9f))
                    {
                        BreakBrick();
                        break;
                    }
                }
            }
        }

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
