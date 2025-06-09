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
using System.IO;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace HelloMarioFramework
{
    public static class HMFUtilities
    {
        [MenuItem("Hello Mario Framework/Getting Started", false, 0)]
        private static void GettingStarted()
        {
            UnityEditor.EditorUtility.DisplayDialog("Hello Mario Framework", "Ready to create your own 3D Mario game? Let's get started! First, navigate to the scenes folder (Assets/HelloMarioFramework/Scene), and open the example scenes included with the framework. These scenes serve as this framework's tutorial, and should answer any questions you may have. Ready to create your own level? A template scene is included for your convenience! Simply duplicate this scene to create your new level! You can duplicate this scene by selecting it and using the Ctrl+D keyboard shortcut!", "Ok");
        }

        [MenuItem("Hello Mario Framework/Open Save File Directory", false, 1)]
        private static void OpenSaveDirectory()
        {
            //Warn dev if the company name has not been changed
            if (Application.companyName == "DefaultCompany")
            {
                Debug.LogWarning("Hello Mario Framework: Save files are stored in a directory that uses the Company Name and Product Name fields in the player settings. Make sure to customize these to ensure that your save files get stored in a unique location! (File > Build Settings > Player Settings)");
                UnityEditor.EditorUtility.DisplayDialog("Hello Mario Framework", "Save files are stored in a directory that uses the Company Name and Product Name fields in the player settings. Make sure to customize these to ensure that your save files get stored in a unique location! (File > Build Settings > Player Settings)", "Ok");
            }
            Application.OpenURL(Application.persistentDataPath);
        }

        [MenuItem("Hello Mario Framework/Open Project Directory", false, 2)]
        private static void OpenProjectDirectory()
        {
            Application.OpenURL(Path.GetDirectoryName(Application.dataPath));
        }

        [MenuItem("Hello Mario Framework/Realtime-CSG Tutorial", false, 20)]
        private static void RealtimeCSGTutorial()
        {
            Application.OpenURL("https://logicalerror.github.io/realtime-csg/");
        }

        [MenuItem("Hello Mario Framework/Hello Mario Framework Website", false, 21)]
        private static void HMFWebsite()
        {
            Application.OpenURL("https://hellofangaming.github.io/HelloMarioFramework/");
        }

        [MenuItem("Hello Mario Framework/Hello Fangaming Website", false, 22)]
        private static void HFWebsite()
        {
            Application.OpenURL("https://hellofangaming.github.io/");
        }
    }
}
#endif
