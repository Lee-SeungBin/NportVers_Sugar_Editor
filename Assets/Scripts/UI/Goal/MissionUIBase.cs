using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionUIBase : MonoBehaviour
{
    public Image image;
    public Text text;
    protected MissionManager missionManager;
    

    public void SetData(Sprite sprite, string value)
    {
        image.sprite = sprite;
        image.SetNativeSize();

        text.text = value;
    }


}
