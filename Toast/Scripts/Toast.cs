﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Toast : MonoBehaviour {

    public enum Time
    {
        fiveSecond,
        threeSecond,
        twoSecond,
        oneSecond
    };
    public enum Position
    {
        top,
        bottom
    };
    public static void ShowMessage ( string msg, Toast.Position position, Toast.Time time, MonoBehaviour instance)
    {

        //Load message prefab from resources folder
        GameObject messagePrefab = Resources.Load ( "Message" ) as GameObject;
        //Get container object of message
        GameObject containerObject = messagePrefab.gameObject.transform.GetChild ( 0 ).gameObject;
        //Get text object
        GameObject textObject = containerObject.gameObject.transform.GetChild ( 0 ).GetChild ( 0 ).gameObject;
        //Get text property
        Text msg_text = textObject.GetComponent<Text> ( );
        //Set message to text ui
        msg_text.text = msg;
        //Set position of container object of message
        SetPosition ( containerObject.GetComponent<RectTransform> ( ), position );
        //Spawn message object with all changes
        GameObject clone = Instantiate ( messagePrefab );
        // Destroy clone of message object according to the time
        //RemoveClone ( clone, time );
        instance.StartCoroutine(Remove(clone, time));
    }

    private static void SetPosition ( RectTransform rectTransform, Position position )
    {
        if (position == Position.top)
        {
            rectTransform.anchorMin = new Vector2 ( 0.5f, 1f );
            rectTransform.anchorMax = new Vector2 ( 0.5f, 1f );
            rectTransform.anchoredPosition = new Vector3 ( 0.5f, -100f, 0 );
        }
        else
        {
            rectTransform.anchorMin = new Vector2 ( 0.5f, 0 );
            rectTransform.anchorMax = new Vector2 ( 0.5f, 0 );
            rectTransform.anchoredPosition = new Vector3 ( 0.5f, 100f, 0 );
        }
    }

    private static void RemoveClone ( GameObject clone, Time time )
    {
        if (time == Time.oneSecond)
        {
            Destroy ( clone.gameObject, 1f );
        }
        else if (time == Time.twoSecond)
        {
            Destroy ( clone.gameObject, 2f );
        }
        else
        {
            Destroy( clone.gameObject, 3f );
        }
    }

    static IEnumerator Remove(GameObject clone, Time time)
    {
        float waitTime;
        switch (time)
        {
            case Time.oneSecond: waitTime = 1f; break;
            case Time.twoSecond: waitTime = 2f; break;
            case Time.threeSecond: waitTime = 3f; break;
            case Time.fiveSecond: waitTime = 5f; break;
            default: waitTime = 3f; break;
        }
        yield return new WaitForSeconds(waitTime);
        DestroyImmediate(clone.gameObject, false);
    }

}
