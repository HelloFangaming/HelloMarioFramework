/*
 *  Copyright (c) 2024 Hello Fangaming
 *
 *  Use of this source code is governed by an MIT-style
 *  license that can be found in the LICENSE file or at
 *  https://opensource.org/licenses/MIT.
 *  
 * */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloMarioFramework
{
    public class SnakeSwitch : MonoBehaviour
    {

        //Components
        private AudioSource audioPlayer;

        //Audio clips
        [SerializeField]
        private AudioClip pressSFX;

        //Game object to hide when switch is on
        [SerializeField]
        private GameObject[] switchOn;

        //Snake panel prefab
        [SerializeField]
        public GameObject panelPrefab;

        //Where to create the snake panels
        [SerializeField]
        private Transform startPoint;

        //List of snake behaviors
        [SerializeField]
        private SnakeBehavior[] snakePath;

        //Game
        private bool pressed = false;
        
        void Start()
        {
            audioPlayer = gameObject.AddComponent<AudioSource>();
        }
        
        void OnTriggerStay(Collider collision)
        {
            if (!pressed && !collision.isTrigger)
            {
                pressed = true;
                switchOn[0].SetActive(false);
                switchOn[1].SetActive(false);
                audioPlayer.PlayOneShot(pressSFX);
                Instantiate(panelPrefab).GetComponent<SnakePanel>().Flip(startPoint, this, 0, snakePath);
            }
        }

        //Call this to reset the switch
        public void EndSnakeSwitch()
        {
            pressed = false;
            switchOn[0].SetActive(true);
            switchOn[1].SetActive(true);
        }

        [Serializable]
        public class SnakeBehavior
        {
            public enum Direction { Forward, Left, Right };
            public enum Angle { Forward, Up, Down };

            public Direction direction;
            public Angle angle;
        }

    }
}
