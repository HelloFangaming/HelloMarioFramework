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
    public class MoverButton : MonoBehaviour
    {

        [Tooltip("How far to move")]
        [SerializeField]
        private Vector3 offset;
        [Tooltip("Speed to move at")]
        [SerializeField]
        private float speed = 1f;
        [Tooltip("Whether the speed should be slower when returning to its original position")]
        [SerializeField]
        private bool returnSlow;
        [SerializeField]
        private ButtonHandler button;

        private Vector3 start;
        private Vector3 end;

        void Start()
        {
            start = transform.localPosition;
            end = start + offset;
#if UNITY_EDITOR
            if (button == null)
            {
                Debug.LogWarning("Hello Mario Framework: MoverButton at " + transform.position + " is missing a button!");
                if (UnityEditor.EditorUtility.DisplayDialog("Hello Mario Framework", "MoverButton at " + transform.position + " is missing a button!", "Select GameObject", "Ignore"))
                {
                    UnityEditor.Selection.activeGameObject = gameObject;
                    UnityEditor.EditorGUIUtility.PingObject(gameObject.GetInstanceID());
                }
            }
#endif
        }

        void FixedUpdate()
        {
            if (button.IsActive())
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, end, speed * Time.fixedDeltaTime);
            }
            else
            {
                if (returnSlow)
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, start, speed * Time.fixedDeltaTime * 0.1f);
                else
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, start, speed * Time.fixedDeltaTime);
            }
        }
    }
}
