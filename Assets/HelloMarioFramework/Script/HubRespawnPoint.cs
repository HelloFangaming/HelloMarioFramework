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
    public class HubRespawnPoint : MonoBehaviour
    {
        //Use this to respawn the player at a new point so that they don't have to start at the beginning when they return to the hub!

        //Collision with player
        private void OnTriggerEnter(Collider collision)
        {
            Player p = collision.transform.GetComponent<Player>();
            if (p != null)
            {
                SaveData.save.SetHubPosition(transform.position);
                SaveData.save.SetHubRotation(transform.rotation);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Exclamation.png", true);
        }

    }
}
