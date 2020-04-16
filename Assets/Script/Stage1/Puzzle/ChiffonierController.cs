using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChiffonierController : MonoBehaviour
{
    public enum Type { Normal, Blocker };
    public Type type;

    private Animator animator;
    private GameObject itemProtector;
    private bool state = false;

    private int remains;

    private AudioSource openDrawSound;
    private AudioSource rattleDrawSound;
    private AudioSource closeDrawSound;

    private void Awake()
    {
        openDrawSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part2/OpenDrawer");
        rattleDrawSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part2/RattleDrawer");
        closeDrawSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part2/CloseDrawer");

        switch (type)
        {
            case Type.Normal:
                remains = 0;
                break;
            case Type.Blocker:
                itemProtector = transform.GetChild(1).gameObject;
                remains = 10;
                break;
            default:
                break;
        }
        animator = transform.parent.GetComponent<Animator>();
    }

    public void ChangeState()
    {
        if (remains > 0)
        {
            rattleDrawSound.Play();
            animator.SetTrigger("Rattle");
            remains -= 1;
            return;
        }

        if (itemProtector != null) Destroy(itemProtector);

        state = !state;

        if (state)
        {
            openDrawSound.Play();
            animator.SetTrigger("Open");
        }
        else
        {
            closeDrawSound.Play();
            animator.SetTrigger("Close");
        }
    }
}