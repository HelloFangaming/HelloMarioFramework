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
    public class HealthCircle : MonoBehaviour
    {

        //Components
        private Image image;
        private Text text;
        private int prevHealth = -1;
        
        [Tooltip("Health circle sprites")]
        [SerializeField]
        private Sprite[] health;
        
        void Start()
        {
            image = GetComponent<Image>();
            text = transform.GetChild(0).GetComponent<Text>(); //First child is text
        }
        
        void LateUpdate()
        {
            int i = Player.singleton.GetHealth();
            if (prevHealth != i)
            {
                prevHealth = i;
                image.sprite = health[i];
                text.text = i.ToString();
            }
        }

    }
}
