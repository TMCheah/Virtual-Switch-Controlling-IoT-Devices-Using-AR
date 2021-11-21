using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class HouseDriver : MonoBehaviour
{
    //store the rooms and lights game object
    public GameObject[] rooms;
    public GameObject[] lights;

    //call all the manager
    //karen got things to complaint!
    //bringing along agent JSON
    private HouseManager houseManager;
    private List<RoomManager> roomManager;
    private RoomManager rm;
    private List<DeviceManager> deviceManager;
    private DeviceManager dm;
    public ESP8266Agent agent;
    private ESP8266Agent.Root json;

    //counter for room and device
    //get from json text
    private int RoomNum;
    private int deviceNum;

    //check if initialized
    bool init = false;

    private async void Start()
    {
        reConenct();
        await wait(8000);
        //getJSON();
        try
        {
            initialize();
        }
        catch (Exception e)
        {
            await wait(3000);
            try
            {
                initialize();
            }
            catch (Exception ee)
            {
                init = false;
            }
        }
        
    }
    
    //initialize GameObject;
    void initialize()
    {
        int lightLength = 0;
        
        RoomNum = json.house.room.Length;
        roomManager = new List<RoomManager>();
        for (int i = 0; i < RoomNum; i++)
        {
            deviceNum = json.house.room[i].device.Length;
            deviceManager = new List<DeviceManager>();
            for (int j = 0; j < deviceNum; j++)
            {
                dm = lights[j + lightLength].AddComponent<DeviceManager>();
                dm.setDevice(lights[j + lightLength]);
                dm.setDeviceNum(json.house.room[i].device[j].deviceNum - 1);
                dm.setType(json.house.room[i].device[j].type);
                if (json.house.room[i].device[j].status.Equals("ON"))
                    dm.setStatus(true);
                else
                    dm.setStatus(false);
                Debug.Log(dm.getDeviceNum());
                deviceManager.Add(dm);
            }
            rm = rooms[i].AddComponent<RoomManager>();
            rm.setRoom(rooms[i]);
            rm.setRoomNum(json.house.room[i].roomNum);
            rm.setDevice(deviceManager);
            roomManager.Add(rm);
    
            lightLength += deviceNum;
            deviceManager = null;
        }
        houseManager = new HouseManager(roomManager);
        init = true;
     }

    void getJSON()
    {
        json = agent.getJSON();
    }

    void setLightObject()
    {
        for (int i = 0; i < houseManager.getRoomManager().Count; i++)
        {
            for (int j = 0; j < houseManager.getRoomManager()[i].getDeviceManager().Count; j++)
            {
                houseManager.getRoomManager()[i].getDeviceManager()[j].getDevice().SetActive(houseManager.getRoomManager()[i].getDeviceManager()[j].getStatus());                
            }
        }
    }

    public HouseManager getHouseManager()
    {
        return houseManager;
    }

    private Task wait(int time)
    {
        return Task.Factory.StartNew(() =>
            {
            Thread.Sleep(time);
            });
    }

    public async void reConenct()
    {
        Toast.ShowMessage("Initializing", Toast.Position.bottom, Toast.Time.threeSecond, this);
        await wait(5000);
        getJSON();
        try
        {
            initialize();
        }
        catch (Exception e)
        {
            await wait(3000);
            try
            {
                initialize();
            }
            catch (Exception ee)
            {
                init = false;
                Toast.ShowMessage("Initializing Fail, press refresh icon to refresh", Toast.Position.bottom, Toast.Time.fiveSecond, this);
                await wait(1000);
            }
        }
        Debug.Log(init);
        if (init)
        {
            setLightObject();
            Toast.ShowMessage("Initialized. scan QRcode", Toast.Position.bottom, Toast.Time.fiveSecond, this);
        }
        /*
        else
        {
            Toast.ShowMessage("Initializing Fail, reconenct in 10s", Toast.Position.bottom, Toast.Time.threeSecond, this);
            await wait(10000);
        }
        */
    }
}
