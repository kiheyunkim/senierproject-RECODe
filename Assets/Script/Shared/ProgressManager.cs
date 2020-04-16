using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProgressManager : MonoBehaviour
{
    private static ProgressManager instance = null;
    public ProgressSaveTile Progress { get; set; }

    public static ProgressManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                GameObject instanceObj = new GameObject
                {
                    name = "Progress Manager"
                };
                instance = instanceObj.AddComponent<ProgressManager>();
                instance.Progress = new ProgressSaveTile();
            }
            return instance;
        }
    }

    public void AddItem(ItemSubMenu.ItemType itemType)
    {
        Progress.itemGets.Add(itemType);
    }

    public bool IsThereItem(ItemSubMenu.ItemType itemType)
    {
        return Progress.itemGets.Exists(x => x == itemType);
    }

    public void RemoveItem(ItemSubMenu.ItemType itemType)
    {
        Progress.itemGets.Remove(itemType); 
    }
}
