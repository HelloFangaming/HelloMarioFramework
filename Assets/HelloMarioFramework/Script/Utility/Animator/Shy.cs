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
    public class Shy : StateMachineBehaviour
    {

        private GameObject[] bodyParts;
        private bool set = false;

        private void SetUp(Transform t)
        {
            set = true;
            bodyParts = new GameObject[] { t.GetChild(1).gameObject, t.GetChild(3).gameObject, t.GetChild(2).gameObject, t.GetChild(4).gameObject };
        }

        //Whether Boo should hide or not
        private void Hide(bool b)
        {
            bodyParts[0].SetActive(!b);
            bodyParts[1].SetActive(!b);
            bodyParts[2].SetActive(b);
            bodyParts[3].SetActive(b);
        }
        
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!set) SetUp(animator.transform);
            Hide(true);
        }
        
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Hide(false);
        }

    }
}
