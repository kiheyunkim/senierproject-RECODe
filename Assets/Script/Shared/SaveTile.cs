    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveTile
{
    public bool IsSaved;
    public string Date;
    public int Stage;
    public Vector3 CharacterPos;
    public Vector3 CharacterBodyQ;
    public Vector3 CameraQ;
    public ProgressSaveTile progressTile;

    public SaveTile()
    {
        IsSaved = false;
        Date = "";
        Stage = 0;
        CharacterPos = Vector3.zero;
        CharacterBodyQ = Vector3.zero;
        CameraQ = Vector3.zero;
        progressTile = new ProgressSaveTile();
    }
}

[System.Serializable]
public class ProgressSaveTile
{
    public List<ItemSubMenu.ItemType> itemGets;

    //Bag
    public bool ItemBagReady;

    //restrict Tutorial     
    public bool TutorialPendant;
    public bool TutorialBag;
    public bool TutorialPiano;

    //Checker -> 
    public bool CheckerInTheHouse;          //문 클릭시 메세지 용
    public bool CheckerDoll2Touch;          //인형 2를 클릭해야 가위 바늘 싫을 얻을 수 있음
    public bool CheckerDollEnd;             //인형 완성 체커
    public bool CheckerLP;                  //LP 얻었는가? 얻었으면 true
    public bool CheckerGrave;               //십자가 획득 조건
    public bool CheckerFootRest;            //피아노 의자 획득 조건
    public bool CheckerDecopiller;          //피아노 따라 치기 전 조건
    public bool CheckerForAlbum;            //앨범 확인후 부터     

    public int Percent;
    public ProgressSaveTile()   
    {
        itemGets = new List<ItemSubMenu.ItemType>();
    }

}

[System.Serializable]
public class SettingSaveTile
{
    public float Bgm;               // 0~10 def = 5
    public float Effect;            // 0~10 def = 5
    public bool Voice;              // on
    public float MouseSensitive;    // 1 ~ 10 def 5
    public float WalkingSpeed;      // 4 ~ 10 def 7

    public SettingSaveTile()
    {
        Bgm = 10.0f; Effect = 10.0f; Voice = true; MouseSensitive = 5.0f; WalkingSpeed = 7.0f;
    }
}

public static class JsonWrapper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Tiles;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>
        {
            Tiles = array
        };
        return UnityEngine.JsonUtility.ToJson(wrapper);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Tiles;
    }
}