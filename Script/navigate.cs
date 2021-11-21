using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class navigate : MonoBehaviour
{
    public GameObject gesture;
    public UnityEvent floorTrigger;
    public UnityEvent floorUnTrigger;
    public UnityEvent roomnTrigger;
    public UnityEvent roomUnTrigger;
    dragController drag;

    // Start is called before the first frame update
    void Start()
    {
        if(this.GetComponent<MeshRenderer>() != null)
            this.GetComponent<MeshRenderer>().enabled = false;
        drag = gesture.GetComponent<dragController>();
    }

    private void OnEnable()
    {
        drag = gesture.GetComponent<dragController>();
    }

    private void OnMouseDown()
    {
        floorTrigger.Invoke();
        roomnTrigger.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (drag.swipeLeft)
        {
            Debug.Log("swiped left");
            floorUnTrigger.Invoke();
            roomUnTrigger.Invoke();
        }
    }
}
