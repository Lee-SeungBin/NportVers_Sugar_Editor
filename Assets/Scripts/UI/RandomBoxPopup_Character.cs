//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class RandomBoxPopup_Character : MonoBehaviour
//{
//    [Serializable]
//    public struct RandomData
//    {
//        public List<Toggle> itemlist;
//        public List<InputField> percentage;
//    }
//    public List<RandomData> DataSet;

//    public RandomBox selectBox;

//    public void Show(RandomBox box)
//    {
//        gameObject.SetActive(true);
//        SetData(box);
//    }

//    public void Hide()
//    {
//        SaveData();
//        gameObject.SetActive(false);
//    }

//    public void SetData(RandomBox box)
//    {
//        List<RandomBox.RandomData> data = box.DataSet;

//        for (int i = 0; i < data.Count; i++)
//        {
//            for (int j = 0; j < data[i].itemlist.Count; j++)
//            {
//                DataSet[i].itemlist[j].isOn = data[i].itemlist[j];
//            }
//            for (int j = 0; j < data[i].percentage.Count; j++)
//            {
//                DataSet[i].percentage[j].text = data[i].percentage[j].ToString();
//            }
//        }
//        selectBox = box;
//    }

//    public void SaveData()
//    {
//        if (selectBox == null)
//            return;
//        for (int i = 0; i < DataSet.Count; i++)
//        {
//            for (int j = 0; j < DataSet[i].itemlist.Count; j++)
//            {
//                selectBox.DataSet[i].itemlist[j] = DataSet[i].itemlist[j].isOn;
//            }
//            for (int j = 0; j < DataSet[i].percentage.Count; j++)
//            {
//                selectBox.DataSet[i].percentage[j] = int.Parse(DataSet[i].percentage[j].text);
//            }
//        }
//        selectBox = null;
//    }
//}
