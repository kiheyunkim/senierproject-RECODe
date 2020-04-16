using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemMagnify : MonoBehaviour
{
    public struct GameItem
    {
        public GameObject ItemObject;
        public bool CanAttempSave;
        public bool IsCanSave;
        public bool ExtraMagnify;
        public int GageValue;
        public System.Text.StringBuilder Number;
        public System.Text.StringBuilder Explaination;
    }

    private UnityEngine.UI.Text explaination;
    private UnityEngine.UI.Text itemNumber;

    private bool itemMagnifyStart = false;
    private bool itemClicked = false;
    private float scaleDefault;

    private CameraControl cameraControl;
    private Camera mainCamera;
    private Camera itemCamera;

    public GameObject pivot;
    public GameObject backwardEffectPivot;
    public GameObject forwardEffectPivot;
    public GameObject itemObject;
    public GameObject effectBackwardObject;
    public GameObject effectForwardObject;

    private GameObject basicEffect;
    private GameObject SaveEffect;
    private GameObject successEffect;
    private GameObject failedEffect;

    private ItemSubMenu.ItemType currentSelected = ItemSubMenu.ItemType.None;
    private Coroutine coroutine;
    private ProgressManager progressManager;
    private bool isExtraMagnify;

    private AudioSource saveButtonSound;
    private AudioSource cancelButtonSound;
    private AudioSource savesuccessSound;
    private AudioSource savefailSound;

    private List<GameItem> gameItems = new List<GameItem>();

    private void Awake()
    {
        saveButtonSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/RightClick/Item/ItemSaveButton");
        cancelButtonSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/RightClick/Item/ItemCancelButton");
        savesuccessSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/RightClick/Item/SaveSuccessButton");
        savefailSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/RightClick/Item/SaveFailButton");

        cameraControl = Camera.main.GetComponent<CameraControl>();
        mainCamera = Camera.main;
        itemCamera = GetComponent<Camera>();
        itemCamera.enabled = false;

        backwardEffectPivot = transform.GetChild(1).gameObject;
        pivot = transform.GetChild(2).gameObject;
        forwardEffectPivot = transform.GetChild(3).gameObject;

        UnityEngine.UI.Text[] texts = GetComponentsInChildren<UnityEngine.UI.Text>();
        explaination = texts[0];
        itemNumber = texts[1];

        progressManager = ProgressManager.GetInstance;

        AddResources();
        AddEffectResources();
    }

    // Update is called once per frame
    void Update()
    {
        if (!itemMagnifyStart) return;

        Ray ray = itemCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit))
        {
            if (raycastHit.transform == null) return;

            if (raycastHit.transform.tag == "Item")
                itemClicked = true;
        }
        if (itemClicked)
        {
            if (Input.GetMouseButton(0))
                itemObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0) * Time.smoothDeltaTime * 200.0f, Space.World);
            else
                itemClicked = false;


            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (isExtraMagnify)
                    scaleDefault = scaleDefault < 0.6f ? scaleDefault += 0.01f : scaleDefault;
                else
                    scaleDefault = scaleDefault < 0.3f ? scaleDefault += 0.01f : scaleDefault;
                itemObject.transform.localScale = new Vector3(scaleDefault, scaleDefault, scaleDefault);
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (isExtraMagnify)
                    scaleDefault = scaleDefault > 0.4f ? scaleDefault -= 0.01f : scaleDefault;
                else
                    scaleDefault = scaleDefault > 0.1f ? scaleDefault -= 0.01f : scaleDefault;
                itemObject.transform.localScale = new Vector3(scaleDefault, scaleDefault, scaleDefault);
            }
        }
    }

    public void StartMagnify(ItemSubMenu.ItemType itemType)
    {
        if (itemType == ItemSubMenu.ItemType.None) return;
        cameraControl.SetCameraStop(true);
        cameraControl.DestoryMenu();
        mainCamera.enabled = false;
        itemCamera.enabled = true;
        Time.timeScale = 1;

        itemMagnifyStart = true;
        SetItemModel(itemType);
    }

    public void SaveBttn()
    {
        if (currentSelected == ItemSubMenu.ItemType.None) return;       //이유는 모르겠는데 다른 카메라에서도 

        saveButtonSound.Play();

        Destroy(effectBackwardObject);

        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(SaveCoroutine());
    }

    private IEnumerator SaveCoroutine()
    {
        effectForwardObject = Instantiate(SaveEffect, forwardEffectPivot.transform.position, Quaternion.identity);
        
        while (effectForwardObject.GetComponent<ParticleSystem>().IsAlive())
            yield return new WaitForSeconds(0.1f);

        Destroy(effectForwardObject);

        if (gameItems[(int)currentSelected - 1].IsCanSave)
        {
            savesuccessSound.Play();
            effectForwardObject = Instantiate(successEffect, forwardEffectPivot.transform.position, Quaternion.identity);
            progressManager.Progress.Percent += gameItems[(int)currentSelected - 1].GageValue;
            progressManager.RemoveItem(currentSelected);
        }
        else
        {
            savefailSound.Play();
            effectForwardObject = Instantiate(failedEffect, forwardEffectPivot.transform.position, Quaternion.identity);
        }

        while (effectForwardObject.GetComponent<ParticleSystem>().IsAlive())
            yield return new WaitForSeconds(0.1f);

        yield return new WaitForSeconds(1.0f);

        cameraControl.SetCameraStop(false);
        cameraControl.ReCreateMenu();
        mainCamera.enabled = true;
        itemCamera.enabled = false;
        Time.timeScale = 0;

        itemMagnifyStart = false;
        currentSelected = ItemSubMenu.ItemType.None;
    }

    public void CancelBttn()
    {
        if (currentSelected == ItemSubMenu.ItemType.None) return;       //이유는 모르겠는데 다른 카메라에서도 

        cancelButtonSound.Play();

        cameraControl.SetCameraStop(false);
        cameraControl.ReCreateMenu();
        mainCamera.enabled = true;
        itemCamera.enabled = false;
        Time.timeScale = 0;

        itemMagnifyStart = false;
        currentSelected = ItemSubMenu.ItemType.None;
    }

    public void SetItemModel(ItemSubMenu.ItemType itemType)
    {
        currentSelected = itemType;

        int itemTypeInt = (int)itemType;
        if (itemObject != null) Destroy(itemObject);
        if (effectBackwardObject != null) Destroy(effectBackwardObject);
        if (effectForwardObject != null) Destroy(effectForwardObject);

        //Load and Set Info
        itemObject = Instantiate(gameItems[itemTypeInt - 1].ItemObject, transform, true);
        itemObject.transform.localPosition = pivot.transform.localPosition;
        itemObject.tag = "Item";
        itemObject.layer = 9;

        //Setting for model Child
        if (itemType == ItemSubMenu.ItemType.Album12 || itemType == ItemSubMenu.ItemType.Album22 || itemType == ItemSubMenu.ItemType.Album32)
        {
            itemObject.transform.GetChild(0).gameObject.layer = 9;
            itemObject.transform.GetChild(0).gameObject.tag = "Item";
            itemObject.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
        }
        itemObject.AddComponent<BoxCollider>();

        //Magnify
        isExtraMagnify = gameItems[itemTypeInt - 1].ExtraMagnify;
        if (isExtraMagnify)
            itemObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        else
            itemObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        scaleDefault = isExtraMagnify ? 0.4f : 0.1f;

        //Explain
        explaination.text = gameItems[itemTypeInt - 1].Explaination.ToString();
        itemNumber.text = gameItems[itemTypeInt - 1].Number.ToString();

        //EFfect
        if(gameItems[itemTypeInt - 1].CanAttempSave)
        {
            effectBackwardObject = Instantiate(basicEffect, backwardEffectPivot.transform.position, Quaternion.identity);
            effectBackwardObject.layer = 9;
        }

    }

    private void AddResources()
    {
        //1
        GameItem item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/Album1 Complete"),
            CanAttempSave = true,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. 09180723");
        item.Explaination.Append("사진이 채워진 앨범이다. 앨범에는 소녀와 소녀의 가족들의 사진이 있다. 내 사진인지는 정확히 기억나지 않는다.");
        gameItems.Add(item);

        //2
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/Album2 Complete"),
            CanAttempSave = true,
            IsCanSave = true,
            ExtraMagnify = false,
            GageValue = 33,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. 09780723");
        item.Explaination.Append("사진이 채워진 앨범이다. 앨범에는 소녀와 소녀의 가족들의 사진이 있다. 내 사진인지는 정확히 기억나지 않는다.");
        gameItems.Add(item);

        //3
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/Album3 Complete"),
            CanAttempSave = true,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. 09780753");
        item.Explaination.Append("사진이 채워진 앨범이다. 앨범에는 소녀와 소녀의 가족들의 사진이 있다. 내 사진인지는 정확히 기억나지 않는다.");
        gameItems.Add(item);

        //4
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/siccors"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. ????????");
        item.Explaination.Append("쇠로 된 가위이다. 무언가를 자를 수 있다.");
        gameItems.Add(item);

        //5
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/mirrorpiece"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = true,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. ????????");
        item.Explaination.Append("깨진 거울에서 나온 조각이다. 꽤 날카로워 간단한 것을 도려낼 수 있을 듯하다.");
        gameItems.Add(item);

        //6
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/pendant-no"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = true,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. q9hyh72e");
        item.Explaination.Append("사진을 넣을 수 있는 펜던트이다. 누군가의 사진을 넣으면…");
        gameItems.Add(item);

        //7
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/pendant1"),
            CanAttempSave = true,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. 08780723");
        item.Explaination.Append("Mary Bell의 사진을 넣은 펜던트이다. 이 사람이 나의 기억과 관련됐을까 ?");
        gameItems.Add(item);

        //8
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/pendant2"),
            CanAttempSave = true,
            IsCanSave = true,
            ExtraMagnify = false,
            GageValue = 33,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. 09780723");
        item.Explaination.Append("Branda Bell의 사진을 넣은 펜던트이다. 이 사람이 나의 기억과 관련됐을까 ?");
        gameItems.Add(item);

        //9
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/pendant3"),
            CanAttempSave = true,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. 09780123");
        item.Explaination.Append("Olivia Bell의 사진을 넣은 펜던트이다. 이 사람이 나의 기억과 관련됐을까 ?");
        gameItems.Add(item);

        //10
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/needle"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = true,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. ????????");
        item.Explaination.Append("끝이 날카로운 바늘이다. 실이 있다면 바느질을 할 수 있다.");
        gameItems.Add(item);

        //11
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/thread"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = true,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. ????????");
        item.Explaination.Append("가느다란 실이 감겨 있는 실뭉치이다. 바늘이 있다면 바느질을 할 수 있다.");
        gameItems.Add(item);

        //12
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/lp plate"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. ????????");
        item.Explaination.Append("축음기에 넣으면 재생할 수 있는 LP판이다. 어떤 음악이 있는지는 알 수 없다.");
        gameItems.Add(item);

        //13
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Furnitures/Part1/pianochair"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. ????????");
        item.Explaination.Append("피아노 앞에 있던 의자이다. 푹신푹신하다.");
        gameItems.Add(item);

        //14
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/doll1Complete"),
            CanAttempSave = true,
            IsCanSave = true,
            ExtraMagnify = false,
            GageValue = 34,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. 09780723");
        item.Explaination.Append("어릴 때 가지고 놀던 인형이다. 어머니께서 주신 기억이 어렴풋이 있다. 팔과 다리가 떨어져 있었으나 바느질로 이어줬다.");
        gameItems.Add(item);

        //15
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/doll1 leg"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = true,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. 09yn07ho");
        item.Explaination.Append("인형에게서 떨어진 다리이다. 바늘과 실이 있다면 몸통에 붙일 수 있다.");
        gameItems.Add(item);

        //16
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/doll1 arm"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = true,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. rd78hj23");
        item.Explaination.Append("인형에게서 떨어진 팔이다. 바늘과 실이 있다면 몸통에 붙일 수 있다.");
        gameItems.Add(item);

        //17
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/1picture"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = true,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. 0z780dt3");
        item.Explaination.Append("Mary Bell의 사진이다. 나와 성이 같다는 건...? ");
        gameItems.Add(item);

        //18
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/2picture"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = true,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. 0e780ty3");
        item.Explaination.Append("Branda Bell의 사진이다. 나와 성이 같다는 건...? ");
        gameItems.Add(item);

        //19
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/3picture"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = true,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. 0t780hu3");
        item.Explaination.Append("Olivia Bell의 사진이다. 나와 성이 같다는 건...? ");
        gameItems.Add(item);

        //20
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/Album1's item/picture1-1"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. 09tjeuwk");
        item.Explaination.Append("앨범에 들어가는 사진 조각 중 일부이다. 사진을 봤을 때 앨범1에 들어가는 듯하다.");
        gameItems.Add(item);

        //21
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/Album1's item/picture1-2"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. dl18wiqp");
        item.Explaination.Append("앨범에 들어가는 사진 조각 중 일부이다. 사진을 봤을 때 앨범1에 들어가는 듯하다.");
        gameItems.Add(item);

        //22
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/Album1's item/picture1-3"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. dlek07gh");
        item.Explaination.Append("앨범에 들어가는 사진 조각 중 일부이다. 사진을 봤을 때 앨범1에 들어가는 듯하다.");
        gameItems.Add(item);

        //23
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/Album1's item/picture1-4"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. qpeisl23");
        item.Explaination.Append("앨범에 들어가는 사진 조각 중 일부이다. 사진을 봤을 때 앨범1에 들어가는 듯하다.");
        gameItems.Add(item);

        //24
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/Album2's item/picture2-1"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. 09xjdids");
        item.Explaination.Append("앨범에 들어가는 사진 조각 중 일부이다. 사진을 봤을 때 앨범2에 들어가는 듯하다.");
        gameItems.Add(item);

        //25
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models / Items / Album2's item/picture2-2"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. ew78mdjf");
        item.Explaination.Append("앨범에 들어가는 사진 조각 중 일부이다. 사진을 봤을 때 앨범2에 들어가는 듯하다.");
        gameItems.Add(item);

        //26
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/Album2's item/picture2-3"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. tigy07al");
        item.Explaination.Append("앨범에 들어가는 사진 조각 중 일부이다. 사진을 봤을 때 앨범2에 들어가는 듯하다.");
        gameItems.Add(item);

        //27
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/Album2's item/picture2-4"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. kqpeis23");
        item.Explaination.Append("앨범에 들어가는 사진 조각 중 일부이다. 사진을 봤을 때 앨범2에 들어가는 듯하다.");
        gameItems.Add(item);

        //28
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/Album3's item/picture3-1"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. 09rislai");
        item.Explaination.Append("앨범에 들어가는 사진 조각 중 일부이다. 사진을 봤을 때 앨범3에 들어가는 듯하다.");
        gameItems.Add(item);

        //29
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/Album3's item/picture3-2"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. wr78paei");
        item.Explaination.Append("앨범에 들어가는 사진 조각 중 일부이다. 사진을 봤을 때 앨범3에 들어가는 듯하다.");
        gameItems.Add(item);

        //30
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/Album3's item/picture3-3"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. iqps07tz");
        item.Explaination.Append("앨범에 들어가는 사진 조각 중 일부이다. 사진을 봤을 때 앨범3에 들어가는 듯하다.");
        gameItems.Add(item);

        //31
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/Album3's item/picture3-4"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. bjgury53");
        item.Explaination.Append("앨범에 들어가는 사진 조각 중 일부이다. 사진을 봤을 때 앨범3에 들어가는 듯하다.");
        gameItems.Add(item);

        //32
        item = new GameItem
        {
            ItemObject = Resources.Load<GameObject>("Models/Items/thecross"),
            CanAttempSave = false,
            IsCanSave = false,
            ExtraMagnify = false,
            GageValue = 0,
            Number = new System.Text.StringBuilder(),
            Explaination = new System.Text.StringBuilder()
        };
        item.Number.Append("NO. ????????");
        item.Explaination.Append("묘비 모양으로 된 모형이다. R.I.P 이라는 글자가 써 있다.");
        gameItems.Add(item);
    }

    private void AddEffectResources()
    {
        basicEffect = Resources.Load<GameObject>("Effect/Item Magnify Effect/BasicEffect");
        SaveEffect = Resources.Load<GameObject>("Effect/Item Magnify Effect/Saveffect");
        successEffect = Resources.Load<GameObject>("Effect/Item Magnify Effect/Successeffect");
        failedEffect = Resources.Load<GameObject>("Effect/Item Magnify Effect/Faileffect");
    }
}
