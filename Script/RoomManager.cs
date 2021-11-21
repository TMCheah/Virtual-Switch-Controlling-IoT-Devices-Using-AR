using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private GameObject room;
    private List<DeviceManager> deviceManager;
    private int roomNum;

    public RoomManager(GameObject room, int roomNum, List<DeviceManager> deviceManager)
    {
        this.room = room;
        this.roomNum = roomNum;
        this.deviceManager = deviceManager;
    }

    public void setRoom(GameObject room)
    {
        this.room = room;
    }

    public GameObject getRoom()
    {
        return this.room;
    }

    public void setRoomNum(int roomNum)
    {
        this.roomNum = roomNum;
    }

    public int getRoomNum()
    {
        return this.roomNum;
    }

    public void setDevice(List<DeviceManager> deviceManager)
    {
        this.deviceManager = deviceManager;
    }

    public List<DeviceManager> getDeviceManager()
    {
        return this.deviceManager;
    }

    public void addDevice(DeviceManager deviceManager)
    {
        this.deviceManager.Add(deviceManager);
    }


}
