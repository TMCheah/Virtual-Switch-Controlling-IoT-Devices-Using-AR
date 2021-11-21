using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Recommendation : MonoBehaviour
{
    string folder;
    public string FileName;
    private string path;
    string content = "";
    string[] splitData_next;
    string[] splitData_day;
    string[] splitData_time;
    string[] splitData_state;
    string splitData_hour;
    string splitData_status;
    private const string SPLIT_NEXT = "#next#";
    private const string SPLIT_DAY_DATA = "#day#";
    private const string SPLIT_HOUR_DATA = "#hh#";
    private const string SPLIT_STATE_DATA = "#state#";
    public GameObject theButton;
    //clone the template

    GameObject[] theRecogn = new GameObject[24];
    int countInstant = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnEnable()
    {
        GameObject Panel_template = transform.GetChild(1).gameObject;
        //Panel_template = transform.GetChild(1).gameObject;

        if (FileName.Equals("/recommendMonth.txt"))
        {
            //content = load(folder + FileName);
            string content = Resources.Load<TextAsset>("recommendMonth").text;

            //remove the last #next#
            content = content.Substring(0, content.LastIndexOf(SPLIT_NEXT));

            splitData_next = content.Split(new[] { SPLIT_NEXT }, System.StringSplitOptions.None);

            for (int i = 0; i < splitData_next.Length; i++)
            {
                splitData_state = splitData_next[i].Split(new[] { SPLIT_STATE_DATA }, System.StringSplitOptions.None);
                splitData_hour = splitData_state[1];

                theRecogn[countInstant] = Instantiate(Panel_template, transform);
                theRecogn[countInstant].transform.GetChild(0).GetComponent<Text>().text = "This device had been frequently turn " + (splitData_state[0].Equals("True") ? "ON" : "OFF") + " at " + splitData_hour + ".";
                countInstant++;
            }
        }
        else if (FileName.Equals("/recommendDay.txt"))
        {
            //content = load(folder + FileName);
            string content = Resources.Load<TextAsset>("recommendDay").text;

            Debug.Log(content);
            //remove the last #next#
            content = content.Substring(0, content.LastIndexOf(SPLIT_NEXT));
            
            splitData_next = content.Split(new[] { SPLIT_NEXT }, System.StringSplitOptions.None);
            Debug.Log(splitData_next.Length);
            for (int i = 0; i < splitData_next.Length; i++)
            {
                splitData_day = splitData_next[i].Split(new[] { SPLIT_DAY_DATA }, System.StringSplitOptions.None);
                splitData_time = splitData_day[1].Split(new[] { SPLIT_HOUR_DATA }, System.StringSplitOptions.None);
                splitData_status = splitData_time[1];

                //check if same day, as in monday's tueday's day
                Debug.Log(splitData_day[0]);
                if (System.DateTime.Now.DayOfWeek.ToString().Equals(splitData_day[0]))
                {
                    Debug.Log("true");
                    Panel_template.SetActive(true);
                    theRecogn[countInstant] = Instantiate(Panel_template, transform);
                    theRecogn[countInstant].transform.GetChild(0).GetComponent<Text>().text = "On " + splitData_day[0] + ", hour " + splitData_time[0] + " had been frequently turn " + (splitData_status.Equals("True") ? "ON" : "OFF") + ".";
                    countInstant++;
                }
            }
        }

        if (countInstant == 0)
            Panel_template.SetActive(false);
        else
            Destroy(Panel_template);
    }

    void OnDisable()
    {
        for (int i = 0; i < countInstant - 1; i++)
        {
            GameObject temp = theRecogn[i];
            theRecogn[i] = null;
            Destroy(temp);
        }
        countInstant = 0;
    }

    string load(string readFile)
    {
        //print(readFile);
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

    public void setFolder(string folder)
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
            path = Application.dataPath;
        else if (SystemInfo.deviceType == DeviceType.Handheld)
            path = Application.persistentDataPath;

        //print(path);

        this.folder = "/Log/" + folder;
        Debug.Log(path + this.folder + FileName);
        //read file
        //bypassing it

        //if (!File.Exists(path + this.folder + FileName))
        //{
        //    theButton.SetActive(false);
        //    return;
        //}
        //else
        //{
            theButton.SetActive(true);
        //}
    }

}
