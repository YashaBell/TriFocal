using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBox : MonoBehaviour
{
    public PlayerMovement PM;
    public float windForce;
    public Transform windBase;
    public Transform playerPos;
    public Rigidbody player;
    public ParticleSystem speedLines;

    /*public void OnTriggerStay(Collider collid)
    {
        if (collid.attachedRigidbody)
        {
            collid.attachedRigidbody.AddForce(windBase.up * windForce);
            Debug.DrawRay(playerPos.position, windBase.up, Color.red);
            Debug.Log("rayDisplay");
        }
    }*/

    public void OnTriggerEnter(Collider collid)
    {
        if (collid.attachedRigidbody && collid.CompareTag("Player"))
        {
            speedLines.Play();
            collid.attachedRigidbody.AddForce(windBase.up * windForce);
        }
    }

    public void OnTriggerExit()
    {
        speedLines.Stop();
    }
}
   

