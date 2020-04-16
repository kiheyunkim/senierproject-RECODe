using UnityEngine;
using System.Collections;

public class CameraFacing : MonoBehaviour
{
	public Camera cameraToLookAt;
	void Awake()
    {
		cameraToLookAt = Camera.main ?? null;
    }

    void Update() 
	{
        if (cameraToLookAt == null) return;

		Vector3 v = cameraToLookAt.transform.position - transform.position;
		v.x = v.z = 0.0f;
   		transform.LookAt(cameraToLookAt.transform.position - v); 
	}
}