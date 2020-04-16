using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll1Controller : MonoBehaviour
{
    private GameObject leg;
    private GameObject arm;

    private bool armReady;
    private bool legReady;

    private void Awake()
    {
        arm = transform.GetChild(0).gameObject;
        leg = transform.GetChild(2).gameObject;

        arm.SetActive(false);
        leg.SetActive(false);
    }

    public void AddArm()
    {
        arm.SetActive(true);
        armReady = true;
    }

    public void AddLeg()
    {
        leg.SetActive(true);
        legReady = true;
    }

    public bool IsCompleted()
    {
        return armReady && legReady;
    }
}
