using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSubMenu : MonoBehaviour
{
    private AudioSource clickSound;
    private void Awake()
    {
        clickSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/RightClick/Main/MainButton");
    }

    public void PushYes()
    {
        clickSound.Play();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        Time.timeScale = 1.0f;
    }

    public void PushNo()
    {
        clickSound.Play();
    }
}
