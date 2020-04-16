using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
    private enum ScreenType { ITEM, OPTION, SAVE, GOTOMAIN }
    private ScreenType screenType;

    private ProgressManager progressManager;

    private GameObject itemPrefab;
    private GameObject optionPrefab;
    private GameObject savePrefab;
    private GameObject goToMainPrefab;

    private GameObject currentPrefab;
    private GameObject gage;

    private AudioSource clickSound;

    void Awake()
    {
        itemPrefab = Resources.Load<GameObject>("Prefab/Sub-Item");
        optionPrefab = Resources.Load<GameObject>("Prefab/Sub-Option");
        savePrefab = Resources.Load<GameObject>("Prefab/Sub-Save");
        goToMainPrefab = Resources.Load<GameObject>("Prefab/Sub-Main");

        clickSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/RightClick/RightButton");

        progressManager = ProgressManager.GetInstance;
        gage = transform.GetChild(1).gameObject;

        gage.GetComponent<UnityEngine.UI.Image>().fillAmount = (float)progressManager.Progress.Percent / (float)100;
    }

    void Start()
    {
        if (!progressManager.Progress.ItemBagReady) return;

        screenType = ScreenType.ITEM;
        currentPrefab = Instantiate(itemPrefab);//, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        currentPrefab.transform.SetParent(transform, false);
    }

    public void ItemButton()
    {
        if (!progressManager.Progress.ItemBagReady) return;

        if (screenType != ScreenType.ITEM)
        {
            clickSound.Play();
            if (currentPrefab != null)
                Destroy(currentPrefab);
            screenType = ScreenType.ITEM;
            currentPrefab = Instantiate(itemPrefab);//, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            currentPrefab.transform.SetParent(transform,false);
        }
    }

    public void OptionButton()
    {
        if (screenType != ScreenType.OPTION)
        {
            clickSound.Play();
            if (currentPrefab != null)
                Destroy(currentPrefab);
            screenType = ScreenType.OPTION;
            currentPrefab = Instantiate(optionPrefab);//, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            currentPrefab.transform.SetParent(transform, false);
        }
    }

    public void SaveButton()
    {
        if (screenType != ScreenType.SAVE)
        {
            clickSound.Play();
            if (currentPrefab != null)
                Destroy(currentPrefab);
            screenType = ScreenType.SAVE;
            currentPrefab = Instantiate(savePrefab);//, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            currentPrefab.transform.SetParent(transform, false);
        }
    }

    public void GoToMainButton()
    {
        if (screenType != ScreenType.GOTOMAIN)
        {
            clickSound.Play();
            if (currentPrefab != null)
                Destroy(currentPrefab);
            screenType = ScreenType.GOTOMAIN;
            currentPrefab = Instantiate(goToMainPrefab);//, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            currentPrefab.transform.SetParent(transform, false);
        }
    }
}
