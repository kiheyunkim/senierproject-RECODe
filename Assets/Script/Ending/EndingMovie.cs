using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingMovie : MonoBehaviour
{
    private UnityEngine.Video.VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        videoPlayer.loopPointReached += MovieEnd;
    }

    void Start ()
    {
        videoPlayer.Play();	
	}

    private void MovieEnd(UnityEngine.Video.VideoPlayer video)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
}
