using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SandWichChangePopup : MonoBehaviour
{

    public Dropdown TasteLayerDropdown;

    public void SetData(Box box)
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        Dropdown.OptionData option;

        int len = box.boxTier.Count + 1;
        for (int i = 1; i < len; ++i)
        {
            option = new Dropdown.OptionData();
            option.text = i.ToString();
            options.Add(option);
        }

        TasteLayerDropdown.options = options;

        if (box.boxLayer == 0)
            MapManager.Instance.specialMode.ActiveAlphaLayerOfBox(box, true);
    }

    public void Show(Box box)
    {
        gameObject.SetActive(true);
        SetData(box);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnChangeTaste()
    {
        MapManager.Instance.ChangeTasteOfBox(TasteLayerDropdown.value);
    }
    
    public void OnDeleteTaste()
    {
        MapManager.Instance.DeleteTasteLayerOfBox(TasteLayerDropdown.value);
        TasteLayerDropdown.value = 0;
    }
}
