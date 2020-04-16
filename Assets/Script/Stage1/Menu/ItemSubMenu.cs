using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSubMenu : MonoBehaviour
{
    public enum ItemType
    {
        None, Album12, Album22, Album32, Siccors, MirrorPiece, PendantEmpty, Pendant1
            , Pendant2, Pendant3, Needle, Thread, lpPlate, PianoChair, DollIntegrate
            , dollLeg, dollArm, pendentPic1, pendentPic2, pendentPic3, AlbumPic11, AlbumPic12
            , AlbumPic13, AlbumPic14, AlbumPic21, AlbumPic22, AlbumPic23, AlbumPic24, AlbumPic31
            , AlbumPic32, AlbumPic33, AlbumPic34, thecross
    }

    private List<Sprite> buttonSpriteList = new List<Sprite>();
    private List<GameObject> buttons = new List<GameObject>();
    private List<ItemType> buttonTypes = new List<ItemType>();
    private ProgressManager progressManager;
    private ItemMagnify itemMagnify;

    private AudioSource itemBoxClickSound;

    private void Awake()
    {
        itemBoxClickSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/RightClick/Item/ItemBoxButton");

        progressManager = ProgressManager.GetInstance;

        buttonSpriteList.AddRange(Resources.LoadAll<Sprite>("Texture/Ingame/Inventory items"));

        foreach (Transform child in transform)
            buttons.Add(child.gameObject);

        for (int i = 0; i < progressManager.Progress.itemGets.Count; i++)
            AddItemInItemMenu(progressManager.Progress.itemGets[i]);

        itemMagnify = GameObject.FindGameObjectWithTag("Item Magnify").GetComponent<ItemMagnify>();
    }

    public void ItemButtonClick(int buttonIndex)
    {
        if (buttonTypes.Count < buttonIndex) return;

        itemBoxClickSound.Play();
        itemMagnify.StartMagnify(buttonTypes[buttonIndex - 1]);
    }

    private void AddItemInItemMenu(ItemType item)
    {
        GameObject addtarget = buttons[buttonTypes.Count];                                      //itemType List 에 따라서 순차적으로 삽입 기준을 정함
        int itemType = (int)item;                                                               //따라서 itemType의 count는 삽입 위치의 index의 역할을 함
        addtarget.GetComponent<UnityEngine.UI.Image>().sprite = buttonSpriteList[itemType - 1]; //itemType enum의 숫자는 itemType의 숫자에서 1을 뺀것과 같은 배열을 가짐
        buttonTypes.Add(item);                                                                  // 0번째는 아무것도 없음을 표시하기 위해서  -1을 뺀것
    }
}