using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using UnityEngine.UI;


public class ExcelReader : MonoBehaviour
{
    public TextAsset textAssetData;
    public GameObject toothBrush;
    public GameObject jaw;
    public TMP_Text messageTxt;
    public TMP_Text stepsTxt;
    public Button stepButton;

    int stepNo = 0;
    Transform go;
    string[] data;
    private void Start()
    {
        data = textAssetData.text.Split(new string[] {";","\n"}, System.StringSplitOptions.None);
    }

    public  void ChangeStep()
    {
        if(stepNo < data.Length)
        {
            stepNo++;
            StartAction();
            stepButton.interactable = false;
        }
    }
    public void EnableButton()
    {
        stepButton.interactable = true;
    }
    string[] row;
    public void StartAction()
    {

            row = data[stepNo].Split(new char[]{','});

            if(row[1] != "")
            {

                if(row[2] == "Toothbrush")
                {
                    go = toothBrush.transform;
                }
                else if(row[2] == "Jaw")
                {
                    go = jaw.transform;
                }
                else
                {
                    // go = toothBrush.transform;
                }
                messageTxt.text = row[0] + "/" + row[2] + "/" + row[3];
                stepsTxt.text = row[1];

                if(row[3] == "Starting" || row[3] == "Move")
                {
                    MoveAction(Int32.Parse(row[1]), go, row[3], float.Parse(row[4]), float.Parse(row[5]), float.Parse(row[6]), float.Parse(row[7]), row[8], Int32.Parse(row[9]), row[10], float.Parse(row[11]));
                }
                else if(row[3] == "Rotate")
                {
                    RotateAction(Int32.Parse(row[1]), go, row[3], float.Parse(row[4]), float.Parse(row[5]), float.Parse(row[6]), float.Parse(row[7]), row[8], Int32.Parse(row[9]), row[10], float.Parse(row[11]));
                }
                else if(row[3] == "Circular")
                {
                    CircularAction(Int32.Parse(row[1]), go, row[3], float.Parse(row[4]), float.Parse(row[5]), float.Parse(row[6]), float.Parse(row[7]), row[8], Int32.Parse(row[9]), row[10], float.Parse(row[11]));
                }
                else
                {
                    Debug.Log("Please define a Automation name");
                }
            }
    }

    void MoveAction(int stepnumber, Transform obj, string action, float X, float Y, float Z, float radius, string direction, int repeat, string media, float speed)
    {
        StartCoroutine(MoveTo(obj, new Vector3(X, Y, Z), 1));
    }

    void RotateAction(int stepnumber, Transform obj, string action, float X, float Y, float Z, float radius, string direction, int repeat, string media, float speed)
    {
        StartCoroutine(RotateTo(obj, new Vector3(X, Y, Z), 1));
    }

    void CircularAction(int stepnumber, Transform obj, string action, float X, float Y, float Z, float radius, string direction, int repeat, string media, float speed)
    {
        StartCoroutine(CircularTo(obj, new Vector3(X, Y, Z), 1, radius, speed, repeat));
    }


    IEnumerator MoveTo(Transform obj, Vector3 endPosition, float time)
    {
        Vector3 start = obj.position;
        Vector3 end = endPosition;
        float t = 0;

        while(t < 1)
        {
            yield return null;
            t += Time.deltaTime / time;
            obj.position = Vector3.Lerp(start, end, t);
        }
        obj.position = end;

        EnableButton();
    }
    
    IEnumerator RotateTo(Transform obj, Vector3 endPosition, float time)
    {
        Vector3 end = endPosition;
        float t = 0;

        while(t < 1)
        {
            yield return null;
            t += Time.deltaTime / time;
            obj.rotation = Quaternion.Lerp(obj.rotation, Quaternion.Euler(endPosition), t);
        }
        obj.rotation = Quaternion.Euler(end);

        EnableButton();
    }
    
    float posX, posY, angle = 0f;
    public Transform rotCenter;

    IEnumerator CircularTo(Transform obj, Vector3 endPosition, float time, float rotRadius, float angularSpeed, int repeat)
    {
        float t = 0;

        while(t < repeat)
        {
            yield return null;

            posX = rotCenter.position.x + Mathf.Cos(angle) * rotRadius;
            posY = rotCenter.position.y + Mathf.Sin(angle) * rotRadius;
            toothBrush.transform.position = new Vector3(posX, posY, 0.537f);
            angle = angle + Time.deltaTime * angularSpeed;

            if(angle >= 6f)
            {
            angle = 0f;
            t++;
            }
        }

        EnableButton();
    }

}
