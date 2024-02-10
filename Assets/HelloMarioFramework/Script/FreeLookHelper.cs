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
using UnityEngine.InputSystem;
using Cinemachine;

namespace HelloMarioFramework
{
    public class FreeLookHelper : MonoBehaviour
    {

        //Components
        public static FreeLookHelper singleton;
        private CinemachineFreeLook freeLook;
        private Transform cameraTransform;

        //Input
        [SerializeField]
        private InputActionReference zoomAction;
        [SerializeField]
        private InputActionReference cameraAction;
        [SerializeField]
        private InputActionReference centerAction;

        //Game
        private float camZoomLevel = 0f;
        private bool worldSpaceCam = false;
        private AxisState.Recentering defaultRecentering;

        void Awake()
        {
            singleton = this;
            freeLook = GetComponent<CinemachineFreeLook>();
            cameraTransform = Camera.main.transform;
            defaultRecentering = freeLook.m_YAxisRecentering;
            LoadSettings();
        }
        
        void Start()
        {
            zoomAction.action.Enable();
            cameraAction.action.Reset();
            centerAction.action.Enable();

            //Set camera follow
            freeLook.m_Follow = Player.singleton.transform;

            //Set camera lookat
            Transform cameraLookAt = new GameObject().transform;
            cameraLookAt.parent = Player.singleton.transform;
            cameraLookAt.localPosition = new Vector3(0f, Player.singleton.hatAttachTransform.position.y - Player.singleton.transform.position.y, 0f);
            freeLook.m_LookAt = cameraLookAt;
        }
        
        void FixedUpdate()
        {

            //Center camera button
            if (centerAction.action.IsPressed())
            {
                if (worldSpaceCam)
                    freeLook.m_XAxis.Value = Quaternion.RotateTowards(Quaternion.Euler(new Vector3(0f, cameraTransform.rotation.eulerAngles.y, 0f)), Player.singleton.transform.rotation, 400f * Time.fixedDeltaTime).eulerAngles.y;
                else
                    freeLook.m_XAxis.Value = Quaternion.RotateTowards(Quaternion.Euler(new Vector3(0f, cameraTransform.rotation.eulerAngles.y, 0f)), Player.singleton.transform.rotation, 400f * Time.fixedDeltaTime).eulerAngles.y - cameraTransform.rotation.eulerAngles.y;
            }

            //Zoom camera
            if (zoomAction.action.ReadValue<float>() != 0f)
            {
                camZoomLevel = Mathf.Clamp(camZoomLevel - zoomAction.action.ReadValue<float>() * 12f * Time.fixedDeltaTime, -10f, 10f);

                //Parabolas connecting 3 points
                freeLook.m_Orbits[0].m_Height = 0.015f * camZoomLevel * camZoomLevel + 0.85f * camZoomLevel + 10f; //3,10,20
                freeLook.m_Orbits[0].m_Radius = -0.005f * camZoomLevel * camZoomLevel + 0.15f * camZoomLevel + 3f; //1,3,4
                freeLook.m_Orbits[1].m_Height = -0.01f * camZoomLevel * camZoomLevel + 0.2f * camZoomLevel + 5f; //2,5,6
                freeLook.m_Orbits[1].m_Radius = 0.035f * camZoomLevel * camZoomLevel + 0.85f * camZoomLevel + 8f; //3,8,20
                freeLook.m_Orbits[2].m_Height = -0.0475f * camZoomLevel * camZoomLevel - 0.525f * camZoomLevel - 1f; //-0.5,-1,-11
                freeLook.m_Orbits[2].m_Radius = 0.1f * camZoomLevel + 2f; //1,2,3
            }

        }

        private void OnDestroy()
        {
            cameraAction.action.Reset();
        }

        //Load the settings set in the options menu
        public void LoadSettings()
        {
#if UNITY_EDITOR
            //Make sure the settings exists
            if (OptionsSave.save == null) OptionsSave.Load();
#endif
            //Camera X Recentering
            if (!OptionsSave.save.cameraXRecentering)
            {
                worldSpaceCam = true;
                freeLook.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
            }

            //Camera Y Recentering
            if (!OptionsSave.save.cameraRecentering)
                freeLook.m_YAxisRecentering = new AxisState.Recentering(false, 0f, 0f);
            else
                freeLook.m_YAxisRecentering = defaultRecentering;
        }

        //Zoom in the freelook for your victory pose
        public void VictoryZoom()
        {
            freeLook.m_Orbits[0].m_Height = 2f;
            freeLook.m_Orbits[0].m_Radius = 3f;
            freeLook.m_Orbits[1].m_Height = 2f;
            freeLook.m_Orbits[1].m_Radius = 3f;
            freeLook.m_Orbits[2].m_Height = 2f;
            freeLook.m_Orbits[2].m_Radius = 3f;
        }

        //Camera fix for warp boxes
        public void WarpCameraFix(Vector3 delta)
        {
            freeLook.OnTargetObjectWarped(Player.singleton.transform, delta);
            freeLook.PreviousStateIsValid = false;
            cameraAction.action.Reset();
            StartCoroutine(CamFix());
        }

        //Force the Free Look to face the correct direction
        private IEnumerator CamFix()
        {
            if (!worldSpaceCam)
                freeLook.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;

            float angle = Player.singleton.transform.rotation.eulerAngles.y;

            //Thanks Cinemachine!
            freeLook.m_XAxis.Value = angle;
            yield return new WaitForFixedUpdate();
            freeLook.m_XAxis.Value = angle;
            yield return new WaitForFixedUpdate();
            freeLook.m_XAxis.Value = angle;
            yield return new WaitForFixedUpdate();
            freeLook.m_XAxis.Value = angle;
            yield return new WaitForFixedUpdate();
            freeLook.m_XAxis.Value = angle;
            yield return new WaitForFixedUpdate();
            freeLook.m_XAxis.Value = angle;
            yield return new WaitForFixedUpdate();
            freeLook.m_XAxis.Value = angle;

            if (!worldSpaceCam)
            {
                yield return new WaitForFixedUpdate();
                freeLook.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
            }
        }

    }
}
