using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPPlayer : MonoBehaviour
{
    private Animator lpAnimator;
    private AudioSource lpOnSound;
    private AudioSource playSound;
    private AudioSource endSound;

    private bool isFirst = false;

    public bool IsPlateOn { get; set; }
    public bool PlayState { get; private set; }

    private void Awake()
    {
        PlayState = false;
        lpAnimator = GetComponent<Animator>();
        lpOnSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/UseLP");
        playSound = AudioSetter.SetBgm(gameObject, "Sound/Stage1/Part1/Gramophone");
        endSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/GramophoneEnd");
    }
    
    public void SetPlay()
    {
        if(!isFirst)
        {
            isFirst = true;
            lpOnSound.Play();
        }
        playSound.Play();
        PlayState = true;
        lpAnimator.SetTrigger("Start");
    }

    public void SetStop()
    {
        playSound.Stop();
        PlayState = false;
        lpAnimator.SetTrigger("Stop");
        endSound.Play();
    }   
}
