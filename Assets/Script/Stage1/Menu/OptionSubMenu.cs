using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionSubMenu : MonoBehaviour
{
    private SettingManager settingManager;
    private List<UnityEngine.UI.Slider> sliderList;

    AudioSource optionBttn;

    private void Awake()
    {
        optionBttn = AudioSetter.SetEffect(gameObject, "Sound/Stage1/RightClick/Option/OptionButton");
        settingManager = SettingManager.GetInstance;
        sliderList = new List<UnityEngine.UI.Slider>(GetComponentsInChildren<UnityEngine.UI.Slider>());
    }

    private void Start()
    {
        //Initialize Settings
        sliderList[0].value = settingManager.SettingTile.Bgm;
        sliderList[1].value = settingManager.SettingTile.Effect;
        //Voice??
        sliderList[2].value = settingManager.SettingTile.MouseSensitive;
        sliderList[3].value = settingManager.SettingTile.WalkingSpeed;
    }

    public void VoiceOn()
    {
        optionBttn.Play();
        settingManager.SettingTile.Voice = true;
        AudioSetter.SetVoiceVolume(settingManager.SettingTile);
    }

    public void VoiceOff()
    {
        optionBttn.Play();
        settingManager.SettingTile.Voice = false;
        AudioSetter.SetVoiceVolume(settingManager.SettingTile);
    }

    public void BgmSlider()
    {
        settingManager.SettingTile.Bgm = sliderList[0].value;
        sliderList[0].value = Mathf.Round(sliderList[0].value);
        AudioSetter.SetBGMVolume(settingManager.SettingTile);
    }

    public void EffectSlider()
    {
        settingManager.SettingTile.Effect = sliderList[1].value;
        sliderList[1].value = Mathf.Round(sliderList[1].value);
        AudioSetter.SetEffectVolume(settingManager.SettingTile);
    }

    public void MouseSlider()
    {
        settingManager.SettingTile.MouseSensitive = sliderList[2].value;
        sliderList[2].value = Mathf.Round(sliderList[2].value);
    }

    public void WalkingSlider()
    {
        settingManager.SettingTile.WalkingSpeed = sliderList[3].value;
        sliderList[3].value = Mathf.Round(sliderList[3].value);
    }

    public void ResetSettings()
    {
        optionBttn.Play();
        SettingSaveTile resetData = new SettingSaveTile();
        settingManager.SettingTile = resetData;

        sliderList[0].value = settingManager.SettingTile.Bgm;
        sliderList[1].value = settingManager.SettingTile.Effect;
        sliderList[2].value = settingManager.SettingTile.MouseSensitive;
        sliderList[3].value = settingManager.SettingTile.WalkingSpeed;

        AudioSetter.SetBGMVolume(settingManager.SettingTile);
        AudioSetter.SetEffectVolume(settingManager.SettingTile);
        AudioSetter.SetVoiceVolume(settingManager.SettingTile);

        SaveSettings();
    }

    private void OnDestroy()
    {
        SaveSettings();
    }

    private void SaveSettings()
    {
        string saveString = JsonUtility.ToJson(settingManager.SettingTile);
        System.IO.File.WriteAllText(Application.dataPath + "/Setting/setting.json", saveString);
    }
}
