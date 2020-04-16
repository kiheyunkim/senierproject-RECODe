using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    private enum ButtonType { Start, Load, Option, Exit };

    private SettingManager settingMgr;

    private AudioSource BgmSound;
    private AudioSource startButtonSound;
    private AudioSource otherButtonSound;
    private Coroutine audioCoroutine;

    private UnityEngine.UI.Image blockImage;
    private UnityEngine.UI.Image logoImage;
    private List<GameObject> buttonlist = new List<GameObject>();

    void Awake()
    {
        PrepareInitialFiles();
        settingMgr = SettingManager.GetInstance;
        AudioSetter.SetBGMVolume(settingMgr.SettingTile);
        AudioSetter.SetEffectVolume(settingMgr.SettingTile);

        BgmSound = AudioSetter.SetBgm(gameObject, "Sound/Main/MainBG");
        startButtonSound = AudioSetter.SetEffect(gameObject, "Sound/Main/StartButton");
        otherButtonSound = AudioSetter.SetEffect(gameObject, "Sound/Main/MainButton");

        Texture2D cursorTexture = Resources.Load<Texture2D>("Texture/UI/cursor");
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        blockImage = transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        logoImage = transform.GetChild(1).GetComponent<UnityEngine.UI.Image>();
        foreach (Transform child in transform.GetChild(2))
            buttonlist.Add(child.gameObject);
    }

    protected IEnumerator ScreenBlockDisable()
    {
        float alpha = 1;
        blockImage.color = new Color(0, 0, 0, alpha);

        while (true)
        {
            alpha -= 0.01f;
            if (alpha < 0)
            {
                blockImage.color = new Color(0, 0, 0, 0);
                Destroy(blockImage.gameObject);
                yield break;
            }
            blockImage.color = new Color(0, 0, 0, alpha);

            yield return new WaitForSeconds(0.01f);  //2초 10단계 0.2초당 255를 10번
        }
    }

    protected IEnumerator LogoButtonAppear()
    {
        float alpha = 0;
        while (true)
        {
            logoImage.color = new Color(1, 1, 1, alpha);

            alpha += 0.1f;

            if (alpha > 1)
            {
                logoImage.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1);
                yield break;
            }

            yield return new WaitForSeconds(0.1f);  //1초 10단계 0.1초당 255를 10번
        }
    }

    protected IEnumerator ButtonAppear()
    {
        yield return StartCoroutine(ScreenBlockDisable());
        yield return StartCoroutine(LogoButtonAppear());

        float alpha = 0;
        while (true)
        {
            for (int i = 0; i < buttonlist.Count; i++)
                buttonlist[i].GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, alpha);

            alpha += 0.1f;

            if (alpha > 1)
            {
                for (int i = 0; i < buttonlist.Count; i++)
                {
                    buttonlist[i].GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1);
                    buttonlist[i].GetComponent<UnityEngine.UI.Button>().enabled = true;
                    Cursor.visible = true;
                }
                yield break;
            }

            yield return new WaitForSeconds(0.1f);  //1초 10단계 0.1초당 255를 10번
        }
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
                    case ButtonType.Start:
                        UnityEngine.SceneManagement.SceneManager.LoadScene("IntroMovie");
                        break;
                    case ButtonType.Load:
                        UnityEngine.SceneManagement.SceneManager.LoadScene("Load");
                        break;
                    case ButtonType.Option:
                        UnityEngine.SceneManagement.SceneManager.LoadScene("Option");
                        break;
                    case ButtonType.Exit:
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                         Application.Quit();
#endif
                        break;
                }

                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    void Start()
    {
        StartCoroutine(ButtonAppear());

        if (BgmSound != null) BgmSound.Play();
    }

    public void StartButton()
    {
        if (audioCoroutine != null) return;

        audioCoroutine = StartCoroutine(SoundAssistant(startButtonSound, ButtonType.Start));
    }

    public void LoadButton()
    {
        if (audioCoroutine != null) return;

        audioCoroutine = StartCoroutine(SoundAssistant(otherButtonSound, ButtonType.Load));
    }

    public void OptionButton()
    {
        if (audioCoroutine != null) return;

        audioCoroutine = StartCoroutine(SoundAssistant(otherButtonSound, ButtonType.Option));
    }

    public void ExitButton()
    {
        if (audioCoroutine != null) return;

        audioCoroutine = StartCoroutine(SoundAssistant(otherButtonSound, ButtonType.Exit));
    }

    private void PrepareInitialFiles()
    {
        ///Prepare for Save Files
        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Application.dataPath + "/Save");
        if (!di.Exists)  //Create Save files Directory
            di.Create();

        System.IO.FileInfo fi = new System.IO.FileInfo(Application.dataPath + "/Save/saveData.json");

        if (!fi.Exists)  //Create Save files 
        {
            List<SaveTile> saveTiles = new List<SaveTile>() { new SaveTile { }, new SaveTile { }, new SaveTile { } };
            string firstSaveStr = JsonWrapper.ToJson(saveTiles.ToArray());

            System.IO.File.WriteAllText(Application.dataPath + "/Save/saveData.Json", firstSaveStr);
        }
    }
}
