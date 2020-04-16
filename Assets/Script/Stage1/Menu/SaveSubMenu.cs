using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveSubMenu : MonoBehaviour
{
    private List<GameObject> buttonList = new List<GameObject>();
    private List<SaveTile> saveTiles = new List<SaveTile>();
    private Sprite dataExistSprite;
    private Sprite dataExistPressSprite;

    private AudioSource clickSound;

    private void Awake()
    {
        clickSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/RightClick/Save/SaveButton");

        foreach (Transform child in transform)
            buttonList.Add(child.gameObject);

        Sprite[] sprites = Resources.LoadAll<Sprite>("Texture/UI/button group1");
        dataExistSprite = sprites[23];
        dataExistPressSprite = sprites[25];

        ReLoadSaves();
    }

    private IEnumerator GetCapture(int saveNum)
    {
        Texture2D tempCapture = GetScreenShot(Camera.main.transform.GetChild(1).GetComponent<Camera>());
        buttonList[saveNum - 1].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(tempCapture, new Rect(0, 0, tempCapture.width, tempCapture.height), Vector2.zero);

        byte[] pngByte = tempCapture.EncodeToPNG();

        File.WriteAllBytes(Application.dataPath + "/Save/SaveScreen" + saveNum.ToString() + ".png", pngByte);

        yield break;
    }

    private IEnumerator SaveRoutine(int saveNum)
    {
        yield return StartCoroutine(GetCapture(saveNum));

        ProgressManager progressManager = ProgressManager.GetInstance;
        ProgressSaveTile progressTile = progressManager.Progress;

        GameObject player = Camera.main.gameObject;

        SaveTile tile = new SaveTile()
        {//Save Contents
            IsSaved = true,
            Date = System.DateTime.Now.ToString("MM-dd HH:mm"),
            Stage = 1, 
            CharacterPos = player.transform.parent.position,
            CharacterBodyQ = player.transform.parent.localEulerAngles,
            CameraQ = player.transform.localEulerAngles,
            progressTile = progressTile
        };

        saveTiles[saveNum - 1] = tile;

        string saveData = JsonWrapper.ToJson(saveTiles.ToArray());
        System.IO.File.WriteAllText(Application.dataPath + "/Save/saveData.Json", saveData);

        ReLoadSaves();

        yield break;
    }

    public void PushSaveBttn(int saveNum)
    {
        clickSound.Play();
        StartCoroutine(SaveRoutine(saveNum));
    }

    private void ReLoadSaves()
    {
        string saveData = File.ReadAllText(Application.dataPath + "/Save/saveData.json");
        List<Sprite> tempSaveImglist = new List<Sprite>();

        //ReLoad Json Dataes;
        saveTiles.Clear();
        saveTiles.AddRange(JsonWrapper.FromJson<SaveTile>(saveData));

        //Load Save Pic Dataes
        for (int i = 1; i <= 3; i++)
        {
            tempSaveImglist.Add(LoadSpriteFromFile(Application.dataPath + "/Save/SaveScreen" + i.ToString() + ".png") ?? null);
            if (saveTiles[i - 1].IsSaved)
            {
                buttonList[i - 1].GetComponent<UnityEngine.UI.Image>().sprite = dataExistSprite;
                buttonList[i - 1].GetComponent<UnityEngine.UI.Button>().spriteState = new UnityEngine.UI.SpriteState { pressedSprite = dataExistPressSprite };
            }
        }

        //Apply All
        for (int i = 0; i < 3; i++)
        {
            if (saveTiles[i].IsSaved)
            {
                UnityEngine.UI.Text[] texts = buttonList[i].GetComponentsInChildren<UnityEngine.UI.Text>();
                texts[0].text = saveTiles[i].Date;
                texts[1].text = "STAGE " + saveTiles[i].Stage.ToString();
                texts[2].text = saveTiles[i].progressTile.Percent.ToString("D3") + " %";
                buttonList[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = new Color(255, 255, 255, 255);
                buttonList[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = tempSaveImglist[i];
            }
        }
    }

    private Sprite LoadSpriteFromFile(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (System.IO.File.Exists(path))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1920, 1080);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }

        return null;
    }

    private Texture2D GetScreenShot(Camera cam)
    {
        Texture2D screen = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;
        screen.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screen.Apply();
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        return screen;
    }
}
