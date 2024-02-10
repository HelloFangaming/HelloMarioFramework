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
    public class Star : MonoBehaviour
    {

        //Audio clips
        [SerializeField]
        private AudioClip collectSFX;

        //Transparent star material
        [SerializeField]
        private Material collectedStar;

        //Star that ends the level
        [Tooltip("Whether collecting this star ends the level")]
        [SerializeField]
        private bool levelEndStar = true;

        //Name to display after collecting star
        [Tooltip("All stars must have a unique name! This is how stars are tracked!")]
        [SerializeField]
        private string starName;

        private void Start()
        {
            if (IsCollected())
                transform.GetChild(0).GetChild(2).GetComponent<SkinnedMeshRenderer>().material = collectedStar;
#if UNITY_EDITOR
            if (starName == "")
                Debug.Log("Hello Mario Framework: Star at " + transform.position + " is missing a name! All stars must have a unique name! This is how stars are tracked!");
#endif
        }

        //Collision with player
        private void OnTriggerEnter(Collider collision)
        {
            Player p = collision.transform.GetComponent<Player>();
            if (p != null)
            {
                p.PlaySound(collectSFX);
                p.Victory(levelEndStar);

                if (levelEndStar)
                    MusicControl.singleton.Victory(starName);
                else
                    MusicControl.singleton.VictoryShort(starName);

                Destroy(gameObject);
            }
        }

        public bool IsCollected()
        {
            return SaveData.save.CheckCollection(starName);
        }

    }
}
