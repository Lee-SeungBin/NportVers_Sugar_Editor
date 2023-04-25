using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum OBSTACLE_TYPE
    {
        JELLY = 0,
        RAIL = 1,
        FROG_SOUP = 2,
        BOX = 3,
        VINE = 4,
    }

    public enum NEXT_MAP_TYPE
    {
        CREAM = 0,
        STRAWBERRY = 1,
        CHOCOLATE = 2,
        EGG = 3,
        BREAD = 4,
        ICE = 5,
        JELLY = 6,
        FROG_SOUP = 7
    }
    public enum CHARACTOR_TASTY_TYPE
    {
        NONE = -1,
        CR,
        ST,
        CH,
        EG,
        BR,
        SP
    }
    public enum CHARCTER_TYPE
    {
        NONE,
        MOVE,
        JUMP,
        WHISTLE,
        CCWMON,
        CWMON,
        BOMBMON,
        TURN_WHBOMBMON,
        WHBOMBMON
    }
    public enum MAP_SELECT_MODE
    {
        MAP_MOVE,
        SELECT_TILESET,
        MONSTER_SET,
        RAIL_SET,
        SPECIAL_SET
    }
    public enum RAIL_TYPE
    {
        ROTATION = 0,
        STRIGHT = 1
    }
    public enum MISSION_TYPE
    {
        CREAM = 0,
        STRAWBERRY = 1,
        CHOCOLATE = 2,
        EGG = 3,
        BREAD = 4,
        ICE = 5,
        JELLY = 6,
        FROG_SOUP = 7
    }
    public enum SPECIAL_TYPE
    {
        NONE = -1,
        JELLY = 0,
        FROG_SOUP = 1,
        BOX = 2,
        WOODEN_FENCE = 3,
        VINE = 4,
    }
}
