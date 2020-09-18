using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    public Sprite[] spriteArr = new Sprite[0];
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    private int spriteIndex;

    public void SetSprite(int spriteIndex)
    {
        this.spriteIndex = spriteIndex;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        spriteRenderer.sprite = spriteArr[spriteIndex];
    }

    public int GetSpriteIndex()
    {
        return spriteIndex;
    }
}
