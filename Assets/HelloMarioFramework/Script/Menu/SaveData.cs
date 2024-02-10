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
    public class SaveData
    {

        //Current loaded save file
        public static SaveData save;
        private static string fileName;

        //Unsaved global variables
        public static bool checkpoint = false;
        public static Vector3 checkpointPos;
        public static Quaternion checkpointRot;
        public static bool hubPositionSet = false;

        //Saved variables
        public int coins = 0;
        public int starCount = 0;
        public List<string> collection = new List<string>();

        //First 3 are position, last is y euler angle
        public float[] hubPosition = new float[] { 0f, 0f, 0f, 0f };

        //Save
        public void Save()
        {
            System.IO.File.WriteAllText(fileName, JsonUtility.ToJson(this));
        }

        //Load (Make sure fileName is set, will be loaded if it exists)
        public static bool Load()
        {
            if (System.IO.File.Exists(fileName))
            {
                save = JsonUtility.FromJson<SaveData>(System.IO.File.ReadAllText(fileName));
                hubPositionSet = true;

                return true;
            }
            else
                return false;
        }

        //Create new game
        public static void NewGame()
        {
            save = new SaveData();
            hubPositionSet = false;
        }

        public static void SetFileName(string name)
        {
            fileName = Path.Combine(Application.persistentDataPath, name + ".json");
        }

        //Null check
        public static void NullCheck()
        {
            if (save == null)
            {
                SetFileName("TestSave");
                if (!Load()) NewGame();
                Debug.Log("Hello Mario Framework: Using test save file!");
                hubPositionSet = false;
            }
        }

        //Get coin count
        public int GetCoins()
        {
            return coins;
        }

        //Add or remove coins
        public void AddCoins(int i)
        {
            coins += i;
        }

        //Collect a single coin
        public void CollectCoin()
        {
            coins++;
        }

        //Get star count
        public int GetStarCount()
        {
            return starCount;
        }

        //Collect a star (Or ignore if already collected)
        public bool CollectStar(string name)
        {
            if (AddCollection(name))
            {
                starCount++;
                return true;
            }
            else return false;
        }

        //Add to collection
        public bool AddCollection(string id)
        {
            if (!collection.Contains(id))
            {
                collection.Add(id);
                return true;
            }
            else return false;
        }

        //Check if something is in the collection
        public bool CheckCollection(string id)
        {
            return collection.Contains(id);
        }

        //Because Unity can't serialize Vector3 and Quaternion
        public Vector3 GetHubPosition()
        {
            return new Vector3(hubPosition[0], hubPosition[1], hubPosition[2]);
        }
        public void SetHubPosition(Vector3 position)
        {
            hubPosition[0] = position.x;
            hubPosition[1] = position.y;
            hubPosition[2] = position.z;
        }
        public Quaternion GetHubRotation()
        {
            return Quaternion.Euler(0f, hubPosition[3], 0f);
        }
        public void SetHubRotation(Quaternion rotation)
        {
            hubPosition[3] = rotation.eulerAngles.y;
        }

    }
}
