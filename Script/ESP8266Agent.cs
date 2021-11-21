using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ESP8266Agent : MonoBehaviour
{
    //for json's key and value class;
    [System.Serializable]
    public class Device
    {
        public int deviceNum;
        public string type;
        public string status;
    }

    [System.Serializable]
    public class Room
    {
        public int roomNum;
        public Device[] device;
    }

    [System.Serializable]
    public class House
    {
        public Room[] room;
    }

    [System.Serializable]
    public class Root
    {
        public House house;
    }

    //ip address of webserver
    public string IP;
    //check if done reading from server;
    private bool done = false;
    //get the json into root;
    private Root root = null;

    private long responseCode;

    private void Start()
    {
        StartCoroutine(read());
        //StartCoroutine(ManualRead());
    }

    public Root getJSON()
    {
        return root;
    }

    public void requestState()
    {
        StartCoroutine(read());
    }

    IEnumerator read()
    {
        using (UnityWebRequest request = new UnityWebRequest(IP))
        {
            yield return request.SendWebRequest();

            if (request.GetResponseHeaders().Count > 0)
            {
                foreach (KeyValuePair<string, string> entry in request.GetResponseHeaders())
                {
                    if (entry.Key.Contains("json"))
                    {
                        print(entry.Value);
                        root = JsonUtility.FromJson<Root>(entry.Value);
                        //print(root.house.room[0].device[0].deviceNum);
                        print(root.house.room.Length);
                    }
                }
            }
            done = true;
            request.Dispose();
            Debug.Log("JUST Done...");
        }
    }

    IEnumerator ManualRead()
    {
        yield return new WaitForSeconds(1);
        string json = "{\"house\":{\"room\":[{\"roomNum\":1,\"device\":[{\"deviceNum\":1,\"type\":\"LED\",\"status\":\"OFF\"}]},{\"roomNum\":2,\"device\":[{\"deviceNum\":2,\"type\":\"LED\",\"status\":\"ON\"}]},{\"roomNum\":3,\"device\":[{\"deviceNum\":3,\"type\":\"LED\",\"status\":\"ON\"}]},{\"roomNum\":4,\"device\":[{\"deviceNum\":4,\"type\":\"LED\",\"status\":\"ON\"}]},{\"roomNum\":5,\"device\":[{\"deviceNum\":5,\"type\":\"LED\",\"status\":\"ON\"}]},{\"roomNum\":6,\"device\":[{\"deviceNum\":6,\"type\":\"LED\",\"status\":\"ON\"}]},{\"roomNum\":7,\"device\":[{\"deviceNum\":7,\"type\":\"LED\",\"status\":\"ON\"}]}]}}";
        root = JsonUtility.FromJson<Root>(json);
        //print(root.house.room[0].device[0].deviceNum);
        print(root.house.room.Length);
        done = true;
    }

    //send
    
    public void manualSendState(string state, string deviceNum)
    {
        UnityWebRequest request = new UnityWebRequest(IP);
        request.SetRequestHeader(state, deviceNum);
        request.SendWebRequest();
        Debug.Log("the label is: ");
        Debug.Log(request.GetRequestHeader(state));
        request.Dispose();

    }

    public void sendState(string state, string deviceNum)
    {
        StartCoroutine(send(state, deviceNum));
    }

    IEnumerator send(string state, string deviceNum)
    {
        using (UnityWebRequest request = new UnityWebRequest(IP))
        {
            request.SetRequestHeader(state, deviceNum);
            yield return request.SendWebRequest();
            Debug.Log("responseCode: ");
            Debug.Log(request.responseCode);
            Debug.Log("NetworkError?: ");
            Debug.Log(request.isNetworkError);
            Debug.Log("HttpError?: ");
            Debug.Log(request.isHttpError);
            responseCode = request.responseCode;
            request.Dispose();
            //return 203 meaning is busy
        }
    }

    public long getResponseCode()
    {
        return this.responseCode;
    }

    IEnumerator syncJSON(int time)
    {
        done = false;
        yield return new WaitForSeconds(time);
        StartCoroutine(read());
        Debug.Log("Finish Waiting");
    }

    void Update()
    {
        if (done)
        {
            Debug.Log("resyncing");
            StartCoroutine(syncJSON(120));
        }
    }

}
