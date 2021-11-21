using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseManager : MonoBehaviour
{
    private List<RoomManager> roomManager;

    public HouseManager(List<RoomManager> roomManager)
    {
        this.roomManager = roomManager;
    }

    public void setRoomManager(List<RoomManager> roomManager)
    {
        this.roomManager = roomManager;
    }

    public List<RoomManager> getRoomManager()
    {
        return this.roomManager;
    }

}
