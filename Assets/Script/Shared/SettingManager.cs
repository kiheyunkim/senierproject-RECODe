using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    private static SettingManager instance = null;
    public SettingSaveTile SettingTile { get; set; }
    public bool NeedLoading { get; set; }

    public static SettingManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                GameObject instanceObj = new GameObject { name = "SettingManager Object" };
                instance = instanceObj.AddComponent<SettingManager>();

                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Application.dataPath + "/Setting");
                if (!di.Exists)
                    di.Create();
                
                System.IO.FileInfo fi = new System.IO.FileInfo(Application.dataPath + "/Setting/setting.json");
                if (!fi.Exists)
                {
                    fi.Create().Dispose();
                    instance.SettingTile = new SettingSaveTile();
                    string saveString = JsonUtility.ToJson(instance.SettingTile);
                    System.IO.File.WriteAllText(Application.dataPath + "/Setting/setting.json", saveString);
                }
                else
                {
                    string loadString = System.IO.File.ReadAllText(Application.dataPath + "/Setting/setting.json");
                    instance.SettingTile = JsonUtility.FromJson<SettingSaveTile>(loadString);
                }

                instance.NeedLoading = false;
            }

            return instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);    
    }

    private void OnApplicationQuit()
    {
        string saveString = JsonUtility.ToJson(SettingTile);
        System.IO.File.WriteAllText(Application.dataPath + "/Setting/setting.json", saveString);
    }
}

