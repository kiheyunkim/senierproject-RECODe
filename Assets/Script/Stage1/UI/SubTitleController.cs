using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubTitleController : MonoBehaviour
{
    private List<AudioSource> audioList = new List<AudioSource>();

    private UnityEngine.UI.Text subtitleText;
    private Font audioFont;
    private Font normalFont;

    private TutorialController tutorialController;

    private void Awake()
    {
        audioFont = Resources.Load<Font>("Fonts/NanumGothicBold");
        normalFont = Resources.Load<Font>("Fonts/NanumMyeongjo");

        subtitleText = GetComponent<UnityEngine.UI.Text>();
        tutorialController = GetComponentInChildren<TutorialController>();

        audioList.Add(AudioSetter.SetVoice(gameObject, "Sound/SubTitle/1"));
        audioList.Add(AudioSetter.SetVoice(gameObject, "Sound/SubTitle/2"));
        audioList.Add(AudioSetter.SetVoice(gameObject, "Sound/SubTitle/3"));
        audioList.Add(AudioSetter.SetVoice(gameObject, "Sound/SubTitle/4"));
        audioList.Add(AudioSetter.SetVoice(gameObject, "Sound/SubTitle/5"));

        /*
         * 나와 같은 성을 가진 할머니다
         * 머리가 어지럽다
         * 우선 집안을 살펴보자
         * 이곳에서 내 기억을 찾아야 다시 태어날 수 있어
         * 저곳인가
        */
    }

    public void SetDoubleSubTitle(string audioSub, int audioIndex, string subTitle)
    {
        StopAllCoroutines();
        StartCoroutine(AudioAndSubtitle(audioSub, audioIndex, subTitle));
    }

    public void SetSubTitle(string subtitle)
    {
        StopAllCoroutines();
        StartCoroutine(ShowSubtitle(subtitle));
    }

    public void SetAudioSubTitle(string subtitle, int audioIndex)
    {
        StopAllCoroutines();
        StartCoroutine(ShowAudioSubtitle(subtitle, audioIndex));
    }

    public void SetTutorial(TutorialController.TutorialType tutorialType, float second)
    {
        tutorialController.SetTutorial(tutorialType, second);
    }

    private IEnumerator AudioAndSubtitle(string audioSub, int audioIndex, string subTitle)
    {
        yield return StartCoroutine(ShowAudioSubtitle(audioSub, audioIndex));

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(ShowSubtitle(subTitle));
    }

    private IEnumerator ShowSubtitle(string subtitle)
    {
        subtitleText.font = normalFont;

        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.Append(subtitle);

        if (stringBuilder.Length == 0)
            yield break;

        char[] delimiterChars = { ',' };
        List<string> splitSubtitles = new List<string>(stringBuilder.ToString().Split(delimiterChars));      //Split First List Element

        tutorialController.AdjustTutorial(true);

        while (true)
        {
            subtitleText.text = "";
            yield return new WaitForSeconds(0.3f);

            for (int i = 0; i < splitSubtitles[0].Length; i++)        //Show Split List
            {
                subtitleText.text += splitSubtitles[0][i];
                yield return new WaitForSeconds(0.1f);
            }

            splitSubtitles.RemoveAt(0);
            if (splitSubtitles.Count == 0)
            {
                yield return new WaitForSeconds(1.0f);
                subtitleText.text = "";
                tutorialController.AdjustTutorial(false);
                yield break;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator ShowAudioSubtitle(string subtitle, int audioIndex)
    {

        subtitleText.font = audioFont;

        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.Append(subtitle);

        if (stringBuilder.Length == 0)
            yield break;

        char[] delimiterChars = { ',' };
        List<string> splitSubtitles = new List<string>(stringBuilder.ToString().Split(delimiterChars));      //Split First List Element

        tutorialController.AdjustTutorial(true);

        audioList[audioIndex].Play();   //재생

        while (true)
        {
            subtitleText.text = "";

            subtitleText.text = splitSubtitles[0];
            yield return new WaitForSeconds(splitSubtitles[0].Length * 0.3f);

            splitSubtitles.RemoveAt(0);

            if (splitSubtitles.Count == 0)
            {
                yield return new WaitForSeconds(1.0f);
                subtitleText.text = "";
                tutorialController.AdjustTutorial(false);
                yield break;
            }
        }
    }
}