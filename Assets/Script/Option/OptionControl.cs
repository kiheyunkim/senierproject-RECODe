using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionControl : MonoBehaviour
{
    private List<UnityEngine.UI.Slider> sliders = new List<UnityEngine.UI.Slider>();
    private SettingManager settingMgr;
    public AudioSource bgmSound;
    public AudioSource buttonSound;

    private void Awake()
    {
        foreach (Transform slider in transform.GetChild(2))
            sliders.Add(slider.GetComponent<UnityEngine.UI.Slider>());

        settingMgr = SettingManager.GetInstance;
        bgmSound = AudioSetter.SetBgm(gameObject, "Sound/Option/OptionBG");
        buttonSound = AudioSetter.SetEffect(gameObject, "Sound/Option/OptionButton");

        Texture2D cursorTexture = Resources.Load<Texture2D>("Texture/UI/cursor");
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);

        sliders[0].value = settingMgr.SettingTile.Bgm;
        sliders[1].value = settingMgr.SettingTile.Effect;
        sliders[2].value = settingMgr.SettingTile.MouseSensitive;
        sliders[3].value = settingMgr.SettingTile.WalkingSpeed;
    }

    private void Start()
    {
        if (bgmSound != null)
            bgmSound.Play();
    }

    public void ResetSetting()
    {
        if (buttonSound != null)
            buttonSound.Play();

        SettingSaveTile resetData = new SettingSaveTile();
        settingMgr.SettingTile = resetData;

        sliders[0].value = settingMgr.SettingTile.Bgm;
        sliders[1].value = settingMgr.SettingTile.Effect;
        sliders[2].value = settingMgr.SettingTile.MouseSensitive;
        sliders[3].value = settingMgr.SettingTile.WalkingSpeed;

        AudioSetter.SetBGMVolume(settingMgr.SettingTile);
        AudioSetter.SetEffectVolume(settingMgr.SettingTile);
        AudioSetter.SetVoiceVolume(settingMgr.SettingTile);

        SaveSettings();
    }

    public void BackButton()
    {
        if (buttonSound != null)
            buttonSound.Play();

        SaveSettings();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    public void BgmSlider()
    {
        sliders[0].value = Mathf.Round(sliders[0].value);
        settingMgr.SettingTile.Bgm = sliders[0].value;

        AudioSetter.SetBGMVolume(settingMgr.SettingTile);
    }

    public void EffectSlider()
    {
        sliders[1].value = Mathf.Round(sliders[1].value);
        settingMgr.SettingTile.Effect = sliders[1].value;

        AudioSetter.SetEffectVolume(settingMgr.SettingTile);
    }

    public void MouseSlider()
    {
        sliders[2].value = Mathf.Round(sliders[2].value);
        settingMgr.SettingTile.MouseSensitive = sliders[2].value;
    }

    public void WalkingSlider()
    {
        sliders[3].value = Mathf.Round(sliders[3].value);
        settingMgr.SettingTile.WalkingSpeed = sliders[3].value;
    }

    public void VoiceOn()
    {
        buttonSound.Play();
        settingMgr.SettingTile.Voice = true;
        AudioSetter.SetVoiceVolume(settingMgr.SettingTile);
    }

    public void VoiceOff()
    {
        buttonSound.Play();
        settingMgr.SettingTile.Voice = false;
        AudioSetter.SetVoiceVolume(settingMgr.SettingTile);
    }

    private void SaveSettings()
    {
        string saveString = JsonUtility.ToJson(settingMgr.SettingTile);
        System.IO.File.WriteAllText(Application.dataPath + "/Setting/setting.json", saveString);
    }
}
