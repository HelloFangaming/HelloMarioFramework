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
    public class HatCoinSpawner : MonoBehaviour
    {

        //Audio clips
        [SerializeField]
        private AudioClip poofSFX;

        [SerializeField]
        private GameObject smoke;

        private Vector3 position;
        private float delta = 0f;

        [SerializeField]
        private GameObject coin;

        private bool started = false;
        
        void Start()
        {
            position = transform.position;
        }
        
        void FixedUpdate()
        {
            //Calculate how much it moved
            delta += (position - transform.position).magnitude;
            position = transform.position;

            //Spawn coins
            if (delta > 3f)
            {
                GameObject o = Instantiate(coin);
                o.transform.position = transform.position + Vector3.up * 0.75f;
                SaveData.save.CollectCoin();
                if (!started)
                {
                    started = true;
                    StartCoroutine(DestroyTime());
                }
                delta = 0f;
            }
        }

        private IEnumerator DestroyTime()
        {
            yield return new WaitForSeconds(8f);
            GameObject o = Instantiate(smoke);
            o.transform.position = transform.position;
            o.transform.rotation = transform.rotation;
            o.transform.localScale = Vector3.one * 2f;
            Player.singleton.PlaySound(poofSFX);
            Player.singleton.RemoveHat();
        }

    }
}
