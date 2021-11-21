using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class interact : MonoBehaviour
{
    //=====================================interact=========================================//
    HouseDriver HD;
    public GameObject Controller;
    public GameObject LightDevice;
    public int roomNumebr;
    public int deviceNumber;
    //=================================recommendation=======================================//
    public string device;
    private string FileName;
    private string folder;
    private string path;
    private string logTemplate = "logTemplate";
    private string masterTemplate = "masterTemplate";
    private string masterTable = "/masterTable.txt";
    private string recommendMonth = "/recommendMonth.txt";
    private string recommendDay = "/recommendDay.txt";
    string data;
    string[] splitData_hour = new string[24];
    string[] splitData_day;
    string[] splitData_time;
    string[] splitData_ontime;
    string[] splitData_offtime;
    string[] splitData_state;
    string splitData_logtime;
    private const string SPLIT_NEXT = "#next#";
    private const string SPLIT_DAY = "#nextday#";
    private const string SPLIT_HOUR = "#nexthour#";
    private const string SPLIT_DAY_DATA = "#day#";
    private const string SPLIT_HOUR_DATA = "#hh#";
    private const string SPLIT_ONTIME_DATA = "#ontime#";
    private const string SPLIT_OFFTIME_DATA = "#offtime#";
    private const string SPLIT_STATE_DATA = "#state#";
    public hourlyData[] dataCenter = new hourlyData[24];
    public dailyData[] theData = new dailyData[31];
    public System.DateTime currentTime;
    int currentHour;


    //================================interact code===========================================//

    // Start is called before the first frame update
    void Start()
    {
        HD = Controller.GetComponent<HouseDriver>();
        folder = "/Log/" + device;
        FileName = folder + "/" + device + ".txt";

        if (SystemInfo.deviceType == DeviceType.Desktop)
            path = Application.dataPath;
        else if (SystemInfo.deviceType == DeviceType.Handheld)
            path = Application.persistentDataPath;
        //path = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("/Android"));

        if (!Directory.Exists(path + folder))
        {
            Directory.CreateDirectory(path + folder);
        }
        
    }

    private void OnMouseDown()
    {
        //toggle first, but set toggled state later once confirm from server;
        //if now false, set on, else set off
        Toast.ShowMessage("Server Processing...", Toast.Position.bottom, Toast.Time.twoSecond, this);
        if(HD.getHouseManager().getRoomManager()[roomNumebr].getDeviceManager()[deviceNumber].getStatus())
            HD.agent.sendState("OFF", HD.getHouseManager().getRoomManager()[roomNumebr].getDeviceManager()[deviceNumber].getDeviceNum().ToString());
        else
            HD.agent.sendState("ON", HD.getHouseManager().getRoomManager()[roomNumebr].getDeviceManager()[deviceNumber].getDeviceNum().ToString());

        setLight();
    }

    private async void setLight()
    {
        await wait(4000);
        if (HD.agent.getResponseCode() == 503)
        {
            Toast.ShowMessage("server is busy, try again..", Toast.Position.bottom, Toast.Time.twoSecond, this);
        }
        else if (HD.agent.getResponseCode() == 203)
        {
            Toast.ShowMessage("No Permission Granted", Toast.Position.bottom, Toast.Time.twoSecond, this);
        }
        else
        {
            //only toggle if server successfully update the state; else, dont do anything
            HD.getHouseManager().getRoomManager()[roomNumebr].getDeviceManager()[deviceNumber].toggleStatus();
            HD.getHouseManager().getRoomManager()[roomNumebr].getDeviceManager()[deviceNumber].getDevice().SetActive(
                HD.getHouseManager().getRoomManager()[roomNumebr].getDeviceManager()[deviceNumber].getStatus());
            Toast.ShowMessage("updated...", Toast.Position.bottom, Toast.Time.oneSecond, this);
			
			updateFile();
        }
    }

    private Task wait(int time)
    {
        return Task.Factory.StartNew(() =>
        {
            Thread.Sleep(time);
        });

    }

    //================================Update File code=========================================================================================================//

    void save(string FileName, string data)
    {
        File.WriteAllText(path + FileName, data);
    }

    string load(string readFile)
    {
        Debug.Log(readFile);
        if (File.Exists(path + readFile))
        {
            string getData = File.ReadAllText(path + readFile);
            return getData;
        }
        else
        {
            Debug.Log("get triggered??");
            return "";
        }
    }

    private void updateFile()
    {
        string newData = "";
        string getTemplate = "";

        //load first
        //check if file exist
        //Debug.Log(Application.streamingAssetsPath + logTemplate);
        if (!File.Exists(path + FileName))
        {
            TextAsset text = Resources.Load<TextAsset>(logTemplate);
            save(FileName, text.text);
        }

        data = load(FileName);

        splitData_hour = data.Split(new[] { SPLIT_HOUR }, System.StringSplitOptions.None);

        //put/split all the data into hourlyData class object
        for (int i = 0; i < splitData_hour.Length; i++)
        {
            //print(i);
            splitData_day = splitData_hour[i].Split(new[] { SPLIT_DAY_DATA }, System.StringSplitOptions.None);
            splitData_time = splitData_day[1].Split(new[] { SPLIT_HOUR_DATA }, System.StringSplitOptions.None);
            splitData_ontime = splitData_time[1].Split(new[] { SPLIT_ONTIME_DATA }, System.StringSplitOptions.None);
            splitData_offtime = splitData_ontime[1].Split(new[] { SPLIT_OFFTIME_DATA }, System.StringSplitOptions.None);
            splitData_state = splitData_offtime[1].Split(new[] { SPLIT_STATE_DATA }, System.StringSplitOptions.None);
            splitData_logtime = splitData_state[1];

            dataCenter[i] = new hourlyData();
            dataCenter[i].day = int.Parse(splitData_day[0]);
            dataCenter[i].hour = int.Parse(splitData_time[0]);
            dataCenter[i].ontime = int.Parse(splitData_ontime[0]);
            dataCenter[i].offtime = int.Parse(splitData_offtime[0]);
            dataCenter[i].state = (splitData_state[0].Equals("True")) ? true : false;
            if (!splitData_logtime.Equals(""))
                dataCenter[i].logtime = System.DateTime.Parse(splitData_logtime);
        }

        Debug.Log("load done");
        //print(dataCenter[4].ToStrings());

        //update the data
        currentTime = System.DateTime.Parse(System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
        currentHour = currentTime.Hour;


        //check if is same day
        //if different
        if (currentTime.Day != dataCenter[0].day)
        {
            //determine when is the last toggle time
            for (int i = dataCenter.Length - 1; i >= 0; i--)
            {
                //check if is empty log time
                if (dataCenter[i].logtime.Year != 1)
                {
                    //check if is on or off
                    if (HD.getHouseManager().getRoomManager()[roomNumebr].getDeviceManager()[deviceNumber].getStatus())          //set to device's status
                    {
                        //update the on off time count
                        Debug.Log(((60 - dataCenter[i].logtime.Minute) * 60 - dataCenter[i].logtime.Second));
                        dataCenter[i].offtime += ((60 - dataCenter[i].logtime.Minute) * 60 - dataCenter[i].logtime.Second);
                        dataCenter[i].state = HD.getHouseManager().getRoomManager()[roomNumebr].getDeviceManager()[deviceNumber].getStatus();          //set to device's status
                    }
                    else
                    {
                        //update the on off time count
                        Debug.Log(((60 - dataCenter[i].logtime.Minute) * 60 - dataCenter[i].logtime.Second));
                        dataCenter[i].ontime += ((60 - dataCenter[i].logtime.Minute) * 60 - dataCenter[i].logtime.Second);
                        dataCenter[i].state = HD.getHouseManager().getRoomManager()[roomNumebr].getDeviceManager()[deviceNumber].getStatus();          //set to device's status
                    }
                    break;
                }
            }

            //calculate the statistic
            if (dataCenter[0].day != 0)
                updateMasterTable(dataCenter, currentTime);

            //create new day stat
            //getTemplate = load(logTemplate);
            //getTemplate = File.ReadAllText(Application.streamingAssetsPath + logTemplate);
            getTemplate = Resources.Load<TextAsset>(logTemplate).text;

            splitData_hour = getTemplate.Split(new[] { SPLIT_HOUR }, System.StringSplitOptions.None);

            //put/split all the data into hourlyData class object
            for (int i = 0; i < splitData_hour.Length; i++)
            {
                splitData_day = splitData_hour[i].Split(new[] { SPLIT_DAY_DATA }, System.StringSplitOptions.None);
                splitData_time = splitData_day[1].Split(new[] { SPLIT_HOUR_DATA }, System.StringSplitOptions.None);
                splitData_ontime = splitData_time[1].Split(new[] { SPLIT_ONTIME_DATA }, System.StringSplitOptions.None);
                splitData_offtime = splitData_ontime[1].Split(new[] { SPLIT_OFFTIME_DATA }, System.StringSplitOptions.None);
                splitData_state = splitData_offtime[1].Split(new[] { SPLIT_STATE_DATA }, System.StringSplitOptions.None);
                splitData_logtime = splitData_state[1];

                dataCenter[i] = new hourlyData();
                dataCenter[i].day = currentTime.Day;
                dataCenter[i].hour = int.Parse(splitData_time[0]);
                dataCenter[i].ontime = int.Parse(splitData_ontime[0]);
                dataCenter[i].offtime = int.Parse(splitData_offtime[0]);
                dataCenter[i].state = (splitData_state[0].Equals("True")) ? true : false;
                if (!splitData_logtime.Equals(""))
                    dataCenter[i].logtime = System.DateTime.Parse(splitData_logtime);
            }
            dataCenter[currentHour].logtime = currentTime;

            for (int i = 0; i < dataCenter.Length; i++)
            {
                newData += currentTime.Day + SPLIT_DAY_DATA;
                newData += dataCenter[i].hour + SPLIT_HOUR_DATA;
                newData += dataCenter[i].ontime + SPLIT_ONTIME_DATA;
                newData += dataCenter[i].offtime + SPLIT_OFFTIME_DATA;
                newData += dataCenter[i].state + SPLIT_STATE_DATA;          //set to device's status
                newData += dataCenter[i].logtime;
                if (i + 1 != dataCenter.Length)
                    newData += SPLIT_HOUR;
                newData += "\n";
            }

            Debug.Log("file updated");
            Debug.Log(newData);
            save(FileName, newData);
        }
        // if same day
        else
        {
            //loop 24 times
            for (int i = currentHour; i >= 0; i--)
            {
                //check if is empty log time
                if (dataCenter[i].logtime.Year != 1)
                {
                    //check if is on or off
                    if (HD.getHouseManager().getRoomManager()[roomNumebr].getDeviceManager()[deviceNumber].getStatus())          //set to device's status
                    {
                        //check if is the same hour
                        if (i == currentHour)
                        {
                            Debug.Log(((currentTime - dataCenter[i].logtime).TotalSeconds).ToString());
                            dataCenter[i].offtime += int.Parse(((currentTime - dataCenter[i].logtime).TotalSeconds).ToString());
                            dataCenter[i].logtime = currentTime;
                        }
                        else
                        {
                            Debug.Log(((60 - dataCenter[i].logtime.Minute) * 60 - dataCenter[i].logtime.Second));
                            dataCenter[i].offtime += ((60 - dataCenter[i].logtime.Minute) * 60 - dataCenter[i].logtime.Second);
                            dataCenter[currentHour].logtime = currentTime;
                        }
                        dataCenter[i].state = HD.getHouseManager().getRoomManager()[roomNumebr].getDeviceManager()[deviceNumber].getStatus();          //set to device's status
                    }
                    else
                    {
                        if (i == currentHour)
                        {
                            Debug.Log(((currentTime - dataCenter[i].logtime).TotalSeconds).ToString());
                            dataCenter[i].ontime += int.Parse(((currentTime - dataCenter[i].logtime).TotalSeconds).ToString());
                            dataCenter[i].logtime = currentTime;
                        }
                        else
                        {
                            Debug.Log(((60 - dataCenter[i].logtime.Minute) * 60 - dataCenter[i].logtime.Second));
                            dataCenter[i].ontime += ((60 - dataCenter[i].logtime.Minute) * 60 - dataCenter[i].logtime.Second);
                            dataCenter[currentHour].logtime = currentTime;
                        }
                        dataCenter[i].state = HD.getHouseManager().getRoomManager()[roomNumebr].getDeviceManager()[deviceNumber].getStatus();          //set to device's status
                    }
                    break;
                }
            }

            Debug.Log("data updated");

            //write back the data

            for (int i = 0; i < dataCenter.Length; i++)
            {
                newData += currentTime.Day + SPLIT_DAY_DATA;
                newData += dataCenter[i].hour + SPLIT_HOUR_DATA;
                newData += dataCenter[i].ontime + SPLIT_ONTIME_DATA;
                newData += dataCenter[i].offtime + SPLIT_OFFTIME_DATA;
                newData += dataCenter[i].state + SPLIT_STATE_DATA;
                newData += dataCenter[i].logtime;
                if (i + 1 != dataCenter.Length)
                    newData += SPLIT_HOUR;
                newData += "\n";
            }

            Debug.Log("file updated");
            Debug.Log(newData);
            save(FileName, newData);
        }

    }

    public void updateMasterTable(hourlyData[] data, System.DateTime currentTime)
    {
        string content;
        string[] split_day_array;
        string[] split_hour_array;

        //check if file exist
        if (!File.Exists(path + folder + masterTable))
        {
            //File.Copy(Application.streamingAssetsPath + masterTemplate, path + folder + masterTable);

            TextAsset getText = Resources.Load<TextAsset>(masterTemplate);
            save(folder+masterTable, getText.text);
        }

        //load the data
        content = load(folder + masterTable);

        //split the array into 31 days
        split_day_array = content.Split(new[] { SPLIT_DAY }, System.StringSplitOptions.None);

        for (int i = 0; i < split_day_array.Length; i++)
        {
            //create 31 objects
            theData[i] = new dailyData();

            //save the data into the object
            split_hour_array = split_day_array[i].Split(new[] { SPLIT_HOUR_DATA }, System.StringSplitOptions.None);
            theData[i].stringTointArray(split_hour_array);
        }

        //set the value if to 1 and -1 as in on and off if more than threshold
        for (int j = 0; j < data.Length; j++)
        {
            if (data[j].ontime >= theData[data[j].day - 1].threshold)
                theData[data[j].day - 1].hours[j] = 1;
            //print("Hour: " + i + " consider as ON!");
            else if (data[j].offtime >= theData[data[j].day - 1].threshold)
                theData[data[j].day - 1].hours[j] = -1;
            //print("Hour: " + i + " consider as OFF!");
        }

        //update the master table file
        string newData = "";

        for (int i = 0; i < theData.Length; i++)
        {

            for (int j = 0; j < theData[i].hours.Length; j++)
            {
                newData += theData[i].hours[j];
                if (j + 1 != theData[i].hours.Length)
                    newData += SPLIT_HOUR_DATA;
            }
            if (i + 1 != theData.Length)
                newData += SPLIT_DAY;
            newData += "\n";
        }

        save(folder + masterTable, newData);


        //call the recommendation system.
        //if full month;
        if (System.DateTime.Now.Day == 1)
        {
            recommendationMonth(theData);
            recommendationDay(theData, currentTime);
        }
        File.Delete(path + folder + masterTable);
        //File.Copy(Application.streamingAssetsPath + masterTemplate, path + folder + masterTable);
        TextAsset text = Resources.Load<TextAsset>(masterTemplate);
        save(folder+masterTable, text.text);
    }

    //recommendation per hour for entire month
    public void recommendationMonth(dailyData[] day)
    {
        int max;
        int min;
        double output;
        double threshold = 0.8;
        string content = "";
        dailyData result = new dailyData();

        //file preparation
        if (!File.Exists(path + recommendMonth))
        {
            File.Create(path + folder + recommendMonth).Close();
        }

        for (int i = 0; i < day.Length; i++)
        {
            for (int j = 0; j < day[i].hours.Length; j++)
            {
                result.hours[j] += day[i].hours[j];
            }
        }

        max = getMax(result.hours);
        min = getMin(result.hours);

        for (int i = 0; i < result.hours.Length; i++)
        {
            if (result.hours[i] >= 0)
            {
                output = (double)result.hours[i] / max;
                if (output >= threshold)
                {
                    Debug.Log("This device had been frequently turn ON at hour " + i);
                    //set selection choice here
                    content += "True" + SPLIT_STATE_DATA;
                    content += i + SPLIT_NEXT;
                }
            }
            else
            {
                output = (double)result.hours[i] / min;
                if (output >= threshold)
                {
                    Debug.Log("This device had been frequently turn OFF at hour " + i);
                    //set selection choice here
                    content += "False" + SPLIT_STATE_DATA;
                    content += i + SPLIT_NEXT;
                }

            }
        }
        save(folder + recommendMonth, content);

    }

    public void recommendationDay(dailyData[] data, System.DateTime currentTime)
    {
        int max;
        int min;
        int DAYS = 7;
        double output;
        double threshold = 0.8;
        string content = "";
        dailyData[] result = new dailyData[7];

        //file preparation
        if (!File.Exists(path + recommendDay))
        {
            File.Create(path + folder + recommendDay).Close();
        }

        for (int i = 0; i < DAYS; i++)
        {
            result[i] = new dailyData();
            result[i].day = new System.DateTime(currentTime.AddDays(-1).Year, currentTime.AddDays(-1).Month, i + 1).DayOfWeek;
            Debug.Log(result[i].day);
        }

        //recognize per hour by week
        for (int i = 0; i < data.Length; i++)
        {
            for (int j = 0; j < data[i].hours.Length; j++)
            {
                result[i % DAYS].hours[j] += data[i].hours[j];
            }
        }

        for (int i = 0; i < result.Length; i++)
        {
            max = getMax(result[i].hours);
            min = getMin(result[i].hours);

            for (int j = 0; j < result[i].hours.Length; j++)
            {
                if (result[i].hours[j] >= 0)
                {
                    output = (double)result[i].hours[j] / max;
                    if (output >= threshold)
                    {
                        Debug.Log("On " + result[i].day + ", Hour " + j + " had been frequently Turn ON.");
                        //set selection choice here
                        content += result[i].day + SPLIT_DAY_DATA;
                        content += j + SPLIT_HOUR_DATA;
                        content += "True" + SPLIT_NEXT;
                    }
                }
                else
                {
                    output = (double)result[i].hours[j] / min;
                    if (output >= threshold)
                    {
                        Debug.Log("On " + result[i].day + ", Hour " + j + " had been frequently Turn OFF.");
                        //set selection choice here
                        content += result[i].day + SPLIT_DAY_DATA;
                        content += j + SPLIT_HOUR_DATA;
                        content += "False" + SPLIT_NEXT;
                    }
                }

            }

            save(folder + recommendDay, content);

        }

    }

    public int getMax(int[] array)
    {
        int max = array[0];

        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > max)
                max = array[i];
        }

        return max;
    }

    public int getMin(int[] array)
    {
        int min = array[0];

        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] < min)
                min = array[i];
        }

        return min;
    }

    public class hourlyData
    {
        public int day;
        public int hour;
        public int ontime;
        public int offtime;
        public bool state;
        public System.DateTime logtime;

    }

    public class dailyData
    {
        public int date;
        public System.DayOfWeek day;
        public int[] hours = new int[24];
        public int threshold = 2880; //3600*0.8
        //public int[] hourly_freq = new int[24];

        public void stringTointArray(string[] getData)
        {
            for (int i = 0; i < getData.Length; i++)
            {
                //print(getData[i]);
                hours[i] = int.Parse(getData[i]);
            }
        }
    }
}
