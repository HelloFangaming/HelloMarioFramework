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
    public class WarpBox : MonoBehaviour
    {

        //Components
        private Animator animator;
        private AudioSource audioPlayer;
        private Animator animatorDest;

        //Audio clips
        [SerializeField]
        private AudioClip warpInSFX;
        [SerializeField]
        private AudioClip warpOutSFX;
        [SerializeField]
        private AudioClip clickSFX;

        //Animator hash values
        private static int warpHash = Animator.StringToHash("Warp");
        
        [Tooltip("Destination")]
        [SerializeField]
        private GameObject dest;
        
        [Tooltip("Optional music to change to")]
        [SerializeField]
        private AudioClip newMusic = null;

        [Tooltip("Optional Cinemachine Virtual Camera to switch to")]
        [SerializeField]
        private CinemachineVirtualCamera camera = null;
        private static CinemachineVirtualCamera prevCamera = null;

        [Tooltip("Whether to lock the player's z axis after warping")]
        [SerializeField]
        private bool zAxisLock = false;

        //Game
        [SerializeField]
        private GameObject smoke;
        private bool ready = true;
        
        void Awake()
        {
            animator = GetComponent<Animator>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
#if UNITY_EDITOR
            if (dest == null)
            {
                Debug.LogWarning("Hello Mario Framework: Warp Box at " + transform.position + " is missing a destination!");
                if (UnityEditor.EditorUtility.DisplayDialog("Hello Mario Framework", "Warp Box at " + transform.position + " is missing a destination!", "Select GameObject", "Ignore"))
                {
                    UnityEditor.Selection.activeGameObject = gameObject;
                    UnityEditor.EditorGUIUtility.PingObject(gameObject.GetInstanceID());
                }
            }
#endif
            animatorDest = dest.GetComponent<Animator>();
        }

        //Warp Mario to destination
        private void OnTriggerEnter(Collider collision)
        {
            if (ready)
            {
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {
                    ready = false;
                    StartCoroutine(Warp(p));
                }
            }
        }

        //Warp
        private IEnumerator Warp(Player p)
        {
            animator.SetBool(warpHash, true);
            animatorDest.SetBool(warpHash, true);
            audioPlayer.PlayOneShot(warpInSFX);

            PlayerResizer resizer = p.gameObject.AddComponent<PlayerResizer>();

            //Disable player temporarily
            p.transform.position = transform.position + new Vector3(0f, 0.762f, 0f); //Center of Warp Box
            p.DisablePhysics();

            yield return new WaitForSeconds(0.1f);
            audioPlayer.PlayOneShot(clickSFX);

            //Smoke
            GameObject o = Instantiate(smoke);
            o.transform.position = transform.position + new Vector3(0f, 0.762f, 0f);
            o.transform.localScale = Vector3.one * 2f;

            yield return new WaitForSeconds(1f);

            //Fade and warp
            FadeControl.singleton.Fade();
            yield return new WaitForSeconds(0.5f);
            animatorDest.SetBool(warpHash, false);

            //Disable camera blending
            CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
            float oldBlend = brain.m_DefaultBlend.m_Time;
            brain.m_DefaultBlend.m_Time = 0f;

            //Warp the player
            p.transform.position = dest.transform.position + new Vector3(0f, 0.762f, 0f);
            p.transform.rotation = dest.transform.rotation;
            p.GetOffGround();

            //Remove hat
            p.RemoveHat();

            //Music change
            if (newMusic != null) MusicControl.singleton.ChangeMusic(newMusic);

            //Change cameras if set
            if (prevCamera != null) prevCamera.m_Priority = 0;
            if (camera != null)
            {
                camera.PreviousStateIsValid = false;
                camera.m_Priority = 12;
                prevCamera = camera;
            }
            else if (FreeLookHelper.singleton != null)
                FreeLookHelper.singleton.WarpCameraFix(dest.transform.position - transform.position);

            //Z axis lock
            p.LockZAxis(zAxisLock);

            yield return new WaitForSeconds(0.5f);
            audioPlayer.PlayOneShot(warpOutSFX);
            resizer.UndoShrink();
            yield return new WaitForSeconds(0.2f);

            //Enable player
            p.EnablePhysics(Vector3.up);

            //Reenable camera blending
            brain.m_DefaultBlend.m_Time = oldBlend;

            yield return new WaitForSeconds(0.1f);
            ready = true;
            animator.SetBool(warpHash, false);

            //Smoke
            o = Instantiate(smoke);
            o.transform.position = dest.transform.position + new Vector3(0f, 0.762f, 0f);
            o.transform.localScale = Vector3.one * 2f;

        }

    }
}
