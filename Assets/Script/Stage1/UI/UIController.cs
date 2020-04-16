using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UIController : MonoBehaviour
{
    public enum AnimationType { BookFind, DoorFind, BoxFind, TearingFind, SewingFind, SiccorFind, LPFind, Research, Return };

    private Animator cursorAnimator;
    private UnityEngine.UI.Image cursor;
    private SubTitleController subTitleController;

    private void Awake()
    {
        cursor = GetComponentInChildren<UnityEngine.UI.Image>();
        cursorAnimator = GetComponentInChildren<Animator>();
        subTitleController = GetComponentInChildren<SubTitleController>();
    }

    public void SetVisibleCursor(bool active)
    {
        cursor.enabled = active;
    }

    public void SetAnimation(AnimationType type)
    {
        if (enabled == false) return;
        cursorAnimator.ResetTrigger("Return");

        switch (type)
        {
            case AnimationType.BookFind:
                cursorAnimator.SetTrigger("BookFind");
                break;
            case AnimationType.BoxFind:
                cursorAnimator.SetTrigger("BoxFind");
                break;
            case AnimationType.DoorFind:
                cursorAnimator.SetTrigger("DoorFind");
                break;
            case AnimationType.LPFind:
                cursorAnimator.SetTrigger("LPFind");
                break;
            case AnimationType.Research:
                cursorAnimator.SetTrigger("Research");
                break;
            case AnimationType.SewingFind:
                cursorAnimator.SetTrigger("SewingFind");
                break;
            case AnimationType.SiccorFind:
                cursorAnimator.SetTrigger("SiccorFind");
                break;
            case AnimationType.TearingFind:
                cursorAnimator.SetTrigger("TearingFind");
                break;
            case AnimationType.Return:
                cursorAnimator.SetTrigger("Return");
                break;
        }
    }

    public void SetDoubleSubTitle(string audioSub, int audioIndex, string subTitle)
    {
        subTitleController.SetDoubleSubTitle(audioSub, audioIndex, subTitle);
    }

    public void SetSubTitle(string subtitle)
    {
        subTitleController.SetSubTitle(subtitle);
    }

    public void SetAudioSubTitle(string subtitle, int audioIndex)
    {
        subTitleController.SetAudioSubTitle(subtitle, audioIndex);
    }

    public void SetTutorial(TutorialController.TutorialType tutorialType, float second)
    {
        subTitleController.SetTutorial(tutorialType, second);
    }
}