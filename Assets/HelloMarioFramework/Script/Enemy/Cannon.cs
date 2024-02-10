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
    public class Cannon : MonoBehaviour
    {

        //Bullet prefab
        [SerializeField]
        private GameObject bullet;

        //Start delay
        [SerializeField]
        private float delay = 0f;
        
        void Start()
        {
            if (delay > 0) StartCoroutine(DelayLaunch());
            else StartCoroutine(Launch());
        }

        //Launch
        private IEnumerator Launch()
        {
            yield return new WaitForSeconds(7f);

            //If player is nearby, launch
            if (Player.singleton.CanBeChased(transform.position, 25f))
            {
                GameObject o = Instantiate(bullet);
                o.transform.position = transform.position + Vector3.up * 0.75f * transform.localScale.y;
                o.transform.rotation = transform.rotation;
                o.transform.localScale = transform.localScale;
            }

            StartCoroutine(Launch());
        }

        private IEnumerator DelayLaunch()
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(Launch());
        }

    }
}
