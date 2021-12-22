using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EventService : MonoBehaviour
{
    public static List<Event> events = new List<Event>();
    public static EventService eventServiceObject;

    public float cooldownBeforeSend = 5f;
    public string serverUrl = "http://testprojects.cs90176.tmweb.ru";
    public Text text;

    void Start()
    {
        EventService.eventServiceObject = this;
        LoadEvents();
        StartCoroutine(Cooldown(cooldownBeforeSend));
    }

    IEnumerator Cooldown(float x)
    {
        while (true)
        {
            if (EventService.events.Count > 0)
            {
                string data = "{ \"events\":[";
                for (int i = 0; i < EventService.events.Count; i++)
                {
                    Event ev = EventService.events[i];
                    if (EventService.events.Count > 1 && i < EventService.events.Count - 1)
                    {
                        data += JsonUtility.ToJson(ev) + ",";
                    }
                    else
                    {
                        data += JsonUtility.ToJson(ev);
                    }
                }
                data += "]}";
                StartCoroutine(Post(data));
                if (EventService.events.Count > 0)
                {
                    text.text = "";
                }
                EventService.events = new List<Event>();

                SaveEvents();
            }

            yield return new WaitForSeconds(x);
        }
    }

    IEnumerator Post(string bodyJsonString)
    {
        var request = new UnityWebRequest(serverUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        text.text += request.downloadHandler.text + "(Status Code: " + request.responseCode + ")" + "\n----------------------------";
        Debug.Log("Status Code: " + request.responseCode);
    }


    public void TrackEvent(string type, string data)
    {
        Event ev = new Event();
        ev.type = type;
        ev.data = data;

        EventService.events.Add(ev);
        EventService.eventServiceObject.SaveEvents();
    }

    public void SaveEvents()
    {
        SaveData saveData = new SaveData();
        saveData.events = EventService.events;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/data.save");
        bf.Serialize(file, saveData);
        file.Close();
    }

    public void LoadEvents()
    {
        string fileName = "/data.save";
        if (File.Exists(Application.persistentDataPath + fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
            SaveData save = (SaveData)bf.Deserialize(file);
            file.Close();

            EventService.events = save.events;
        }
    }
}
