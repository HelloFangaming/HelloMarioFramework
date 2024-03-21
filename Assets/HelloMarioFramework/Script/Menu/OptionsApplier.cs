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
using UnityEngine.Rendering.PostProcessing;

namespace HelloMarioFramework
{
    public class OptionsApplier : MonoBehaviour
    {

        //Put OptionsApplier on the post process volume
        private PostProcessVolume volume;

        private ColorGrading colorgrading = null;
        private Bloom bloom = null;
        private Vignette vignette = null;
        private AmbientOcclusion occlusion = null;
        private ScreenSpaceReflections reflections = null;
        private DepthOfField dof = null;

        //Handle other options too
        [SerializeField]
        public AudioSource musicPlayer;

        void Start()
        {

            //Get the post process components
            volume = GetComponent<PostProcessVolume>();
            volume.profile.TryGetSettings(out colorgrading);
            volume.profile.TryGetSettings(out bloom);
            volume.profile.TryGetSettings(out vignette);
            volume.profile.TryGetSettings(out occlusion);
            volume.profile.TryGetSettings(out reflections);
            volume.profile.TryGetSettings(out dof);

            //Make sure the settings exists
            if (OptionsSave.save == null) OptionsSave.Load();

            //Apply settings
            ChangeSettings();

        }

        public void ChangeSettings()
        {

            //Change music volume
            if (musicPlayer != null) musicPlayer.volume = ((float)OptionsSave.save.musicVolume) / 20;

            //Set the frame rate
            if (OptionsSave.save.frameRate == 0)
            {
                Application.targetFrameRate = -1;
                QualitySettings.vSyncCount = 1;
            }
            else
            {
                Application.targetFrameRate = OptionsSave.save.frameRate;
                QualitySettings.vSyncCount = 0;
            }

            //Toggle postprocess effects
            colorgrading.enabled.value = OptionsSave.save.colorGrading;
            bloom.enabled.value = OptionsSave.save.bloom;
            vignette.enabled.value = OptionsSave.save.vignette;
            occlusion.enabled.value = OptionsSave.save.ambientOcclusion;
            reflections.enabled.value = OptionsSave.save.screenSpaceReflections;
            dof.enabled.value = OptionsSave.save.depthOfField;

            //Disable depth of field if the camera is orthographic
            if (Camera.main.orthographic) dof.enabled.value = false;

            //Change camera settings
            if (FreeLookHelper.singleton != null)
                FreeLookHelper.singleton.LoadSettings();
        }

        public static void ChangeResolution(int resolution)
        {
            float ratio = (float)Display.main.systemWidth / (float)Display.main.systemHeight;
            if (resolution == -1) resolution = Display.main.systemHeight;
            Screen.SetResolution((int)(resolution * ratio), resolution, Screen.fullScreen);
        }

    }
}