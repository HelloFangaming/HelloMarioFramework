/*
 *  Copyright (c) 2024 Hello Fangaming
 *
 *  Use of this source code is governed by an MIT-style
 *  license that can be found in the LICENSE file or at
 *  https://opensource.org/licenses/MIT.
 *  
 * */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HelloMarioFramework
{
    [Serializable]
    public class OptionsSave
    {

        //Current loaded save file
        public static OptionsSave save;
        private static string fileName;

        //Saved variables
        public byte musicVolume = 8;
        public bool cameraRecentering = true;
        public bool colorGrading = true;
        public bool bloom = true;
        public bool vignette = true;
        public bool ambientOcclusion = true;
        public bool screenSpaceReflections = true;
        public bool depthOfField = true;
        public bool cameraXRecentering = true;
        public byte frameRate = 0; //0 = VSync, 30, 60
        public int resolution = 720;

        //Load
        public static void Load()
        {
            fileName = Path.Combine(Application.persistentDataPath, "Settings.json");

            if (System.IO.File.Exists(fileName))
            {
                save = JsonUtility.FromJson<OptionsSave>(System.IO.File.ReadAllText(fileName));
            }
            else
            {
                save = new OptionsSave();
#if (UNITY_ANDROID || UNITY_IOS)
                //Default to off on mobile
                save.ambientOcclusion = false;
                save.screenSpaceReflections = false;
#endif
            }
        }

        //Save
        public void Save()
        {
            System.IO.File.WriteAllText(fileName, JsonUtility.ToJson(this));
        }

    }
}
