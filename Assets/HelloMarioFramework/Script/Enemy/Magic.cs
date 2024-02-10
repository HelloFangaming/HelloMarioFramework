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
    public class Magic : MonoBehaviour
    {

        //Smoke
        [SerializeField]
        private GameObject smoke;
        
        void Start()
        {
            GetComponent<Rigidbody>().velocity = (Player.singleton.hatAttachTransform.position - transform.position).normalized * 5f;
            StartCoroutine(DestroyWhenTooFar());
        }

        //Hurt player and destroy
        private void OnCollisionEnter(Collision collision)
        {
            //All collisions
            foreach (ContactPoint contact in collision.contacts)
            {
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {
                    p.Hurt(false, contact.normal);
                }
            }
            Explode();
        }

        private void Explode()
        {
            GameObject o = Instantiate(smoke);
            o.transform.position = transform.position;
            o.transform.localScale *= 0.5f;
            Destroy(gameObject);
        }

        private IEnumerator DestroyWhenTooFar()
        {
            yield return new WaitForSeconds(14f);
            Explode();
        }

    }
}
