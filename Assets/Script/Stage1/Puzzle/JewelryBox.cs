using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelryBox : GameCamera
{
    private Animator jewelryBoxAnimation;
    private List<GameObject> buttons = new List<GameObject>();
    private List<int> numbers = new List<int>();
    private List<Texture2D> images = new List<Texture2D>();

    private AudioSource buttonSound;

    protected override void Awake()
    {
        base.Awake();

        InitializeParent(40.0f, transform.GetChild(1).GetChild(1).gameObject, transform.GetChild(0).gameObject);
        jewelryBoxAnimation = GetComponent<Animator>();

        List<Sprite> tempList = new List<Sprite>(Resources.LoadAll<Sprite>("Texture/House/Furnitures/part2/box number_D"));
        for (int i = 0; i < tempList.Count; i++)
            images.Add(SpriteConverter(tempList[i]));

        foreach (Transform button in transform.GetChild(1))
        {
            buttons.Add(button.gameObject);
            numbers.Add(0);
        }

        buttons[0].GetComponent<Renderer>().material.SetTexture("_MainTex", images[numbers[0]]);
        buttons[1].GetComponent<Renderer>().material.SetTexture("_MainTex", images[numbers[0]]);
        buttons[2].GetComponent<Renderer>().material.SetTexture("_MainTex", images[numbers[0]]);

        buttonSound = AudioSetter.SetEffect(gameObject, "Sound/Stage1/Part2/OpenBoxNumber");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState != GameState.Playing) return;


        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10f))
            {
                Transform target = hit.transform;
                if (target == null) return;

                Debug.Log(target);

                ProcessForKey(target);

            }
        }

        //Terminate Game
        if (Input.GetKeyUp(KeyCode.Escape) && gameState != GameState.End)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            EndGame(GameState.NotPlaying);
        }
    }

    public void StartJewelryBox()
    {
        if (gameState == GameState.End) return;

        StartGame();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public bool GetGameEnd()
    {
        return gameState == GameState.End;
    }

    private void ProcessForKey(Transform target)
    {
        switch(target.name)
        {
            case "box number1":
                numbers[0] = numbers[0] + 1 > 9 ? 0 : numbers[0] + 1;
                Debug.Log(numbers[0]);
                buttons[0].GetComponent<Renderer>().material.SetTexture("_MainTex", images[numbers[0]]);
                buttonSound.Play();
                break;
            case "box number2":
                numbers[1] = numbers[1] + 1 > 9 ? 0 : numbers[1] += 1;
                Debug.Log(numbers[1]);
                buttons[1].GetComponent<Renderer>().material.SetTexture("_MainTex", images[numbers[1]]);
                buttonSound.Play();
                break;
            case "box number3":
                numbers[2] = numbers[2] + 1 > 9 ? 0 : numbers[2] += 1;
                Debug.Log(numbers[2]);
                buttons[2].GetComponent<Renderer>().material.SetTexture("_MainTex", images[numbers[2]]);
                buttonSound.Play();
                break;
        }

        if (target.name == "Piano") return;
        
        if (Checker())
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            jewelryBoxAnimation.SetTrigger("Open");
            EndGame(GameState.End);
        }
    }

    private bool Checker()
    {
        return numbers[0] == 6 && numbers[1] == 4 && numbers[2] == 5;
    }

    private Texture2D SpriteConverter(Sprite sprite)
    {
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        Color[] colors = sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);

        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }
}
