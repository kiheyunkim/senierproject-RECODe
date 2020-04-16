using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroMovie : MonoBehaviour
{
    private UnityEngine.Video.VideoPlayer videoPlayer;
    private UnityEngine.UI.Image pressText;
    private bool mouseEnable = false;

    private void Awake()
    {
        pressText = GameObject.FindGameObjectWithTag("UI").GetComponent<UnityEngine.UI.Image>();
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayer.started += MovieStart;
        videoPlayer.loopPointReached += MovieEnd;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private IEnumerator EnableMouseInput(int endTime)
    {   
        int counter = 0;
        while (true)
        {
            if (endTime - 1 < counter)
            {
                StartCoroutine(Appearing());
                mouseEnable = true;
                yield break;
            }

            counter++;
            yield return new WaitForSecondsRealtime(1.0f);
        }
    }

    private IEnumerator Appearing()
    {
        float alpha = 0;
        while (true)
        {
            alpha += 0.01f;

            if (alpha > 1)
            {
                pressText.color = new Color(1, 1, 1, 1);
                yield break;
            }

            pressText.color = new Color(1, 1, 1, alpha);
            yield return new WaitForSeconds(0.01f);
        }
    }

    void Start ()
    {
        videoPlayer.Play();
	}

    private void Update()
    {
        if (mouseEnable&&Input.GetMouseButtonUp(0))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
            mouseEnable = false;
        }
    }


    private void MovieStart(UnityEngine.Video.VideoPlayer video)
    {
        StartCoroutine(EnableMouseInput(10));
    }

    private void MovieEnd(UnityEngine.Video.VideoPlayer video)
    {
        mouseEnable = false;
        video.playbackSpeed = 0.0f;// vp.playbackSpeed / 10.0F;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
    }
}
