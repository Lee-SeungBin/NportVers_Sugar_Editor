using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBoxPopup_Item : MonoBehaviour
{
    [Serializable]
    public struct RandomData
    {
        public List<string> itemlist;
        public List<InputField> percentage;
    }
    public List<RandomData> DataSet;

    public RandomBox selectBox;

    [SerializeField]
    private List<GameObject> types;

    [SerializeField]
    private Dropdown typeDropdown;

    [SerializeField]
    private Text percentInfo;
    public void Show(RandomBox box)
    {
        gameObject.SetActive(true);
        SetData(box);
    }

    public void Hide()
    {
        SaveData();
        gameObject.SetActive(false);
    }

    public void SetData(RandomBox box)
    {
        selectBox = box;
        List<RandomBox.RandomData> data = box.DataSet;
        for (int i = 0; i < data.Count; i++)
        {
            for (int j = 0; j < data[i].itemlist.Count; j++)
            {
                DataSet[i].itemlist[j] = data[i].itemlist[j];
            }
            for (int j = 0; j < data[i].percentage.Count; j++)
            {
                DataSet[i].percentage[j].text = data[i].percentage[j].ToString();
            }
        }
        OnChangePercentInfo();
    }
    private int CalculateTotalPercentage()
    {
        int totalPercentage = 0;

        foreach (RandomData data in DataSet)
        {
            foreach (InputField inputField in data.percentage)
            {
                int percentageValue;
                if (int.TryParse(inputField.text, out percentageValue))
                {
                    totalPercentage += percentageValue;
                }
            }
        }

        return totalPercentage;
    }
    public void SaveData()
    {
        if (selectBox == null)
            return;
        for (int i = 0; i < DataSet.Count; i++)
        {
            for (int j = 0; j < DataSet[i].itemlist.Count; j++)
            {
                selectBox.DataSet[i].itemlist[j] = DataSet[i].itemlist[j];
            }
            for (int j = 0; j < DataSet[i].percentage.Count; j++)
            {
                selectBox.DataSet[i].percentage[j] = int.Parse(DataSet[i].percentage[j].text);
            }
        }
        selectBox = null;
    }
    public void OnChangePercentInfo()
    {
        percentInfo.text = CalculateTotalPercentage().ToString();
        if (CalculateTotalPercentage() > 100)
            percentInfo.color = new Color(255, 0, 0);
        else
            percentInfo.color = new Color(0, 0, 0);
        if (selectBox != null)
            selectBox.boxpercent = CalculateTotalPercentage();
    }
    public void OnChangeMonsterType()
    {
        for (int i = 0; i < types.Count; ++i)
        {
            types[i].gameObject.SetActive(false);
        }

        types[typeDropdown.value].gameObject.SetActive(true);
    }

}
