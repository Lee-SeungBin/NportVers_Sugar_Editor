using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charactor : MonoBehaviour
{
    public int tileSetIndex;
    public int tileIndex;

    public Enums.CHARCTER_TYPE characterType;
    
    public GameObject star;
    private bool _isStar;
    public bool isStar
    {
        get
        {
            return _isStar;
        }

        set
        {
            _isStar = value;
            star.SetActive(value);
        }
    }

    public SpriteChanger ice;
    private int _iceStep;
    public int iceStep
    {
        get
        {
            return _iceStep;
        }

        set
        {
            _iceStep = value;
            if(_iceStep == 0)
            {
                ice.gameObject.SetActive(false);
            }
            else
            {
                ice.gameObject.SetActive(true);
                ice.SetSprite(_iceStep - 1);
            }
        }
    }


    private Vector2 widthDirection = new Vector2(1, 1);
    private Vector2 heightDirection = new Vector2(-1, 1);
    private bool _isHeightDirection;
    public bool isHeightDirection
    {
        get
        {
            return _isHeightDirection;
        }

        set
        {
            _isHeightDirection = value;
            if (_isHeightDirection)
            {
                transform.localScale = heightDirection;
            }
            else
            {
                transform.localScale = widthDirection;
            }
        }
    }

    [SerializeField]
    private GameObject particle;
    private bool _isUser;
    public bool isUser
    {
        get
        {
            return _isUser;
        }

        set
        {
            _isUser = value;
            particle.SetActive(value);

            if(value)
            {

            }
        }
    }




    private void Awake()
    {
        iceStep = 0;
        isStar = false;
        isUser = false;
    }
}
