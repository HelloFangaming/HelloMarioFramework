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
    public class Galoomba : Enemy
    {

        //Game
        private bool flipped = false;

        //Animator hash values
        private static int flipHash = Animator.StringToHash("Flip");

        //What to do when stomped. Override this.
        protected override void WhenStomped()
        {
            if (flipped) base.WhenStomped();
            else
            {
                StartCoroutine(UnFlip());
                stomped = false;
                chase = false;
            }
        }

        //What to do when hurting player. Override this.
        protected override void WhenHurtPlayer(Player p)
        {
            if (!flipped) base.WhenHurtPlayer(p);
        }

        //Un flip self
        private IEnumerator UnFlip()
        {
            flipped = true;
            cooldown = true;
            animator.SetBool(flipHash, true);
            yield return new WaitForSeconds(2f);
            animator.SetBool(flipHash, false);
            yield return new WaitForSeconds(2f);
            flipped = false;
            yield return new WaitForSeconds(1f);
            cooldown = false;
            animator.SetBool(chaseHash, false);
        }

    }
}
