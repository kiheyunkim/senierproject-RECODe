using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : GameCamera
{
    private Animator animator;
    private GameObject siccor;
    private UIController uIController;
    private ProgressManager progressManager;
    private bool showTutorial;

    private AudioSource siccorGetSound;
    private AudioSource bagOpenSound;

    protected override void Awake()
    {
        base.Awake();

        InitializeParent(20.0f, transform.GetChild(1).gameObject, transform.GetChild(2).gameObject);
        animator = GetComponent<Animator>();
        siccor = transform.GetChild(3).gameObject;
        uIController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();

        siccorGetSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/GetObject1");
        bagOpenSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/OpenBag1");
    }

    private void Start()
    {
        progressManager = ProgressManager.GetInstance;
    }

    void Update()
    {
        if (gameState != GameState.Playing) return; 

        if(!showTutorial)
        {
            uIController.SetTutorial(TutorialController.TutorialType.ESC, 3.0f);
            showTutorial = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!progressManager.Progress.CheckerDoll2Touch) return;

            if (siccor != null)
            {   
                Destroy(siccor);
                siccorGetSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.Siccors);
                uIController.SetSubTitle("가위를 획득했다");
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
            EndGame(GameState.NotPlaying);
    }

    public void StartBag()
    {
        StartGame();
        animator.SetTrigger("Open");
        bagOpenSound.Play();
    }
}
