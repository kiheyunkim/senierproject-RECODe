using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public enum TutorialType { NONE, WASD, ESC, DRAG, EBttn, LMouse, RMouse };

    public List<Sprite> tutorialSprites = new List<Sprite>();
    private UnityEngine.UI.Image tutorialImage;
    private Coroutine showTutorialCoroutine = null;
    private Coroutine adjustTutorialCoroutine = null;

    public void AdjustTutorial(bool isUp)
    {
        if (adjustTutorialCoroutine != null)
            StopCoroutine(adjustTutorialCoroutine);
        adjustTutorialCoroutine = StartCoroutine(TutorialAdjust(isUp));
    }

    private void Awake()
    {
        tutorialSprites.AddRange(Resources.LoadAll<Sprite>("Texture/UI/button group2-2"));
        tutorialImage = GetComponent<UnityEngine.UI.Image>();
    }

    public void SetTutorial(TutorialType tutorialType, float second)
    {   
        if (showTutorialCoroutine != null)
            StopCoroutine(showTutorialCoroutine);

        switch (tutorialType)
        {
            case TutorialType.WASD:
                tutorialImage.sprite = tutorialSprites[0];
                break;
            case TutorialType.ESC:
                tutorialImage.sprite = tutorialSprites[1];
                break;
            case TutorialType.DRAG:
                tutorialImage.sprite = tutorialSprites[2];
                break;
            case TutorialType.EBttn:
                tutorialImage.sprite = tutorialSprites[3];
                break;
            case TutorialType.LMouse:
                tutorialImage.sprite = tutorialSprites[4];
                break;
            case TutorialType.RMouse:
                tutorialImage.sprite = tutorialSprites[5];
                break;
        }

        showTutorialCoroutine = StartCoroutine(ShowTutorial(second));
    }

    private IEnumerator TutorialAdjust(bool isUp)
    {//30 -> 130
        const float minPosY = 30.0f;
        const float maxPosY = 130.0f;

        float posY = isUp ? minPosY : maxPosY;

        while (true)
        {
            if (posY > maxPosY && isUp)
            {
                tutorialImage.transform.localPosition = new Vector3(0, maxPosY, 0);
                yield break;
            }

            if (posY < minPosY && !isUp)
            {
                tutorialImage.transform.localPosition = new Vector3(0, minPosY, 0);
                yield break;
            }

            tutorialImage.transform.localPosition = new Vector3(0, posY, 0);

            if (isUp)
                posY += 3.0f;
            else
                posY -= 3.0f;

            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator ShowTutorial(float timer)
    {
        float step = 0;
        float maxTimer = timer * 10;
        yield return StartCoroutine(TransparentControl(true));

        while (true)
        {
            if (step > maxTimer)
            {
                yield return StartCoroutine(TransparentControl(false));
                tutorialImage.sprite = null;
                yield break;
            }
            step += 1.0f;

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator TransparentControl(bool alphaUp)
    {
        float firstAlpha = alphaUp ? 0 : 1;

        while(true)
        {
            if(alphaUp)
                firstAlpha += 0.03f;
            else
                firstAlpha -= 0.03f;

            if (alphaUp && firstAlpha > 1)
            {
                tutorialImage.color = new Color(1, 1, 1, 1);
                yield break;
            }

            if (!alphaUp && firstAlpha < 0)
            {
                tutorialImage.color = new Color(1, 1, 1, 0);
                yield break;
            }

            tutorialImage.color = new Color(1, 1, 1, firstAlpha);

            yield return new WaitForSeconds(0.01f);
        }
    }
}
