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
    public class Checkpoint : MonoBehaviour
    {

        //Components
        private Animator animator;
        private AudioSource audioPlayer;

        //Audio clips
        [SerializeField]
        private AudioClip checkpointSFX;

        //Materials
        [SerializeField]
        private Material flagSet;
        private Material flagDefault;
        private SkinnedMeshRenderer flagMaterial;

        //Game
        private bool flag = false;
        private bool delay = true;

        //Animator hash values
        private static int shakeHash = Animator.StringToHash("Shake");

        // Start is called before the first frame update
        void Start()
        {
            animator = transform.GetChild(0).GetComponent<Animator>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
            flagMaterial = transform.GetChild(0).GetChild(2).GetComponent<SkinnedMeshRenderer>();
            flagDefault = flagMaterial.material;
            StartCoroutine(Delay());
        }

        //Collision with player
        private void OnTriggerEnter(Collider collision)
        {
            if (!flag)
            {
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {

                    //Unset all other flags
                    foreach (Checkpoint other in FindObjectsByType<Checkpoint>(FindObjectsSortMode.None))
                    {
                        other.UnsetFlag();
                    }

                    p.Heal();
                    p.Heal();
                    SetFlag();
                    if (!delay)
                    {
                        audioPlayer.PlayOneShot(checkpointSFX);
                        p.CollectItemVoice();
                    }
                }
            }
        }

        //Set flag
        private void SetFlag()
        {
            flag = true;
            flagMaterial.material = flagSet;
            SaveData.checkpoint = true;
            SaveData.checkpointPos = transform.position;
            Vector3 rot = transform.rotation.eulerAngles;
            SaveData.checkpointRot = Quaternion.Euler(rot.x, rot.y + 180f, rot.z);
            StartCoroutine(Shake());
        }

        //Unset flag
        private void UnsetFlag()
        {
            if (flag)
            {
                flag = false;
                flagMaterial.material = flagDefault;
            }
        }

        //Shake
        private IEnumerator Shake()
        {
            animator.SetBool(shakeHash, true);
            yield return new WaitForSeconds(0.5f);
            animator.SetBool(shakeHash, false);
        }

        private IEnumerator Delay()
        {
            yield return new WaitForSeconds(0.1f);
            delay = false;
        }

    }
}
