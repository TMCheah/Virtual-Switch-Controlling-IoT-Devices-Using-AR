using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceManager : MonoBehaviour
{
    private GameObject device;
    private int deviceNum;
    private string type;
    private bool status;
    private bool toggle;

    public DeviceManager(GameObject device, int deviceNum, string type)
    {
        this.device = device;
        this.deviceNum = deviceNum;
        this.type = type;
        status = false ;
    }

    public DeviceManager(int deviceNum, string type)
    {
        this.deviceNum = deviceNum;
        this.type = type;
        status = false;
    }

    public void setDevice(GameObject device)
    {
        this.device = device;
    }

    public GameObject getDevice()
    {
        return this.device;
    }

    public void setDeviceNum(int deviceNum)
    {
        this.deviceNum = deviceNum;
    }

    public int getDeviceNum()
    {
        return this.deviceNum;
    }

    public void setType(string type)
    {
        this.type = type;
    }

    public string getType()
    {
        return this.type;
    }

    public void setStatus(bool status)
    {
        this.status = status;
    }

    public bool getStatus()
    {
        return this.status;
    }

    public void toggleStatus()
    {
        toggle = getStatus();
        if (toggle)
            setStatus(!toggle);
        else
            setStatus(!toggle);
    }

}
