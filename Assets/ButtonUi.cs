using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUi : MonoBehaviour
{
    public string type = "buttonPressed";
    public Text text;
    public void AddEvent()
    {
        EventService.eventServiceObject.TrackEvent(type, text.text);
    }
}
