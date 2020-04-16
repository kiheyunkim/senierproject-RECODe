using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    private AsyncOperation async;
    
    IEnumerator Loading()
    {
        async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Stage1");

        while(!async.isDone)
        {
            Debug.Log(async.progress);
            yield return true;
        }
    }

	// Use this for initialization
	void Start ()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine("Loading");
	}
}
