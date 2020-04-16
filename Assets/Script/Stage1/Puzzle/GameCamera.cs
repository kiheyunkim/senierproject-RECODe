using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public enum GameState { NotPlaying, Playing, End };
    protected GameState gameState = GameState.NotPlaying;

    private const float maxFov = 60.0f;
    private float minFov;

    private UIController uiController;
    private CameraControl cameraControl;

    private GameObject lookatTarget;
    private Vector3 gameCameraPos;

    private Vector3 initializePos;

    private Coroutine endCoroutine;

    protected virtual void Awake()
    {
        uiController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        cameraControl = Camera.main.GetComponent<CameraControl>();
    }

    protected IEnumerator CameraMoveIn()
    {
        float step = 0;
        Camera mainCamera = Camera.main;
        initializePos = mainCamera.transform.position;

        while (true)
        {
            if (step > 0.99f)
            {
                mainCamera.fieldOfView = minFov;
				gameState = GameState.Playing;
                yield break;
            }

            mainCamera.transform.position = Vector3.Slerp(initializePos, gameCameraPos, step);
            mainCamera.transform.LookAt(lookatTarget.transform.position);
            mainCamera.fieldOfView -= (maxFov - minFov) * 0.02f;
            step += 0.02f;

            yield return new WaitForSeconds(0.01f);
        }
    }

    protected IEnumerator CameraMoveOut(GameState endState)
    {
        float step = 0;
        Camera mainCamera = Camera.main;

        while (true)
        {
            if (step > 0.99f)
            {
                mainCamera.transform.LookAt(lookatTarget.transform);
                mainCamera.transform.localPosition = new Vector3(0, 0.5f, 0);
                gameState = endState;
                uiController.SetVisibleCursor(true);
                uiController.SetAnimation(UIController.AnimationType.Research);
                cameraControl.SetCameraStop(false);
                endCoroutine = null;
                yield break;
            }

            mainCamera.transform.position = Vector3.Slerp(gameCameraPos,initializePos, step);
            mainCamera.transform.LookAt(lookatTarget.transform.position);
            mainCamera.fieldOfView += (maxFov - minFov) * 0.02f;
            step += 0.02f;

            yield return new WaitForSeconds(0.01f);
        }

    }

    protected void StartGame()
    {   
        if (gameState == GameState.NotPlaying)
        {
            cameraControl.SetCameraStop(true);
            uiController.SetVisibleCursor(false);
            StartCoroutine(CameraMoveIn());
        }
    }

    protected void EndGame(GameState endState)
    {
        if (gameState == GameState.Playing || gameState == GameState.End)
        {
            if (endCoroutine != null) return;
            endCoroutine= StartCoroutine(CameraMoveOut(endState));
        }
    }

    protected void InitializeParent(float paramMinFov, GameObject viewTarget, GameObject cameraPos)
    {
        minFov = paramMinFov;
        lookatTarget = viewTarget;
        gameCameraPos = cameraPos.transform.position;
    }
}