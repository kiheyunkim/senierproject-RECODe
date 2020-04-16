using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadHelper : MonoBehaviour
{
    private static LoadHelper instance = null;

    public SaveTile SaveTile { get; set; }
    
    public static LoadHelper GetInstance
    {
        get
        {
            if (instance == null)
            {

                GameObject instanceObj = new GameObject
                {
                    name = "Load Helper"
                };
                instance = instanceObj.AddComponent<LoadHelper>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
