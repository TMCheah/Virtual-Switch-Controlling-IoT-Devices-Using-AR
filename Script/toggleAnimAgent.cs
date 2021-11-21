using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggleAnimAgent : MonoBehaviour
{
    public GameObject obj;
    public string getOn;
    public string getOff;
    public bool toggle;
    Animator anim;
    bool init = false;

    public Renderer[] rend;
    public Renderer[] Off;
    public Renderer[] On;
    public Material onMat;
    public Material offMat;
    public Material defaultMat;


    public void OnMouseDown()
    {
        if (!init)
        {
            anim = obj.GetComponent<Animator>();
            init = true;
        }
        Debug.Log("pressed");
        if (toggle)
            toggle = false;
        else
            toggle = true;
        play(toggle);
    }

    public void play(bool on)
    {
        Debug.Log("animate start?");
        if (!init)
        {
            anim = obj.GetComponent<Animator>();
            init = true;
        }
        if (on)
        {
            Debug.Log("on");
            anim.Play("on");
            for (int i = 0; i < rend.Length; i++)
                rend[i].material = onMat;
            for (int i = 0; i < On.Length; i++)
                On[i].material = onMat;
            for (int i = 0; i < Off.Length; i++)
                Off[i].material = defaultMat;
        }
        else
        {
            Debug.Log("off");
            anim.Play("off");
            //Color col = new Color(255, 0, 0);
            for (int i = 0; i < rend.Length; i++)
                rend[i].material = offMat;
            for (int i = 0; i < On.Length; i++)
                On[i].material = defaultMat;
            for (int i = 0; i < Off.Length; i++)
                Off[i].material = offMat;
        }
        Debug.Log("animate end");
    }


}
