using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerController : MonoBehaviour
{
    private GameObject flower;
    private GameObject flowerBud;

    private void Awake()
    {
        flower = transform.GetChild(0).gameObject;
        flowerBud = transform.GetChild(1).gameObject;

        flower.SetActive(false);
    }

    public void SetBlossom()
    {
        flowerBud.SetActive(false);
        flower.SetActive(true);
    }
}
