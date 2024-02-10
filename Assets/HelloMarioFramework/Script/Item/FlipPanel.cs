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
    [RequireComponent(typeof(ButtonHandler))]
    public class FlipPanel : MonoBehaviour
    {

        //Components
        private AudioSource audioPlayer;
        private ButtonHandler myButton;

        //Audio clips
        [SerializeField]
        private AudioClip onSFX;
        [SerializeField]
        private AudioClip offSFX;
        [SerializeField]
        private AudioClip setSFX;

        [SerializeField]
        private bool isParent = false;
        [SerializeField]
        private FlipPanel[] children;

        private bool active = false;
        private bool set = false;
        private FlipPanel parent = null;
        private Transform[] panels;
        
        void Start()
        {
            audioPlayer = gameObject.AddComponent<AudioSource>();
            myButton = GetComponent<ButtonHandler>();
            if (isParent)
            {
                foreach (FlipPanel panel in children)
                    panel.parent = this;
            }
            panels = new Transform[] { transform.GetChild(0), transform.GetChild(1), transform.GetChild(2) };
        }

        void OnTriggerEnter(Collider collision)
        {
            if (!set && collision.attachedRigidbody != null && !collision.attachedRigidbody.isKinematic && !collision.isTrigger)
            {
                active = !active;
                panels[0].gameObject.SetActive(!active);
                panels[1].gameObject.SetActive(active);
                if (active)
                {
                    audioPlayer.PlayOneShot(onSFX);
                    PanelCheck();
                }
                else audioPlayer.PlayOneShot(offSFX);
            }
        }

        public bool IsActive()
        {
            return active;
        }

        public void PanelCheck()
        {
            if (isParent)
            {
                if (active)
                {
                    bool b = false;
                    for (int i = 0; i < children.Length; i++)
                    {
                        if (!children[i].IsActive()) b = true;
                    }
                    if (!b)
                    {
                        audioPlayer.PlayOneShot(setSFX);
                        myButton.SetActive(true);
                        PanelComplete();
                        foreach (FlipPanel panel in children)
                            panel.PanelComplete();
                    }
                }
            }
            else
                parent.PanelCheck();
        }

        public void PanelComplete()
        {
            set = true;
            panels[1].gameObject.SetActive(false);
            panels[2].gameObject.SetActive(true);
        }

        void OnDrawGizmos()
        {
            if (isParent) Gizmos.DrawIcon(transform.position, "Exclamation.png", true);
        }

    }
}
