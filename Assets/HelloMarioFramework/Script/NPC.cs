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
using Cinemachine;

namespace HelloMarioFramework
{
    public class NPC : MonoBehaviour
    {

        //Cursor above NPC's head (Prefab)
        [SerializeField]
        private GameObject cursor;

        //Audio clips
        [SerializeField]
        public AudioClip voiceSFX;

        //Height for the cursor
        [SerializeField]
        private float adjustCursorHeight = 0f;

        //NPC name
        [SerializeField]
        public string npcName;

        //Text strings
        [SerializeField]
        [TextArea(10, 15)]
        public string[] dialogue;

        //Game
        private bool active = true;
        private int cursorActive = 0;
        private GameObject cursorObj;
        private CinemachineVirtualCamera cam;
        protected Player player;

        //Button checker
        private bool jumpDown = false;
        private bool jumpPress = false;
        
        protected virtual void Start()
        {
            cursorObj = Instantiate(cursor);
            cursorObj.transform.SetParent(transform, false);
            cursorObj.SetActive(false);
            cam = GetComponentInChildren<CinemachineVirtualCamera>();
            if (adjustCursorHeight != 0f) cursorObj.transform.localPosition = Vector3.up * adjustCursorHeight;
        }

        protected virtual void FixedUpdate()
        {
            if (cursorActive > 0)
            {
                cursorActive--;
                if (cursorActive == 0)
                {
                    cursorObj.SetActive(false);
                }
            }
        }

        //Collision with player
        private void OnTriggerStay(Collider collision)
        {
            if (active)
            {
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {
                    //Handle input
                    if (p.jumpAction.action.IsPressed() && !jumpDown) jumpPress = true;
                    jumpDown = p.jumpAction.action.IsPressed();

                    if (cursorActive == 0)
                    {
                        cursorObj.SetActive(true);
                        jumpPress = false;
                    }
                    cursorActive = 4;

                    //Disable jump button and check if can talk
                    if (p.AltJumpButtonCheck() && jumpPress)
                    {
                        //Reset jump key presses
                        jumpDown = true;
                        jumpPress = false;

                        //Remember player object
                        player = p;

                        //Send info to dialog engine and start
                        DialogControl.singleton.DisplayDialog(this);

                        //Disable player
                        player.DisablePhysics();

                        //Delay until you can talk again
                        active = false;

                        //NPC cam
                        if (cam != null) cam.m_Priority = 20;
                    }

                    //Reset buttons
                    jumpPress = false;
                }
            }
        }

        //Called by Dialog Control when finished speaking
        public void FinishedSpeaking()
        {

            //Enable player
            player.EnablePhysics(Vector3.zero);

            //Add 1 extra second of delay
            StartCoroutine(Delay());

            //NPC cam
            if (cam != null) cam.m_Priority = 0;
        }

        private IEnumerator Delay()
        {
            yield return new WaitForSeconds(1f);
            active = true;
        }

    }
}
