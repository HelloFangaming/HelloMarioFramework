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
using UnityEngine.UI;

namespace HelloMarioFramework
{
    public class HudControl : MonoBehaviour
    {

        //Components
        [SerializeField]
        private Text starText;
        [SerializeField]
        private Text coinText;

        //Optimization
        private int prevStar = -1;
        private int prevCoin = 0;
#if UNITY_EDITOR
        //Null check
        void Awake()
        {
            SaveData.NullCheck();
        }
#endif
        //Update star and coin count
        void LateUpdate()
        {
            if (prevCoin != SaveData.save.GetCoins())
            {
                prevCoin = SaveData.save.GetCoins();
                coinText.text = SaveData.save.GetCoins().ToString();
            }
            if (prevStar != SaveData.save.GetStarCount())
            {
                prevStar = SaveData.save.GetStarCount();
                starText.text = SaveData.save.GetStarCount().ToString();
            }
        }
    }
}
