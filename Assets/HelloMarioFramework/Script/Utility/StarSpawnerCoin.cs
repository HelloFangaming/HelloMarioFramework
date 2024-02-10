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
    public class StarSpawnerCoin : MonoBehaviour
    {
        
        [Tooltip("Spawn a star after all coins this color are collected)")]
        [SerializeField]
        private CoinColor.Color color;

        //Game
        private int coins = 0;
        private int totalCoins = 0;
        
        void Start()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            //Find the coins that need to be collected
            foreach (CoinColor c in FindObjectsByType<CoinColor>(FindObjectsSortMode.None))
            {
                if (c.Track(this, color)) totalCoins++;
            }
        }

        public int Notify()
        {
            coins++;
            if (coins >= totalCoins)
            {
                MusicControl.singleton.StarAppears();
                transform.GetChild(0).gameObject.SetActive(true);
            }
            return coins;
        }

    }
}
