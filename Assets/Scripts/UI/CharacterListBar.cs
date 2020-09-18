using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterListBar : MonoBehaviour
{
    public Image[] characters;

    public void SetSelectCharacter(int index)
    {
        for(int i = 0; i < characters.Length; ++i)
        {
            if(i == index)
            {
                characters[i].color = Color.cyan;
            }
            else
            {
                characters[i].color = Color.white;
            }
        }
    }
}
