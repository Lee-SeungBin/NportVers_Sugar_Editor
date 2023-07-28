using System;
using System.Collections.Generic;

public class RandomBox : Box
{
    public bool isflavone;
    //public List<bool> itemlist;
    //public List<int> percentage;
    public int boxpercent;

    [Serializable]
    public struct RandomData
    {
        public List<string> itemlist;
        public List<int> percentage;
    }
    public List<RandomData> DataSet;

    public void Init()
    {
        //itemlist = new List<bool>();
        //percentage = new List<int>();
    }

    public void Push(bool list, int per)
    {
        //itemlist.Add(list);
        //percentage.Add(per);
    }
}
