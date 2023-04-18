using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessagePopup : MonoBehaviour
{
    [SerializeField]
    private Text text;

    public void SetMessage(string message)
    {
        text.text = message;

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
