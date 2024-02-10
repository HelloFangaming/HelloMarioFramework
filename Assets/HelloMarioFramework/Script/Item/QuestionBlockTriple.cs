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
    public class QuestionBlockTriple : QuestionBlock
    {

        //Sprout
        [SerializeField]
        private Sprout[] sproutList;

        //Before block moves up, make coins here
        protected override void BlockHitCoin()
        {
            for (int i = 0; i < sproutList.Length; i++)
            {
                if (sproutList[i] == Sprout.Coin || sproutList[i] == Sprout.MultiCoin)
                {
                    GameObject o = Instantiate(coin, transform);
                    o.transform.localPosition = new Vector3(i - 1, 0.5f, 0f);
                    SaveData.save.CollectCoin();
                }
            }
        }

        //Block hit
        protected override void BlockHit()
        {
            for (int i = 0; i < sproutList.Length; i++)
            {
                switch (sproutList[i])
                {
                    case Sprout.Coin:
                        Empty();
                        break;
                    case Sprout.MultiCoin:
                        if (timer == 2) Empty();
                        else
                        {
                            bumpable = true;
                            if (timer == 0)
                            {
                                timer = 1;
                                StartCoroutine(Timer());
                            }
                        }
                        break;
                    case Sprout.Mushroom:
                        GameObject o = Instantiate(mushroom, transform);
                        o.transform.localPosition = new Vector3(i - 1, 0f, 0f);
                        Empty();
                        break;
                }
            }
        }

    }
}
