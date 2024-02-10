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
    public class SnakePanel : MonoBehaviour
    {

        //Components
        private Collider myCollider;

        //Faces
        [SerializeField]
        private GameObject faceDefault;
        [SerializeField]
        private GameObject faceSad;

        //Start Points
        [SerializeField]
        private Transform startForward;
        [SerializeField]
        private Transform startLeft;
        [SerializeField]
        private Transform startRight;

        private SnakeSwitch parentSwitch;
        private int index;
        private SnakeSwitch.SnakeBehavior[] snakePath;
        private bool ending;

        private void Start()
        {
            myCollider = GetComponent<Collider>();
            myCollider.enabled = false;
        }

        public void Flip(Transform t, SnakeSwitch s, int i, SnakeSwitch.SnakeBehavior[] p)
        {
            transform.position = t.position;
            transform.rotation = t.rotation;

            parentSwitch = s;
            index = i;
            snakePath = p;

            ending = (index == snakePath.Length - 1);
            StartCoroutine(FlipAnimate());
        }

        private IEnumerator FlipAnimate()
        {
            RotatorReverse r = gameObject.AddComponent<RotatorReverse>();
            if (snakePath[index].angle == SnakeSwitch.SnakeBehavior.Angle.Up) r.angle = 90f;
            else if (snakePath[index].angle == SnakeSwitch.SnakeBehavior.Angle.Down) r.angle = 270f;
            else r.angle = 180f;

            r.rotationSpeed = Vector3.right * 6f;
            r.pause = 100f;

            //Flip open
            yield return new WaitForSeconds(r.angle / 450f);
            Destroy(r);
            faceDefault.SetActive(true);
            myCollider.enabled = true;

            //Create next panel
            if (!ending)
            {
                Transform nextT;
                if (snakePath[index + 1].direction == SnakeSwitch.SnakeBehavior.Direction.Left) nextT = startLeft;
                else if (snakePath[index + 1].direction == SnakeSwitch.SnakeBehavior.Direction.Right) nextT = startRight;
                else nextT = startForward;
                Instantiate(parentSwitch.panelPrefab).GetComponent<SnakePanel>().Flip(nextT, parentSwitch, index + 1, snakePath);
            }

            //Warning
            yield return new WaitForSeconds(8f); //15 seconds before warning (Accurate, but too long!)
            faceDefault.SetActive(false);
            faceSad.SetActive(true);

            //Destroy
            yield return new WaitForSeconds(1f);
            if (ending) parentSwitch.EndSnakeSwitch();
            Destroy(gameObject);
        }

    }
}
