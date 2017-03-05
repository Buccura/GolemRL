using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraShift : MonoBehaviour {

    private Vector3 fwd;
    private Vector3 startPos;

    private Vector3 downPos;
    private Quaternion downRot;
    private Vector3 upPos;
    private Quaternion upRot;

    void Start()
    {
        downPos = new Vector3(-0.4f, 10.95f, -6.23f);
        downRot = new Quaternion(60.04f, 0f, 0f, 0f);
        upPos = new Vector3(-0.4f, 10.95f, -0.28f);
        upRot = new Quaternion(88.04f, 0f, 0f, 0f);

        startPos = transform.localPosition;
        fwd = transform.TransformDirection(Vector3.forward);
    }
	
	void Update ()
    {
   /*     RaycastHit hit;
        startPos = transform.localPosition;
        fwd = transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(startPos,fwd, out hit))
        {
            Debug.Log(hit.collider.gameObject);
            if(hit.collider.gameObject.tag == "Wall")
            {
                transform.localPosition = new Vector3(-0.4f, 10.95f, Mathf.Lerp(transform.localPosition.z, upPos.z, Time.deltaTime));
                transform.localRotation = new Quaternion(Mathf.Lerp(transform.localRotation.x, upRot.x, Time.deltaTime), 0f, 0f, 0f);
            }
            else
            {
                transform.localPosition = new Vector3(-0.4f, 10.95f, Mathf.Lerp(transform.localPosition.x, downPos.z, Time.deltaTime));
                transform.localRotation = new Quaternion(Mathf.Lerp(transform.localRotation.x, downRot.x, Time.deltaTime), 0f, 0f, 0f);
            }
        }*/
    }
}
