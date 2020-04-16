using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaController : MonoBehaviour
{
    private GameObject doorObject;
    private UIController uIController;
    private CameraControl cameraControl;
    private ProgressManager progressManager;
    private GameObject pianoChair;

    private AudioSource doorOpen;
    private AudioSource doorClose;

    private bool isInBlocker = false;

    public enum FoorestArea { None, JumpArea, OnChair, FootRest2, FootRest3, FootRest4, FootRest5, FootRest6, stairchiffo3, stairchiffo, FinalFoorest }
    public FoorestArea currentArea;

    void Awake()
    {
        uIController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        doorObject = GameObject.FindGameObjectWithTag("House").transform.GetChild(2).gameObject;
        cameraControl = Camera.main.GetComponent<CameraControl>();
        progressManager = ProgressManager.GetInstance;

        pianoChair = Resources.Load<GameObject>("Models/Furnitures/Part1/pianochair");

        doorOpen = AudioSetter.SetEffect(doorObject, "Sound/Stage1/Part1/OpenDoor");
        doorClose = AudioSetter.SetEffect(doorObject, "Sound/Stage1/Part1/CloseDoor");
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Door Open")
        {
            doorOpen.Play();
            doorObject.GetComponent<Animator>().SetTrigger("Open");
            Destroy(other.gameObject);
        }

        if (other.name == "Blocker")
        {
            if (progressManager.Progress.TutorialPendant && progressManager.Progress.TutorialBag && progressManager.Progress.TutorialPiano)
            {
                Destroy(other.gameObject);
                return;
            }

            if (!isInBlocker)
            {
                uIController.SetSubTitle("주변을 조금만 더 살펴보고 가자");
                isInBlocker = true;
            }

        }

        if (other.name == "p2Entrance")
        {
            Destroy(other.gameObject);
            cameraControl.StartCameraWalking(1);
        }

        if (other.name == "ChairArea")
        {
            if (!progressManager.IsThereItem(ItemSubMenu.ItemType.PianoChair)) return;
            GameObject chair = Instantiate(pianoChair);
            chair.transform.position = other.transform.position;
            chair.AddComponent<BoxCollider>().isTrigger = true;
            chair.AddComponent<BoxCollider>();
            chair.tag = "Playing";
            progressManager.RemoveItem(ItemSubMenu.ItemType.PianoChair);
            Destroy(other);
        }

        if (other.name == "pianochair(Clone)")
        {
            currentArea = FoorestArea.OnChair;
        }

        if (other.name == "footrest2")
        {
            currentArea = FoorestArea.FootRest2;
        }

        if (other.name == "footrest3")
        {
            currentArea = FoorestArea.FootRest3;
        }
        
        if (other.name == "footrest4")
        {
            currentArea = FoorestArea.FootRest4;
        }

        if (other.name == "footrest5")
        {
            currentArea = FoorestArea.FootRest5;
        }

        if (other.name == "footrest6")
        {
            currentArea = FoorestArea.FootRest6;
        }

        if (other.name == "stairchiffo3")
        {
            currentArea = FoorestArea.stairchiffo3;
        }

        if (other.name == "stairchiffo")
        {
            currentArea = FoorestArea.stairchiffo;
        }

        if (other.name == "FinalFoorest")
        {
            currentArea = FoorestArea.FinalFoorest;
        }

    }

    public void OnTriggerExit(Collider other)
    {
        if (other.name == "Door Close")
        {
            doorObject.GetComponent<Animator>().SetTrigger("Close");
            progressManager.Progress.CheckerInTheHouse = true;
            uIController.SetSubTitle("이곳에서 내 기억을 찾아야 다시 태어날 수 있어, 우선 집안을 살펴보자");
            Destroy(other.gameObject);
            doorClose.Play();
        }

        if (other.name == "Blocker")
        {
            isInBlocker = false;
        }

        if (other.name == "JumpArea" || other.name == "footrest2" || other.name == "footrest3"
            || other.name == "footrest4" || other.name == "footrest5" || other.name == "footrest6"
            || other.name == "stairchiffo3" || other.name == "stairchiffo" || other.name == "FinalFoorest")
        {
            currentArea = FoorestArea.None;
        }
    }
}