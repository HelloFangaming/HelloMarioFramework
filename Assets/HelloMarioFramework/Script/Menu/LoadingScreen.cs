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
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace HelloMarioFramework
{
    public class LoadingScreen : MonoBehaviour
    {
        private static AsyncOperation asyncLoad;

        //Next scene to load
        public static string scene;

        //Remember hub scene information
        public static string titleScene;
        public static string hubScene;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(LoadAsyncScene());
        }

        private IEnumerator LoadAsyncScene()
        {
            //Load the scene from the variable
#if UNITY_EDITOR
            if (scene == null)
            {
                Debug.Log("Hello Mario Framework: Scene variable not set. Returning to title screen!");
                asyncLoad = SceneManager.LoadSceneAsync(0);
            }
            else
#endif
                asyncLoad = SceneManager.LoadSceneAsync(scene);

            //Fade Control (Wait for fadeout)
            asyncLoad.allowSceneActivation = false;
            yield return new WaitForSeconds(0.5f);
            asyncLoad.allowSceneActivation = true;
        }

        public static bool IsHubScene()
        {
            return (SceneManager.GetActiveScene().path == hubScene);
        }
    }
}
