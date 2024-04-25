using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public Transform teleportLocation;
    public Transform cameraRig;
    public Transform headset;

    [Space]

    public bool teleportWhenEnable = false;
    public bool doNotChangeHeight = false;

    WaitForSeconds fewMillisecond = new WaitForSeconds(0.01f);

    private void Awake()
    {
        // if (cameraRig==null)
        // {
        //     cameraRig = FindObjectOfType<OVRManager>().transform;
        // }
        // if (headset==null)
        // {
        //     headset = FindObjectOfType<OVRScreenFade>().transform;
        // }
    }
    private void OnEnable()
    {
        StartCoroutine(FillVariable());

        if (teleportWhenEnable)
            StartCoroutine(TeleportWithDelay());
    }

    IEnumerator FillVariable()
    {
        yield return fewMillisecond;

        if (teleportLocation == null) teleportLocation = GetComponent<Transform>();
    }

    IEnumerator TeleportWithDelay()
    {
        yield return fewMillisecond;

        if (teleportLocation == null) teleportLocation = GetComponent<Transform>();

        TeleportCameraRig();
    }

    [DrawButton]
    public void TeleportCameraRig()
    {
        StartCoroutine(teleportToGiven());
    }
    IEnumerator teleportToGiven()
    {
        // cameraRig.SetPositionAndRotation(CalculatePosition(), CalculateRotation());
        cameraRig.rotation = CalculateRotation();
        cameraRig.position = CalculatePosition();

        ResetRigRotation();
        yield return fewMillisecond;
        cameraRig.position = CalculatePosition();
    }

    Quaternion CalculateRotation()
    {
        return Quaternion.Inverse(headset.rotation) * cameraRig.rotation * teleportLocation.rotation;
    }

    Vector3 CalculatePosition()
    {
        Vector3 rigPosition = cameraRig.position - (headset.position - teleportLocation.position);
        if (doNotChangeHeight)
            rigPosition.y = cameraRig.position.y;
        else
            rigPosition.y = teleportLocation.position.y;
        return rigPosition;
    }

    void ResetRigRotation()
    {
        // Euler implementation
        // Vector3 camEuler = cameraRig.localEulerAngles;
        // camEuler.x = 0f;
        // camEuler.z = 0f;
        // cameraRig.localEulerAngles = camEuler;

        // Quaternion implementation
        float y = cameraRig.localRotation.eulerAngles.y;
        // cameraRig.localRotation = Quaternion.identity;
        cameraRig.localRotation = Quaternion.AngleAxis(y, Vector3.up);
    }



    // Draw Gizmos cube of [1 feet x 1 feet x 6 feet] at the teleport location
    Vector3 cubeSize = new Vector3(0.3048f, 1.8288f, 0.3048f);
    Vector3 yOffset = new Vector3(0f, 1.8288f / 2f, 0f);
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Matrix4x4 tempMat = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(teleportLocation.position, teleportLocation.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero + yOffset, cubeSize);
        Gizmos.matrix = tempMat;
        Gizmos.DrawLine(teleportLocation.position, teleportLocation.position + teleportLocation.forward * cubeSize.y);
    }
}