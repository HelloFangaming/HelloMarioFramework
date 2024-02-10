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
using Cinemachine;

namespace HelloMarioFramework
{
    public class Painting : MonoBehaviour
    {

        //Components
        private AudioSource audioPlayer;
        private CinemachineVirtualCamera camAtPainting;
        private Cloth cloth;

        //Audio clips
        [SerializeField]
        private AudioClip enterPaintingSFX;

        //Star select UI prefab
        [SerializeField]
        private GameObject starSelectPrefab;

        //Game
        [SerializeField]
        private string levelName;
        [SerializeField]
        private StarInfo[] levelStars;
        [Tooltip("Whether or not the star select menu should be shown")]
        [SerializeField]
        private bool showIntro = true;
        private bool active = false;

        // Start is called before the first frame update
        void Start()
        {
            audioPlayer = gameObject.AddComponent<AudioSource>();
            camAtPainting = GetComponentInChildren<CinemachineVirtualCamera>();
            cloth = GetComponentInChildren<Cloth>();
            SetClothCollider();
        }

        //Collision with player
        private void OnTriggerEnter(Collider collision)
        {
            if (!active)
            {
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {
                    active = true;
                    p.EnableControls(false);
                    p.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    if (camAtPainting != null) camAtPainting.m_Priority = 20;
                    PauseMenu.singleton.Disable();
                    audioPlayer.PlayOneShot(enterPaintingSFX);
                    if (!showIntro) audioPlayer.PlayOneShot(p.GetStartVoiceClip());
                    StartCoroutine(StartIntro(p.GetStartVoiceClip()));
                }
            }
        }

        //Set cloth collider
        public void SetClothCollider()
        {
            if (cloth != null) cloth.capsuleColliders = new CapsuleCollider[] { Player.singleton.GetComponent<CapsuleCollider>() };
        }

        //Intro ui
        private IEnumerator StartIntro(AudioClip voiceClip)
        {
            yield return new WaitForSeconds(1.5f);
            if (showIntro)
            {
                FadeControl.singleton.FadeWhite();
                yield return new WaitForSeconds(0.5f);
                MusicControl.singleton.StopMusic();
                PauseMenu.singleton.CreateUI(starSelectPrefab).GetComponent<IntroMenu>().DisplayIntro(levelName, levelStars, voiceClip);
            }
            else
            {
                LoadingScreen.scene = levelStars[0].scene.ScenePath;
                PauseMenu.levelSubText = "";
                MusicControl.singleton.Enter();
            }
        }

        [Serializable]
        public struct StarInfo
        {
            public string starName;
            public SceneReference scene;
        }

    }
}
