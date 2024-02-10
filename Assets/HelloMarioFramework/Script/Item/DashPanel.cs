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
    public class DashPanel : MonoBehaviour
    {

        //Components
        private static DashPanel animator;
        private static Vector2 uvAnimationRate = new Vector2(1f / 14f, -1f / 14f);
        private Material material;

        //Audio clips
        [SerializeField]
        private AudioClip dashSFX;
        
        void Start()
        {
            //Animate the dash panel material
            if (animator == null)
            {
                animator = this;
                material = GetComponentInChildren<Renderer>().sharedMaterial;
                StartCoroutine(PixelAnimate());
            }
        }

        //Give Mario a speed boost
        private void OnTriggerEnter(Collider collision)
        {
            Player p = collision.transform.GetComponent<Player>();
            if (p != null)
            {
                p.PlaySound(dashSFX);
                p.BreakSpeedCap();
                p.Knockback(p.transform.forward * -18f);
            }
        }

        //Maintain speed boost
        private void OnTriggerStay(Collider collision)
        {
            Player p = collision.transform.GetComponent<Player>();
            if (p != null)
            {
                p.BreakSpeedCap();
                p.Knockback(p.transform.forward * -18f);
            }
        }

        //Animate the material
        private IEnumerator PixelAnimate()
        {
            yield return new WaitForSeconds(0.1f);
            material.mainTextureOffset += uvAnimationRate;
            StartCoroutine(PixelAnimate());
        }

        private void OnDestroy()
        {
            if (animator == this)
                material.mainTextureOffset = Vector2.zero;
        }

    }
}
