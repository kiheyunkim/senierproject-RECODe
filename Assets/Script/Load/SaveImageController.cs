using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveImageController : MonoBehaviour
{
    const float maxDistance = 1000.0f;
    const float minDistance = -10.0f;
    const float zoomSpeed = 1500.0f;

    private UnityEngine.UI.Image saveImage;
    private GameObject timer;
    public bool Activated { get; private set; }
    private float size;


    private void Awake()
    {
        timer = transform.GetChild(0).gameObject;
        saveImage = GetComponent<UnityEngine.UI.Image>();
        timer.SetActive(false);
    }

    private void Start()
    {
        saveImage.material.SetFloat("_Distance", minDistance);
        size = minDistance;
    }

    private IEnumerator ZoomOut()
    {
        while (true)
        {
            saveImage.material.SetFloat("_Distance", size);
            size -= zoomSpeed * Time.smoothDeltaTime;

            if (size < minDistance)
            {
                saveImage.material.SetFloat("_Distance", minDistance);
                yield break;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator ZoomIn(Texture image)
    {
        yield return StartCoroutine(ZoomOut());

        saveImage.material.SetTexture("_Map", image);

        while (true)
        {
            saveImage.material.SetFloat("_Distance", size);
            size += zoomSpeed * Time.smoothDeltaTime;

            if (size > maxDistance)
            {
                saveImage.material.SetFloat("_Distance", maxDistance);
                Activated = false;
                timer.SetActive(true);
                yield break;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void SetSaveImage(Texture image, bool isActivate)
    {
        if (isActivate)
            StartCoroutine(ZoomIn(image));
        timer.SetActive(false);
        Activated = true;
    }

    public void  SetTimer(string time)
    {
        timer.GetComponent<UnityEngine.UI.Text>().text = time;
    }

}
