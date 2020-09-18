using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleMissionUI : MissionUIBase
{
    private void Awake()
    {
        missionManager = GameObject.Find("MissionInfo").GetComponent<MissionManager>();
        gameObject.SetActive(false);
    }

    public void IsOn(Toggle toggle)
    {
        gameObject.SetActive(toggle.isOn);
    }

}
