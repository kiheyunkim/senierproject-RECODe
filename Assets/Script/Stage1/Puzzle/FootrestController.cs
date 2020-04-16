using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootrestController : MonoBehaviour
{
    private Vector3 originPos;
    private AudioSource footstepSound;

    protected IEnumerator FootRestDown()
    {
        float step = 0;
        Vector3 currentPos = transform.localPosition;

        while (true)
        {
            if (!footstepSound.isPlaying)
                footstepSound.Play();

            transform.localPosition = Vector3.Lerp(currentPos, originPos, step);
            step += 0.005f;

            if (step > 1)
            {
                transform.localPosition = originPos;
                footstepSound.Stop();
                yield break;
            }


            yield return new WaitForSeconds(0.0005f);
        }
    }

    private void Awake()
    {
        footstepSound = AudioSetter.SetBgm(gameObject, "Sound/Stage1/Part2/DownFootsteps");
        originPos = new Vector3(3.742742f, 8.418624f, 1.787394f);
    }

    public void SetFootrestPlayReady()
    {
        StartCoroutine(FootRestDown());
        Debug.Log("hi");
    }
}
