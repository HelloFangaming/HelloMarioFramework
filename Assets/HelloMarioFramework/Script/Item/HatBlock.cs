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
    public class HatBlock : MonoBehaviour
    {

        //Components
        private Collider myCollider;

        //Audio clips
        [SerializeField]
        private AudioClip bumpSFX;

        //Propeller (Optional)
        [SerializeField]
        private Propeller propeller;

        // Start is called before the first frame update
        void Start()
        {
            myCollider = GetComponent<Collider>();
        }

        //Bump on contact
        private void OnCollisionEnter(Collision collision)
        {
            Player p = collision.transform.GetComponent<Player>();
            if (p != null && p.CanAcceptHat())
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    if (Vector3.Dot(contact.normal, Vector3.up) > 0.9f)
                    {
                        p.PlaySound(bumpSFX);
                        myCollider.enabled = false;
                        p.AcceptHat(transform, propeller);
                        Destroy(this);
                        break;
                    }
                }
            }
        }

    }
}
