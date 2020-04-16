using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AimControl : MonoBehaviour
{
    public GameObject AimedTarget { get; private set; }
    public string GetAimedName { private set { } get { return AimedTarget == null ? "none" : AimedTarget.name; } }

    private ProgressManager progressManager;
    private UIController uIController;
    private CameraControl cameraControl;
    private AreaController areaController;

    private GameObject letterPrefab;
    private GameObject crossEndedModel;

    private AudioSource cellarDoorSound;
    //빨간부분
    private AudioSource getObject1Sound;
    private AudioSource getDool1ArmSoun;
    private AudioSource getPianoChairSound;
    private AudioSource getPictureSound;
    private AudioSource openLetterSound;
    private AudioSource lookLetterSound;
    private AudioSource GetLpSound;
    private AudioSource dollLegGetSound;
    private AudioSource dollgetSound;
    private AudioSource siccorsUseSound;

    void Awake()
    {
        letterPrefab = Resources.Load<GameObject>("Prefab/Open Letter");
        crossEndedModel = Resources.Load<GameObject>("Models/Items/thecross(find)");
        uIController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        cameraControl = GetComponent<CameraControl>();
        areaController = transform.parent.GetComponent<AreaController>();
        progressManager = ProgressManager.GetInstance;

        //progressManager.AddItem(ItemSubMenu.ItemType.DollIntegrate);
        //progressManager.AddItem(ItemSubMenu.ItemType.Album22);
        //progressManager.AddItem(ItemSubMenu.ItemType.lpPlate);

        cellarDoorSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/OpenCellarDoor");
        getObject1Sound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/GetObject1");
        getDool1ArmSoun = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/GetDoll1Arm");
        getPianoChairSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/GetPianoChair");
        getPictureSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/GetPictures");
        openLetterSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/OpenLetter");
        lookLetterSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/LookLetter");
        GetLpSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part2/GetLP");
        dollLegGetSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part2/GetDoll1Leg");
        dollgetSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part2/GetDoll1");
        siccorsUseSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part2/Usesiccors");
    }

    void Start()
    {
        Texture2D cursorTexture = Resources.Load<Texture2D>("Texture/UI/cursor");
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //Debug.Log(AimedTarget);
        if (cameraControl.CameraState()) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2.5f))
        {
            Transform target = hit.transform;
            if (target == null) return;

            switch (target.tag)
            {
                case "Research":
                    AimForResearch(target);
                    break;

                case "Item":
                    AimForItem(target);
                    break;

                case "Playing":
                    AimForPlaying(target);
                    break;

                default:
                    uIController.SetAnimation(UIController.AnimationType.Return);
                    AimedTarget = null;
                    break;
            }
        }
        else
        {
            uIController.SetAnimation(UIController.AnimationType.Return);
            AimedTarget = null;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (AimedTarget == null) return;
            switch (AimedTarget.tag)
            {
                case "Research":
                    ProcessForResearch(AimedTarget);
                    break;

                case "Item":
                    ProcessForItem(AimedTarget);
                    break;

                case "Playing":
                    ProcessForPlaying(AimedTarget);
                    break;

                default:
                    break;
            }
        }

    }
        
    void AimForResearch(Transform target)
    {
        if (target.name == GetAimedName) return;

        switch (target.name)
        {
            case "door":
                if (!progressManager.Progress.CheckerInTheHouse) return;
                uIController.SetAnimation(UIController.AnimationType.DoorFind);
                AimedTarget = target.gameObject;
                break;

            case "cellardoor":
                uIController.SetAnimation(UIController.AnimationType.DoorFind);
                AimedTarget = target.gameObject;
                break;

            case "enclosed letter":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "Open Letter(Clone)":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "decopiller":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "grandfather clock":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            default:
                break;
        }


    }

    void AimForItem(Transform target)
    {
        if (target.name == GetAimedName) return;

        switch (target.name)
        {
            case "frame no1":
            case "frame no2":
            case "frame no3":
                if (progressManager.IsThereItem(ItemSubMenu.ItemType.MirrorPiece))
                    uIController.SetAnimation(UIController.AnimationType.TearingFind);
                else
                {
                    if (!progressManager.Progress.TutorialPendant)
                        uIController.SetTutorial(TutorialController.TutorialType.LMouse, 2.0f);
                    uIController.SetAnimation(UIController.AnimationType.Research);
                }
                AimedTarget = target.gameObject;
                break;

            case "doll1 arm":
            case "doll1 leg":
            case "mirrorpiece":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "Album1":
            case "Album2":
            case "Album3":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "picture1-1":
            case "picture1-2":
            case "picture1-3":
            case "picture1-4":
            case "picture2-1":
            case "picture2-2":
            case "picture2-3":
            case "picture2-4":
            case "picture3-1":
            case "picture3-2":
            case "picture3-3":
            case "picture3-4":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            default:
                break;
        }
    }

    void AimForPlaying(Transform target)
    {
        if (target.name == GetAimedName) return;

        switch (target.name)
        {
            case "bag":
                if (!progressManager.Progress.TutorialBag)
                    uIController.SetTutorial(TutorialController.TutorialType.LMouse, 2.0f);
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "piano":
                if (!progressManager.Progress.TutorialPiano)
                    uIController.SetTutorial(TutorialController.TutorialType.LMouse, 2.0f);
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "thecross":
                if (!progressManager.Progress.CheckerGrave) return;
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "grave":
                if (progressManager.Progress.CheckerFootRest) return;
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;
            case "bag2":
            case "chiffonier1-1":
            case "chiffonier1-2":
            case "chiffonier1-3":
            case "pendant-no":
            case "lp plate(find)":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "gramophone":
                if (progressManager.IsThereItem(ItemSubMenu.ItemType.lpPlate))
                    uIController.SetAnimation(UIController.AnimationType.LPFind);
                else
                    uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "cellardoor":
                uIController.SetAnimation(UIController.AnimationType.DoorFind);
                AimedTarget = target.gameObject;
                break;

            case "doll1":
                if (!progressManager.Progress.CheckerDoll2Touch)     //터치 전
                {
                    uIController.SetAnimation(UIController.AnimationType.Research);
                    AimedTarget = target.gameObject;
                    break;
                }

                if (progressManager.IsThereItem(ItemSubMenu.ItemType.Thread) && progressManager.IsThereItem(ItemSubMenu.ItemType.Needle)) //수리용품은 있음
                {
                    if (progressManager.Progress.CheckerDollEnd)        //인형 완성됨
                        uIController.SetAnimation(UIController.AnimationType.Research);
                    else                                                //인형 미완성
                        uIController.SetAnimation(UIController.AnimationType.SewingFind);
                    AimedTarget = target.gameObject;
                }
                break;

            case "pianochair":
                if (!progressManager.Progress.CheckerFootRest) return;
                getPianoChairSound.Play();
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "footrest2":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "footrest3":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "footrest4":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "footrest5":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "stairchiffo3":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "pianochair(Clone)":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "thread":
                if (!progressManager.Progress.CheckerDoll2Touch) return;
                if (!progressManager.IsThereItem(ItemSubMenu.ItemType.Siccors))
                {
                    uIController.SetAnimation(UIController.AnimationType.Research);
                    AimedTarget = target.gameObject;
                    return;
                }
                else
                {
                    uIController.SetAnimation(UIController.AnimationType.SiccorFind);
                    AimedTarget = target.gameObject;
                    return;
                }

            case "flower":
                if (!progressManager.Progress.CheckerDoll2Touch) return;
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "jewelry box":
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                break;

            case "door":
                if (progressManager.Progress.Percent != 100)
                    uIController.SetAnimation(UIController.AnimationType.Research);
                else
                    uIController.SetAnimation(UIController.AnimationType.DoorFind);

                AimedTarget = target.gameObject;
                break;
        }
    }

    void ProcessForItem(GameObject target)
    {
        switch (target.name)
        {
            case "frame no1":
                if (progressManager.IsThereItem(ItemSubMenu.ItemType.MirrorPiece))
                    progressManager.AddItem(ItemSubMenu.ItemType.pendentPic1);
                else
                {
                    uIController.SetDoubleSubTitle("나와 같은 성을 가진 할머니다", 1, "사진 모퉁이에는 매우 작은 사진이 있다");
                    progressManager.Progress.TutorialPendant = true;
                }
                break;
            case "frame no2":
                if (progressManager.IsThereItem(ItemSubMenu.ItemType.MirrorPiece))
                    progressManager.AddItem(ItemSubMenu.ItemType.pendentPic2);
                else
                {
                    uIController.SetDoubleSubTitle("나와 같은 성을 가진 할머니다", 1, "사진 모퉁이에는 매우 작은 사진이 있다");
                    progressManager.Progress.TutorialPendant = true;
                }
                break;
            case "frame no3":
                if (progressManager.IsThereItem(ItemSubMenu.ItemType.MirrorPiece))
                    progressManager.AddItem(ItemSubMenu.ItemType.pendentPic3);
                else
                {
                    uIController.SetDoubleSubTitle("나와 같은 성을 가진 할머니다", 1, "사진 모퉁이에는 매우 작은 사진이 있다");
                    progressManager.Progress.TutorialPendant = true;
                }
                break;

            case "doll1 arm":
                getDool1ArmSoun.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.dollArm);
                uIController.SetSubTitle("인형의 팔을 얻었다");
                Destroy(AimedTarget);
                break;

            case "doll1 leg":
                dollLegGetSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.dollLeg);
                uIController.SetSubTitle("인형의 다리를 얻었다");
                Destroy(AimedTarget);
                break;

            case "Album1":
                if (progressManager.IsThereItem(ItemSubMenu.ItemType.AlbumPic11) &&
                    progressManager.IsThereItem(ItemSubMenu.ItemType.AlbumPic12) &&
                    progressManager.IsThereItem(ItemSubMenu.ItemType.AlbumPic13) &&
                    progressManager.IsThereItem(ItemSubMenu.ItemType.AlbumPic14))
                {
                    progressManager.RemoveItem(ItemSubMenu.ItemType.AlbumPic11);
                    progressManager.RemoveItem(ItemSubMenu.ItemType.AlbumPic12);
                    progressManager.RemoveItem(ItemSubMenu.ItemType.AlbumPic13);
                    progressManager.RemoveItem(ItemSubMenu.ItemType.AlbumPic14);

                    AimedTarget.GetComponent<AlbumControl>().StartZoom(true);
                }
                else
                {
                    progressManager.Progress.CheckerForAlbum = true;
                    AimedTarget.GetComponent<AlbumControl>().StartZoom(false);
                }
                break;


            case "Album2":
                if (progressManager.IsThereItem(ItemSubMenu.ItemType.AlbumPic21) &&
                    progressManager.IsThereItem(ItemSubMenu.ItemType.AlbumPic22) &&
                    progressManager.IsThereItem(ItemSubMenu.ItemType.AlbumPic23) &&
                    progressManager.IsThereItem(ItemSubMenu.ItemType.AlbumPic24))
                {
                    progressManager.RemoveItem(ItemSubMenu.ItemType.AlbumPic21);
                    progressManager.RemoveItem(ItemSubMenu.ItemType.AlbumPic22);
                    progressManager.RemoveItem(ItemSubMenu.ItemType.AlbumPic23);
                    progressManager.RemoveItem(ItemSubMenu.ItemType.AlbumPic24);

                    AimedTarget.GetComponent<AlbumControl>().StartZoom(true);
                }
                else
                {
                    progressManager.Progress.CheckerForAlbum = true;
                    AimedTarget.GetComponent<AlbumControl>().StartZoom(false);
                }
                break;
            case "Album3":
                if (progressManager.IsThereItem(ItemSubMenu.ItemType.AlbumPic31) &&
                     progressManager.IsThereItem(ItemSubMenu.ItemType.AlbumPic32) &&
                     progressManager.IsThereItem(ItemSubMenu.ItemType.AlbumPic33) &&
                     progressManager.IsThereItem(ItemSubMenu.ItemType.AlbumPic34))
                {
                    progressManager.RemoveItem(ItemSubMenu.ItemType.AlbumPic31);
                    progressManager.RemoveItem(ItemSubMenu.ItemType.AlbumPic32);
                    progressManager.RemoveItem(ItemSubMenu.ItemType.AlbumPic33);
                    progressManager.RemoveItem(ItemSubMenu.ItemType.AlbumPic34);

                    AimedTarget.GetComponent<AlbumControl>().StartZoom(true);
                }
                else
                {
                    progressManager.Progress.CheckerForAlbum = true;
                    AimedTarget.GetComponent<AlbumControl>().StartZoom(false);
                }
                break;

            case "picture1-1":
                getPictureSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.AlbumPic11);
                uIController.SetSubTitle("사진조각을 얻었다");
                Destroy(AimedTarget);
                break;
            case "picture1-2":
                getPictureSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.AlbumPic12);
                uIController.SetSubTitle("사진조각을 얻었다");
                Destroy(AimedTarget);
                break;
            case "picture1-3":
                getPictureSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.AlbumPic13);
                uIController.SetSubTitle("사진조각을 얻었다");
                Destroy(AimedTarget);
                break;
            case "picture1-4":
                getPictureSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.AlbumPic14);
                uIController.SetSubTitle("사진조각을 얻었다");
                Destroy(AimedTarget);
                break;
            case "picture2-1":
                getPictureSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.AlbumPic21);
                uIController.SetSubTitle("사진조각을 얻었다");
                Destroy(AimedTarget);
                break;
            case "picture2-2":
                getPictureSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.AlbumPic22);
                uIController.SetSubTitle("사진조각을 얻었다");
                Destroy(AimedTarget);
                break;
            case "picture2-3":
                getPictureSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.AlbumPic23);
                uIController.SetSubTitle("사진조각을 얻었다");
                Destroy(AimedTarget);
                break;
            case "picture2-4":
                getPictureSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.AlbumPic24);
                uIController.SetSubTitle("사진조각을 얻었다");
                Destroy(AimedTarget);
                break;
            case "picture3-1":
                getPictureSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.AlbumPic31);
                uIController.SetSubTitle("사진조각을 얻었다");
                Destroy(AimedTarget);
                break;
            case "picture3-2":
                getPictureSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.AlbumPic32);
                uIController.SetSubTitle("사진조각을 얻었다");
                Destroy(AimedTarget);
                break;
            case "picture3-3":
                getPictureSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.AlbumPic33);
                uIController.SetSubTitle("사진조각을 얻었다");
                Destroy(AimedTarget);
                break;
            case "picture3-4":
                getPictureSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.AlbumPic34);
                uIController.SetSubTitle("사진조각을 얻었다");
                Destroy(AimedTarget);
                break;

            case "mirrorpiece":
                progressManager.AddItem(ItemSubMenu.ItemType.MirrorPiece);
                uIController.SetSubTitle("거울조각을 얻었다");
                Destroy(AimedTarget);
                break;

            default:
                break;
        }
    }

    void ProcessForPlaying(GameObject target)
    {
        switch (target.name)
        {
            case "bag":
                target.GetComponent<Bag>().StartBag();
                progressManager.Progress.TutorialBag = true;
                break;

            case "piano":
                target.GetComponent<Piano>().StartPiano();
                progressManager.Progress.TutorialPiano = true;
                break;

            case "gramophone":
                LPPlayer lPPlayer = target.GetComponent<LPPlayer>();
                if (progressManager.IsThereItem(ItemSubMenu.ItemType.lpPlate) || lPPlayer.IsPlateOn)  
                {
                    if (!progressManager.Progress.CheckerLP) progressManager.Progress.CheckerLP = true;
                    if (!lPPlayer.IsPlateOn)
                    {
                        progressManager.RemoveItem(ItemSubMenu.ItemType.lpPlate);
                        uIController.SetAnimation(UIController.AnimationType.Research);
                        lPPlayer.IsPlateOn = true;
                    }
                    if (lPPlayer.PlayState)
                        target.GetComponent<LPPlayer>().SetStop();
                    else
                        target.GetComponent<LPPlayer>().SetPlay();
                }
                break;

            case "bag2":
                if (progressManager.Progress.ItemBagReady) break;
                GetComponent<ItemZoom>().SetTarget(target);
                uIController.SetSubTitle("가방을 얻었다");
                progressManager.Progress.ItemBagReady = true;
                break;

            case "thecross":
                if (!progressManager.Progress.CheckerGrave) return;
                getObject1Sound.Play();
                uIController.SetSubTitle("묘비 모형을 얻었다");
                progressManager.AddItem(ItemSubMenu.ItemType.thecross);
                Destroy(AimedTarget);
                break;

            case "grave":
                if(!progressManager.IsThereItem(ItemSubMenu.ItemType.thecross))
                {
                    uIController.SetSubTitle("이건…묘지 모형인가?");
                    progressManager.Progress.CheckerGrave = true;
                }
                else
                {
                    getObject1Sound.Play();
                    uIController.SetSubTitle("묘비 모형을 꽂았다, 무언가 움직이기 시작한다…");
                    GameObject cross = Instantiate  (crossEndedModel, AimedTarget.transform.parent);
                    cross.transform.localPosition = AimedTarget.transform.localPosition + Vector3.up * 0.1f;
                    GameObject.FindGameObjectWithTag("FootRest").GetComponent<FootrestController>().SetFootrestPlayReady();
                    cameraControl.StartCameraWalking(2);
                    progressManager.Progress.CheckerFootRest = true;
                }
                break;  

            case "chiffonier1-1":
            case "chiffonier1-2":
            case "chiffonier1-3":
                AimedTarget.GetComponent<ChiffonierController>().ChangeState();
                break;

            case "lp plate(find)":
                GetLpSound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.lpPlate);
                uIController.SetSubTitle("LP판을 얻었다");
                Destroy(AimedTarget);
                break;

            case "pianochair":
                uIController.SetSubTitle("이거라면..?");
                progressManager.AddItem(ItemSubMenu.ItemType.PianoChair);
                Destroy(AimedTarget);
                break;

            case "footrest2":
                if (areaController.currentArea != AreaController.FoorestArea.OnChair)
                    uIController.SetSubTitle("너무 높아, 뭔가 밟고 올라갈 것이 필요할꺼 같아");
                else
                    cameraControl.SetJumpTo(transform.position, AimedTarget.transform.position);
                break;

            case "footrest3":
                if (areaController.currentArea != AreaController.FoorestArea.FootRest2)
                    uIController.SetSubTitle("바로는 못가겠는데?");
                else
                    cameraControl.SetJumpTo(transform.position, AimedTarget.transform.position);
                break;

            case "footrest4":
                if (areaController.currentArea != AreaController.FoorestArea.FootRest3)
                    uIController.SetSubTitle("바로는 못가겠는데?");
                else
                    cameraControl.SetJumpTo(transform.position, AimedTarget.transform.position);
                break;

            case "footrest5":
                if (areaController.currentArea != AreaController.FoorestArea.FootRest4)
                    uIController.SetSubTitle("바로는 못가겠는데?");
                else
                    cameraControl.SetJumpTo(transform.position, AimedTarget.transform.position);
                break;

            case "stairchiffo3":
                if (areaController.currentArea != AreaController.FoorestArea.FootRest6)
                    uIController.SetSubTitle("바로는 못가겠는데?");
                else
                    cameraControl.SetJumpTo(transform.position, AimedTarget.transform.position);
                break;

            case "pianochair(Clone)":
                cameraControl.SetJumpTo(transform.position, AimedTarget.transform.position);
                break;

            case "doll1":
                if (!progressManager.Progress.CheckerDoll2Touch)     //터치 전
                {
                    uIController.SetDoubleSubTitle("이건…, 내가 어릴 때 가지고 놀던 인형이랑 비슷하다, 하지만 팔과 다리가 없다", 1, "인형을 고쳐야겠다");
                    progressManager.Progress.CheckerDoll2Touch = true;
                    break;
                }

                if (progressManager.IsThereItem(ItemSubMenu.ItemType.Thread) && progressManager.IsThereItem(ItemSubMenu.ItemType.Needle)) //수리용품은 있음
                {
                    if (progressManager.Progress.CheckerDollEnd)        //인형 완성됨
                    {
                        dollgetSound.Play();
                        Destroy(AimedTarget);
                        uIController.SetSubTitle("인형을 얻었다");
                        progressManager.AddItem(ItemSubMenu.ItemType.DollIntegrate);
                        break;
                    }
                    else                                                //인형 미완성
                    {
                        Doll1Controller doll = AimedTarget.GetComponent<Doll1Controller>();

                        if(progressManager.IsThereItem(ItemSubMenu.ItemType.dollArm))       //팔만 있는 경우
                        {
                            getObject1Sound.Play();
                            progressManager.RemoveItem(ItemSubMenu.ItemType.dollArm);
                            doll.AddArm();
                            progressManager.Progress.CheckerDollEnd = doll.IsCompleted();
                            uIController.SetSubTitle("바느질로 인형의 팔을 붙여주었다");
                            break;
                        }

                        if(progressManager.IsThereItem(ItemSubMenu.ItemType.dollLeg))       //다리만 있는 경우
                        {
                            getObject1Sound.Play();
                            progressManager.RemoveItem(ItemSubMenu.ItemType.dollLeg);
                            doll.AddLeg();
                            progressManager.Progress.CheckerDollEnd = doll.IsCompleted();
                            uIController.SetSubTitle("바느질로 인형의 다리를 붙여주었다");
                            break;
                        }

                        //아무것도 없으면
                        uIController.SetSubTitle("고칠 재료가 필요해");
                    }
                }
                break;

            case "pendant-no":
                getObject1Sound.Play();
                uIController.SetSubTitle("사진이 없는 펜던트를 얻었다");
                progressManager.AddItem(ItemSubMenu.ItemType.PendantEmpty);
                Destroy(AimedTarget);
                break;

            case "thread":
                if(!progressManager.IsThereItem(ItemSubMenu.ItemType.Siccors))
                {
                    uIController.SetSubTitle("인형에 실뭉치가 달려있다, 단단히 박혀 있어 가져갈 수 없다, 자를 만한 무언가 필요하다");
                }
                else
                {
                    siccorsUseSound.Play();
                    uIController.SetSubTitle("실뭉치를 얻었다");
                    progressManager.AddItem(ItemSubMenu.ItemType.Thread);
                    Destroy(AimedTarget);
                }
                break;

            case "flower":
                uIController.SetSubTitle("바늘을 얻었다");
                getObject1Sound.Play();
                progressManager.AddItem(ItemSubMenu.ItemType.Needle);
                AimedTarget.tag = "Untagged";
                break;

            case "door":
                if (progressManager.Progress.Percent != 100)
                    uIController.SetSubTitle("문이 열리지 않는다, 내 기억과 관련된 물품을 더 찾아야 할 것 같아");
                else
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Ending");
                break;

            case "jewelry box":
                AimedTarget.GetComponent<JewelryBox>().StartJewelryBox();
                break;

            default:
                break;
        }
    }
    void ProcessForResearch(GameObject target)
    {
        switch (target.name)
        {
            case "door":
                uIController.SetSubTitle("지금 나갈 수는 없어");
                break;

            case "cellardoor":
                cellarDoorSound.Play();
                uIController.SetSubTitle("잠겨 있다");
                break;

            case "enclosed letter":
                openLetterSound.Play();
                uIController.SetSubTitle("봉투를 열어 편지를 꺼냈다.");
                GameObject letter = Instantiate(letterPrefab, target.transform.parent, true);
                letter.tag = "Research";
                letter.transform.position = target.transform.position;
                Destroy(target);
                break;

            case "Open Letter(Clone)":
                lookLetterSound.Play();
                uIController.SetAnimation(UIController.AnimationType.Research);
                AimedTarget = target.gameObject;
                uIController.SetSubTitle("사랑하는 나의 어머니께, 잘 지내고 계신가요?," +
                    " 저희 딸은 그곳에서 잘 지내고 있나요?, 자주 뵈러 가지 못해 죄송합니다," +
                    " 편지와 함께 상자를 보냈습니다, 상자는 그 곳에 있는 한 잠꾸러기가 열어줄 것입니다," +
                    " 딸아이가 크면 이 편지와 함께 전해주세요," +
                    " 감사합니다, 당신을 사랑하는 아들이");
                break;

            case "decopiller":
                uIController.SetSubTitle("'황제의 노래에 고개를 든다'라….");
                progressManager.Progress.CheckerDecopiller = true;
                break;

            case "grandfather clock":
                uIController.SetSubTitle("시간이 멈춰있다");
                break;
        }
    }
}