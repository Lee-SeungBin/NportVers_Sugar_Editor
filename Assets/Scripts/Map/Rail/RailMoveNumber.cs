using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailMoveNumber : MonoBehaviour
{
    public Text text;

    public void SetNumber(int number)
    {
        text.text = number.ToString();
    }

    public void Select()
    {
        text.color = Color.red;
    }

    public void Unselect()
    {
        text.color = Color.gray;
    }
}
