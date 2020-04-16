using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadMenu : MonoBehaviour
{
    private enum ButtonType { Back, LoadYes };

    private List<SaveTile> saveTiles = new List<SaveTile>();
    private GameObject saveScreen;
    private GameObject loadBttn;

    private List<Sprite> saveScreenImages = new List<Sprite>();
    private SettingManager settingManager;
    private Sprite emptySaveImages;

    private List<GameObject> loadButtons = new List<GameObject>();
    private int selectedScene = 0;

    private GameObject loadCheckerPanel;

    private AudioSource bgmSound;
    private AudioSource buttonSound;
    private AudioSource loadButtonSound;
    private Coroutine soundAssistant;

    void Awake()
    {
        Sprite[] tempSprite = Resources.LoadAll<Sprite>("Texture/UI/button group2-1");

        settingManager = SettingManager.GetInstance;
        emptySaveImages = Resources.Load<Sprite>("Texture/UI/Load/BG(photo)");

        for (int i = 1; i <= 3; i++)
            saveScreenImages.Add(LoadSpriteFromFile(Application.dataPath + "/Save/SaveScreen" + i.ToString() + ".png") ?? emptySaveImages);

        string saveData = File.ReadAllText(Application.dataPath + "/Save/saveData.json");
        saveTiles.AddRange(JsonWrapper.FromJson<SaveTile>(saveData));

        foreach (Transform buttons in transform.GetChild(2))
            loadButtons.Add(buttons.gameObject);

        for (int i = 0; i < 3; i++)
        {
            if (saveTiles[i].IsSaved)
            {
                UnityEngine.UI.Text[] texts = loadButtons[i].GetComponentsInChildren<UnityEngine.UI.Text>();
                texts[0].text = "STAGE " + saveTiles[i].Stage.ToString();
                texts[1].text = saveTiles[i].progressTile.Percent.ToString("D3") + "%";
                loadButtons[i].GetComponent<UnityEngine.UI.Image>().sprite = tempSprite[9];
                loadButtons[i].GetComponent<UnityEngine.UI.Button>().spriteState = new UnityEngine.UI.SpriteState() { pressedSprite = tempSprite[8] };
            }
        }

        loadCheckerPanel = transform.GetChild(5).gameObject;
        saveScreen = transform.GetChild(1).gameObject;
        loadBttn = transform.GetChild(4).gameObject;
        loadBttn.SetActive(false);

        //SetSound
        bgmSound = AudioSetter.SetBgm(gameObject, "Sound/Load/LoadBG");
        buttonSound = AudioSetter.SetEffect(gameObject, "Sound/Load/LoadButton");
        loadButtonSound = AudioSetter.SetEffect(gameObject, "Sound/Load/LoadButton");

        //Cursor
        Texture2D cursorTexture = Resources.Load<Texture2D>("Texture/UI/cursor");
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
    }

    private void Start()
    {
        if (bgmSound != null)
            bgmSound.Play();
    }

    private IEnumerator SoundAssistant(AudioSource targetSound, ButtonType buttonType)
    {
        targetSound.Play();

        while (true)
        {
            if (!targetSound.isPlaying)
            {
                switch (buttonType)
                {
                    case ButtonType.Back:
                        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
                        break;
                    case ButtonType.LoadYes:
                        UnityEngine.SceneManagement.SceneManager.LoadScene("Stage1");
                        break;
                }

                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    

    public void PushLoadBttn(int loadNumber)
    {
        if (buttonSound != null)
            buttonSound.Play();

        SaveImageController saveImageController = saveScreen.GetComponent<SaveImageController>();
        if (saveImageController.Activated) return;

        if (saveTiles[loadNumber - 1].IsSaved)
        {
            selectedScene = loadNumber;
            saveImageController.SetTimer(saveTiles[loadNumber - 1].Date);
            saveImageController.SetSaveImage(saveScreenImages[loadNumber - 1].texture, true);
            loadBttn.SetActive(true);
        }
        else
        {
            saveImageController.SetSaveImage(emptySaveImages.texture, false);
            loadBttn.SetActive(false);
        }

    }

    public void PushLoad()
    {
        if (loadButtonSound != null)
            loadButtonSound.Play();

        loadCheckerPanel.SetActive(true);
    }

    public void CheckerYes()
    {
        //f (buttonSound != null)
        //   buttonSound.Play();

        if (soundAssistant != null) return;

        settingManager.NeedLoading = true;

        LoadHelper loadHelper = LoadHelper.GetInstance;
        loadHelper.SaveTile = saveTiles[selectedScene - 1];
        soundAssistant = StartCoroutine(SoundAssistant(buttonSound, ButtonType.LoadYes));
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Stage1");
    }

    public void CheckerNo()
    {
        if (buttonSound != null)
            buttonSound.Play();

        loadCheckerPanel.SetActive(false);
    }
    
    public void PushBack()
    {
        if (soundAssistant != null) return;

        soundAssistant = StartCoroutine(SoundAssistant(buttonSound, ButtonType.Back));
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
}