using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public struct WalkingPivot
    {
        public GameObject pivot;
        public GameObject nextPivot;
        public IEnumerator work;
        public System.Text.StringBuilder subTitle;
        public int audioNum;
        public float pivotTimer;
    }

    private readonly float minimumY = -89F;
    private readonly float maximumY = 89F;
    private float rotationY = 0F;

    private GameObject menuObject;
    private GameObject menuPrefab;

    private GameObject cameraWalkingPivot;
    public List<Vector3> rePositioningPos = new List<Vector3>();

    private bool cameraStop = false;
    private bool inputLock = false;

    private SettingManager settings;
    private UIController uiController;
    private ProgressManager progressManager;

    private AudioSource jumpSound;
    private AudioSource walkSound;

    void Awake()
    {
        menuPrefab = Resources.Load<GameObject>("Prefab/menu");
        settings = SettingManager.GetInstance;
        uiController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        cameraWalkingPivot = GameObject.FindGameObjectWithTag("CameraWalk");
        progressManager = ProgressManager.GetInstance;

        jumpSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part2/Jump");
        walkSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/Walk(Wood)");

        foreach (Transform rePos in cameraWalkingPivot.transform.GetChild(4))
            rePositioningPos.Add(rePos.position);

        if (settings.NeedLoading)
        {
            LoadHelper loadHelper = LoadHelper.GetInstance;
            SaveTile loadSettings = loadHelper.GetComponent<LoadHelper>().SaveTile;

            GameObject character = Camera.main.gameObject;
            rotationY = loadSettings.CameraQ.x > 60 ? 360 - loadSettings.CameraQ.x : -loadSettings.CameraQ.x;
            transform.localEulerAngles = new Vector3(loadSettings.CameraQ.x, 0, 0);
            transform.parent.eulerAngles = loadSettings.CharacterBodyQ;
            transform.parent.position = loadSettings.CharacterPos;
            settings.NeedLoading = false;

            Destroy(loadHelper.gameObject);
        }

    }

    private void Start()
    {
        uiController.SetAudioSubTitle("머리가 어지럽다, 저곳인가?", 1);
        uiController.SetTutorial(TutorialController.TutorialType.WASD, 3.0f);
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Alpha1)&& progressManager.IsThereItem(ItemSubMenu.ItemType.PendantEmpty))
        {
            progressManager.RemoveItem(ItemSubMenu.ItemType.PendantEmpty);
            progressManager.AddItem(ItemSubMenu.ItemType.Pendant2);
        }

        if (cameraStop) return;

        if (Input.GetMouseButtonUp(1))
        {
            if (menuObject == null)
            {
                uiController.SetVisibleCursor(false);
                menuObject = Instantiate(menuPrefab);
                inputLock = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                uiController.SetVisibleCursor(true);
                Destroy(menuObject);
                inputLock = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (inputLock) return;

        float rotationX = transform.eulerAngles.y + Input.GetAxisRaw("Mouse X") * settings.SettingTile.MouseSensitive;

        rotationY += Input.GetAxisRaw("Mouse Y") * settings.SettingTile.MouseSensitive;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
        transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
        transform.parent.eulerAngles = new Vector3(0, rotationX, 0);

        //keyboard Moving
        Vector3 moveVector = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            moveVector += transform.parent.transform.forward;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            moveVector -= transform.parent.transform.forward;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            moveVector -= transform.parent.transform.right;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            moveVector += transform.parent.transform.right;

        if (moveVector != Vector3.zero)
        {
            if (!walkSound.isPlaying)
                walkSound.Play();
        }
        else
            walkSound.Stop();

        transform.parent.Translate(moveVector * Time.smoothDeltaTime * settings.SettingTile.WalkingSpeed * 0.1f, Space.World);
    }

    public void DestoryMenu()
    {
        Destroy(menuObject);
    }

    public void ReCreateMenu()
    {
        menuObject = Instantiate(menuPrefab);
    }

    public void SetCameraStop(bool state)
    {
        cameraStop = state;
    }

    public bool CameraState()
    {
        return cameraStop || inputLock;
    }

    public void SetInputLock(bool state)
    {
        inputLock = state;
    }

    private IEnumerator CameraWalking(int type)
    {
        List<WalkingPivot> pivots = new List<WalkingPivot>();

        ///Setting Camera Pivots
        switch (type)
        {
            case 1:
                SetWalking1Pivot(pivots);
                break;

            case 2:
                SetWalking2Pivot(pivots);
                break;
            default:
                yield break;
        }

        int count = pivots.Count;
        int step = 0;
        SetCameraStop(true);
        
        while (true)
        {
            if (pivots.Count == 0)
            {
                SetCameraStop(false);
                yield break;
            }
            
            if (pivots[step].subTitle != null)
                uiController.SetAudioSubTitle(pivots[step].subTitle.ToString(), pivots[step].audioNum);

            for (float i = 0; i <= 1; i += 0.02f)
            {
                Vector3 lookPos = Vector3.Lerp(pivots[step].pivot.transform.position, pivots[step].nextPivot.transform.position, i);
                Camera.main.transform.LookAt(lookPos);

                yield return new WaitForSeconds(0.01f);
            }
            
            if (pivots[step].work != null)
                yield return StartCoroutine(pivots[step].work);
            step++;

            if (step  > count - 1)
            {
                Camera.main.transform.LookAt(pivots[count - 1].nextPivot.transform);
                SetCameraStop(false);
                yield break;
            }

            yield return new WaitForSeconds(pivots[step].pivotTimer);
        }
    }

    ///type
    ///1: First Walking
    public void StartCameraWalking  (int type)
    {
        StartCoroutine(CameraWalkingFromTo(transform.parent.position, rePositioningPos[0]));
        StartCoroutine(CameraWalking(type));
    }

    private void SetWalking1Pivot(List<WalkingPivot> walkingPivots)
    {
        List<GameObject> tempPivots = new List<GameObject>();
        foreach (Transform child in cameraWalkingPivot.transform.GetChild(0))
            tempPivots.Add(child.gameObject);

        WalkingPivot walkingPivot = new WalkingPivot()
        {
            pivot = tempPivots[0],
            nextPivot = tempPivots[0],
            subTitle = new System.Text.StringBuilder("나는 어릴 적 나무 위에서 떨어지는 사고를 겪었다고 한다, 물론 그 당시를 기억하지 못한다"),
            audioNum = 1,
            work = null,
            pivotTimer = 3.0f
        };
        walkingPivots.Add(walkingPivot);


        walkingPivot = new WalkingPivot()
        {
            pivot = tempPivots[0],
            nextPivot = tempPivots[1],
            subTitle = null,
            audioNum = 0,
            work = null,
            pivotTimer = 0.5f
        };
        walkingPivots.Add(walkingPivot);

        walkingPivot = new WalkingPivot()
        {
            pivot = tempPivots[1],
            nextPivot = tempPivots[2],
            subTitle = new System.Text.StringBuilder("나는 왜 나무 위를 올라갔던 것일까?, 기억이 전혀 나지 않는다"),
            audioNum = 0,
            work = null,
            pivotTimer = 0.5f
        };
        walkingPivots.Add(walkingPivot);

        walkingPivot = new WalkingPivot()
        {
            pivot = tempPivots[2],
            nextPivot = tempPivots[3],
            subTitle = null,
            audioNum = 0,
            work = null,
            pivotTimer = 3.0f
        };
        walkingPivots.Add(walkingPivot);

        walkingPivot = new WalkingPivot()
        {   
            pivot = tempPivots[3],
            nextPivot = tempPivots[4],
            subTitle = new System.Text.StringBuilder("이 곳에서 그 기억도 찾을 수 있을까?"),
            audioNum = 0,
            work = ZoomInOut(),
            pivotTimer = 3.0f
        };
        walkingPivots.Add(walkingPivot);

        walkingPivot = new WalkingPivot()
        {
            pivot = tempPivots[4],
            nextPivot = tempPivots[5],
            subTitle = null,
            audioNum = 0,
            work = null,
            pivotTimer = 2.0f
        };
        walkingPivots.Add(walkingPivot);

    }

    private void SetWalking2Pivot(List<WalkingPivot> walkingPivots)
    {
        List<GameObject> tempPivots = new List<GameObject>();
        foreach (Transform child in cameraWalkingPivot.transform.GetChild(1))
            tempPivots.Add(child.gameObject);

        GameObject footrest = GameObject.FindGameObjectWithTag("FootRest").transform.GetChild(0).gameObject;

        WalkingPivot walkingPivot = new WalkingPivot()
        {
            pivot = footrest,
            nextPivot = footrest,
            subTitle = null,
            audioNum = 1,
            work = null,
            pivotTimer = 0.01f
        };

        for(int i=0;i<5;i++)
            walkingPivots.Add(walkingPivot);

        walkingPivot = new WalkingPivot()
        {
            pivot = footrest,
            nextPivot = tempPivots[0],
            subTitle = null,
            audioNum = 0,
            work = null,
            pivotTimer = 1.0f
        };
        walkingPivots.Add(walkingPivot);
    }

    private IEnumerator ZoomInOut()
    {
        float origin = Camera.main.fieldOfView;

        for (int i = 0; i < 30; i++)
        {
            Camera.main.fieldOfView -= 1f;
            yield return new WaitForSeconds(0.001f);
        }

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < 30; i++)
        {
            Camera.main.fieldOfView += 1f;
            yield return new WaitForSeconds(0.001f);
        }

        Camera.main.fieldOfView = origin;
        yield return new WaitForSeconds(0.01f);

        yield break;
    }

    private IEnumerator CameraWalkingFromTo(Vector3 from, Vector3 to)
    {
        float step = 0.01f;

        //Correction
        Vector3 toVec = to;
        toVec = new Vector3(toVec.x, transform.parent.position.y, toVec.z);

        for (int i = 0; i < 100; i++)
        {
            transform.parent.position = Vector3.Lerp(from, toVec, step);
            step += 0.01f;
            yield return new WaitForSeconds(0.02f);
        }

        yield break;
    }

    protected IEnumerator JumpTo(Vector3 orginFrom, Vector3 originTo)
    {
        float step = 0f;
        SetCameraStop(true);
        transform.parent.GetComponent<Rigidbody>().AddForce(transform.parent.up * 2.5f, ForceMode.Impulse);
        jumpSound.Play();

        while (true)
        {
            if (step > 1)
            {
                SetCameraStop(false);
                yield break;
            }
            Vector3 nextVec = Vector3.Slerp(orginFrom, originTo, step);
            nextVec.y = transform.parent.position.y;
            transform.parent.position = nextVec;
            step += 0.03f;

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void SetJumpTo(Vector3 orginFrom, Vector3 originTo)
    {
        StartCoroutine(JumpTo(orginFrom, originTo));
    }
}


