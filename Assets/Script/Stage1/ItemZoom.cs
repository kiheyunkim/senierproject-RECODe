using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemZoom : MonoBehaviour
{
    private readonly float maxAperture = 12.0f;
    private readonly float minAperture = 6.0f;

    private UnityEngine.PostProcessing.PostProcessingProfile depthOfFeild;
    private GameObject targetObject = null;
    private UIController uIController;

    private float apertureValue;
    private float step;
    private bool itemGetReady = false;

    private CameraControl cameraControl;
    private AudioSource getBag2Sound;

    private void Awake()
    {//-> 5 ~2.7
        depthOfFeild = Camera.main.GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>().profile;
        uIController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        cameraControl = Camera.main.GetComponent<CameraControl>();
        depthOfFeild.depthOfField.enabled = false;

        getBag2Sound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part1/GetBag2");
    }

    private IEnumerator ItemZoomIn()
    {
        step = 0;
        Vector3 originalItemPos = targetObject.transform.position;
        Vector3 showPos = transform.GetChild(0).position;
        while (true)
        {
            if (step > 1.0f)
            {
                itemGetReady = true;
                yield break;
            }

            apertureValue = Mathf.Lerp(maxAperture, minAperture, step);
            depthOfFeild.depthOfField.settings = new UnityEngine.PostProcessing.DepthOfFieldModel.Settings
            {
                focusDistance = 0.3f,
                useCameraFov = true,
                kernelSize = UnityEngine.PostProcessing.DepthOfFieldModel.KernelSize.VeryLarge,
                aperture = apertureValue
            };

            targetObject.transform.position = Vector3.Slerp(originalItemPos, showPos, step);

            step += 0.03f;

            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator ItemZoomOut()
    {
        step = 0;

        while (true)
        {
            if (step > 1.0f)
            {
                depthOfFeild.depthOfField.enabled = false;
                yield break;
            }

            apertureValue = Mathf.Lerp(minAperture, maxAperture, step);
            depthOfFeild.depthOfField.settings = new UnityEngine.PostProcessing.DepthOfFieldModel.Settings
            {
                focusDistance = 0.3f,
                useCameraFov = true,
                kernelSize = UnityEngine.PostProcessing.DepthOfFieldModel.KernelSize.VeryLarge,
                aperture = apertureValue
            };

            step += 0.03f;

            yield return new WaitForSeconds(0.01f);
        }
    }
    // Update is called once per frame

    void Update()
    {
        if (!itemGetReady) return;

        if (Input.GetMouseButton(0))
        {
            getBag2Sound.Play();
            cameraControl.SetCameraStop(false);
            uIController.SetVisibleCursor(true);
            Destroy(targetObject);
            targetObject = null;
            itemGetReady = false;
            StartCoroutine(ItemZoomOut());
        }

    }

    public void SetTarget(GameObject targetObject)
    {
        this.targetObject = targetObject;
        cameraControl.SetCameraStop(true);
        uIController.SetVisibleCursor(false);
        depthOfFeild.depthOfField.enabled = true;
        StartCoroutine(ItemZoomIn());
    }
}
