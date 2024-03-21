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
    public class RotatorButton : MonoBehaviour
    {

        [SerializeField]
        private Vector3 rotationSpeed = Vector3.up;
        [SerializeField]
        private float angle = 180f;
        [SerializeField]
        private bool returnSlow;
        [SerializeField]
        private ButtonHandler button;

        private bool changeTo = true;
        private bool move = true;
        private bool wait = true;
        private Vector3 totalRotation = Vector3.zero;
        private Quaternion startRotation;

        void Start()
        {
            rotationSpeed *= 100; //Makes this feel more consistent with speeds used in Mover
            startRotation = transform.localRotation;
#if UNITY_EDITOR
            if (button == null)
            {
                Debug.LogWarning("Hello Mario Framework: RotatorButton at " + transform.position + " is missing a button!");
                if (UnityEditor.EditorUtility.DisplayDialog("Hello Mario Framework", "RotatorButton at " + transform.position + " is missing a button!", "Select GameObject", "Ignore"))
                {
                    UnityEditor.Selection.activeGameObject = gameObject;
                    UnityEditor.EditorGUIUtility.PingObject(gameObject.GetInstanceID());
                }
            }
#endif
        }

        void FixedUpdate()
        {
            if (!wait)
            {
                if (move)
                {
                    transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
                    totalRotation += rotationSpeed * Time.fixedDeltaTime;
                    if (totalRotation.magnitude >= angle)
                    {
                        changeTo = false;
                        move = false;
                        wait = true;
                        transform.Rotate(-rotationSpeed.normalized * (totalRotation.magnitude - angle));
                        totalRotation = Vector3.zero;
                    }
                }
                else
                {
                    if (returnSlow)
                    {
                        transform.Rotate(-rotationSpeed * Time.fixedDeltaTime * 0.1f);
                        totalRotation += rotationSpeed * Time.fixedDeltaTime * 0.1f;
                    }
                    else
                    {
                        transform.Rotate(-rotationSpeed * Time.fixedDeltaTime);
                        totalRotation += rotationSpeed * Time.fixedDeltaTime;
                    }
                    if (totalRotation.magnitude >= angle)
                    {
                        changeTo = true;
                        move = true;
                        wait = true;
                        transform.localRotation = startRotation;
                        totalRotation = Vector3.zero;
                    }
                }
            }
            else if (button.IsActive() == changeTo) wait = false;
        }

    }
}
