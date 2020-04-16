using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piano : GameCamera
{
    private Animator pianoAnimator;
    private GameObject keyCollider;
    private UIController uIController;
    private ProgressManager progressManager;
    private FlowerController flowerController;

    private AudioSource cSound;
    private AudioSource csSound;
    private AudioSource dSound;
    private AudioSource dsSound;
    private AudioSource eSound;
    private AudioSource fSound;
    private AudioSource fsSound;
    private AudioSource gSound;
    private AudioSource gsSound;
    private AudioSource aSound;
    private AudioSource asSound;
    private AudioSource bSound;

    private bool showTutorial;
    public List<string> answer = new List<string>();

    protected override void Awake()
    {
        base.Awake();
        InitializeParent(40.0f, transform.GetChild(9).gameObject, transform.GetChild(0).gameObject);
        pianoAnimator = GetComponent<Animator>();
        uIController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        progressManager = ProgressManager.GetInstance;
        flowerController = GameObject.FindGameObjectWithTag("Flower").GetComponent<FlowerController>();

        cSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/piano c");
        csSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/piano c#");
        dSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/piano d");
        dsSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/piano d#");
        eSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/piano e");
        fSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/piano f");
        fsSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/piano f#");
        gSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/piano g");
        gsSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/piano g#");
        aSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/piano a");
        asSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/piano a#");
        bSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/piano b");
    }

    private void Update()
    {
        if (gameState != GameState.Playing) return;

        if (!showTutorial)
        {
            uIController.SetTutorial(TutorialController.TutorialType.ESC, 3.0f);
            showTutorial = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10f))
            {
                Transform target = hit.transform;
                if (target == null) return;

                ProcessForKey(target);
            }
        }

        //Terminate Game
        if (Input.GetKeyUp(KeyCode.Escape) && gameState != GameState.End)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Destroy(keyCollider);
            keyCollider = null;
            answer.Clear();
            EndGame(GameState.NotPlaying);
        }
    }

    public void StartPiano()
    {
        if (gameState == GameState.Playing) return;
        if (keyCollider != null) return;
        if (gameState == GameState.End)
        {
            uIController.SetSubTitle("더 이상 볼 필요가 없다");
            return;
        }

        GameObject colliderPref = Resources.Load<GameObject>("Prefab/KeyCollider");
        keyCollider = Instantiate(colliderPref,transform);
        StartGame();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ProcessForKey(Transform target)
    {
        if (target.tag != "Research") return;
        pianoAnimator.SetTrigger(target.name);
        answer.Add(target.name);

        Debug.Log(target.name);

        switch (target.name)
        {
            case "C":
                cSound.Play();
                break;
            case "C#":
                csSound.Play();
                break;
            case "D":
                dSound.Play();
                break;
            case "D#":
                dsSound.Play();
                break;
            case "E":
                eSound.Play();
                break;
            case "F":
                fSound.Play();
                break;
            case "F#":
                fsSound.Play();
                break;
            case "G":
                gSound.Play();
                break;
            case "G#":
                gsSound.Play();
                break;
            case "A":
                aSound.Play();
                break;
            case "A#":
                asSound.Play();
                break;
            case "B":
                bSound.Play();
                break;
            default:
                break;

        }

        if (Checker(answer))
        {
            flowerController.SetBlossom();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Destroy(keyCollider);
            keyCollider = null;
            uIController.SetSubTitle("변화가 있나 확인해보자");
            EndGame(GameState.End);
        }
    }

    private bool Checker(List<string> checkers)
    {
        if (!progressManager.Progress.CheckerDecopiller) return false;
        if (checkers.Count != 6) return false;

        List<string> solution = new List<string>() { "G", "G", "E", "A", "G", "E" };

        for (int i = 0; i < solution.Count; i++)
        {
            if (solution[i] != answer[i])
                return false;
        }

        return true;
    }
}