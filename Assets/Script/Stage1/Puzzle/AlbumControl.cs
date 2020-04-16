using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlbumControl : MonoBehaviour
{
    private Vector3 standardPos;
    private Vector3 frontPos;
    private Quaternion standardQuatern;

    private CameraControl cameraControl;
    private ProgressManager progressManager;
    private Animator animator;

    private Texture completeSprite;
    private bool coroutineActivate = false;
    private bool isActive = false;
    private bool isCompleted = false;

    private AudioSource albumClickSound;
    private AudioSource alblumOpenSound;
    private AudioSource albumGetSound;

    public enum AlbumType { Album1, Album2, Album3 }
    public AlbumType type;


    private void Awake()
    {
        standardPos = transform.position;
        standardQuatern = transform.rotation;
        cameraControl = Camera.main.GetComponent<CameraControl>();
        progressManager = ProgressManager.GetInstance;
        animator = GetComponent<Animator>();

        albumClickSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part2/ClickAlbums");
        alblumOpenSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part2/OpenAlbums");
        albumGetSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part2/GetAlbums");

        switch (type)
        {
            case AlbumType.Album1:
                completeSprite = Resources.Load<Texture>("Texture/House/Furnitures/Ohters/Album1Complete_D");
                break;
            case AlbumType.Album2:
                completeSprite = Resources.Load<Texture>("Texture/House/Furnitures/Ohters/Album2Complete_D");
                break;
            case AlbumType.Album3:
                completeSprite = Resources.Load<Texture>("Texture/House/Furnitures/Ohters/Album3Complete_D");
                break;
        }

        frontPos = transform.GetChild(0).position;
    }

    private IEnumerator ZoomIn()
    {
        coroutineActivate = true;
        isActive = true;
        float step = 0;
        cameraControl.SetCameraStop(true);
        yield return StartCoroutine(GetFront());

        while (true)
        {
            if (step > 1)
            {
                transform.rotation = Camera.main.transform.GetChild(0).rotation;
                transform.position = Camera.main.transform.GetChild(0).position;
                animator.SetTrigger("Open");
                alblumOpenSound.Play();
                coroutineActivate = false;
                yield break;
            }

            transform.rotation = Quaternion.Slerp(standardQuatern, Camera.main.transform.GetChild(0).rotation, step);
            transform.position = Vector3.Slerp(frontPos, Camera.main.transform.GetChild(0).position, step);

            step += 0.02f;
            yield return new WaitForSeconds(0.001f);
        }
    }

    private IEnumerator GetFront()
    {
        float step = 0;
        albumClickSound.Play();

        while (true)
        {
            if (step > 1)
            {
                transform.position = frontPos;
                yield break;
            }

            transform.position = Vector3.Slerp(standardPos, frontPos, step);

            step += 0.02f;
            yield return new WaitForSeconds(0.001f);
        }
    }

    private IEnumerator ZoomOut()
    {
        float step = 0;
        coroutineActivate = true;
        cameraControl.SetCameraStop(true);
        animator.SetTrigger("Close");
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            if (step > 1)
            {
                transform.rotation = standardQuatern;
                transform.position = frontPos;
                yield return StartCoroutine(SetBack());
                coroutineActivate = false;
                yield break;
            }

            transform.rotation = Quaternion.Slerp(Camera.main.transform.GetChild(0).rotation, standardQuatern, step);
            transform.position = Vector3.Slerp(Camera.main.transform.GetChild(0).position, frontPos, step);

            step += 0.02f;
            yield return new WaitForSeconds(0.001f);
        }
    }

    private IEnumerator SetBack()
    {
        float step = 0;

        while (true)
        {
            if (step > 1)
            {
                transform.position = standardPos;
                isActive = false;
                cameraControl.SetCameraStop(false);
                yield break;
            }

            transform.position = Vector3.Slerp(frontPos, standardPos, step);

            step += 0.02f;
            yield return new WaitForSeconds(0.001f);
        }
    }

    public void StartZoom(bool isComplete)
    {
        if (isActive) return;

        if (isComplete)
            transform.GetChild(1).GetComponent<Renderer>().material.SetTexture("_MainTex", completeSprite);

        isCompleted = isComplete;
        StartCoroutine(ZoomIn());
    }

    // Update is called once per frame
    void Update()
    {
        if (coroutineActivate) return;
        if (!isActive) return;

        if (!isCompleted)
        {
            if (Input.GetMouseButtonUp(0))
                StartCoroutine(ZoomOut());
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                albumGetSound.Play();
                cameraControl.SetCameraStop(false);
                Destroy(gameObject);

                switch (type)
                {
                    case AlbumType.Album1:
                        progressManager.AddItem(ItemSubMenu.ItemType.Album12);
                        break;
                    case AlbumType.Album2:
                        progressManager.AddItem(ItemSubMenu.ItemType.Album22);
                        break;
                    case AlbumType.Album3:
                        progressManager.AddItem(ItemSubMenu.ItemType.Album32);
                        break;
                }

            }
        }
    }
}
