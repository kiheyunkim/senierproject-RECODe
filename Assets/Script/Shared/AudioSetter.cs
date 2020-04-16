using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * [0] Master
 * [1] Effect
 * [2] BGM
 */
public class AudioSetter : MonoBehaviour
{
    static public AudioSource SetEffect(GameObject where, string path)
    {
        UnityEngine.Audio.AudioMixer audioMixer = Resources.Load<UnityEngine.Audio.AudioMixer>("Audio/SoundMixer");
        if (audioMixer == null) return null;

        AudioClip audioClip = Resources.Load<AudioClip>(path);
        if (audioClip == null) return null;

        AudioSource audioSource = where.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[1];

        return audioSource;
    }

    static public AudioSource SetBgm(GameObject where, string path)
    {
        UnityEngine.Audio.AudioMixer audioMixer = Resources.Load<UnityEngine.Audio.AudioMixer>("Audio/SoundMixer");
        if (audioMixer == null) return null;

        AudioClip audioClip = Resources.Load<AudioClip>(path);
        if (audioClip == null) return null;

        AudioSource audioSource = where.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[2];
        audioSource.loop = true;

        return audioSource;
    }
    static public AudioSource SetVoice(GameObject where, string path)
    {
        UnityEngine.Audio.AudioMixer audioMixer = Resources.Load<UnityEngine.Audio.AudioMixer>("Audio/SoundMixer");
        if (audioMixer == null) return null;

        AudioClip audioClip = Resources.Load<AudioClip>(path);
        if (audioClip == null) return null;

        AudioSource audioSource = where.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[3];

        return audioSource;
    }

    static public void SetBGMVolume(SettingSaveTile settingTile)
    {//Value mapping 0~10 to -80 ~ 0 (mute)
        if (settingTile == null) return;

        UnityEngine.Audio.AudioMixer audioMixer = Resources.Load<UnityEngine.Audio.AudioMixer>("Audio/SoundMixer");
        audioMixer.SetFloat("BgmVolume", (10 - settingTile.Bgm) * -8.0f);
    }

    static public void SetEffectVolume(SettingSaveTile settingTile)
    {//Value mapping 0~10 to -80 ~ 0(mute)
        if (settingTile == null) return;

        UnityEngine.Audio.AudioMixer audioMixer = Resources.Load<UnityEngine.Audio.AudioMixer>("Audio/SoundMixer");
        audioMixer.SetFloat("EffectVolume", (10 - settingTile.Effect) * -8.0f);
    }

    static public void SetVoiceVolume(SettingSaveTile settingTile)
    {
        if (settingTile == null) return;

        UnityEngine.Audio.AudioMixer audioMixer = Resources.Load<UnityEngine.Audio.AudioMixer>("Audio/SoundMixer");
        audioMixer.SetFloat("VoiceVolume", settingTile.Voice ? 0.0f : -80.0f);
    }
}
