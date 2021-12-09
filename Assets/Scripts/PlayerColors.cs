using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerColors
{
    static readonly string RED_MATERIAL = "StarSparrow_Red";
    static readonly string RED_ICON = "red";

    static readonly string BLUE_MATERIAL = "StarSparrow_Blue";    
    static readonly string BLUE_ICON = "blue";

    static readonly string GREEN_MATERIAL = "StarSparrow_Green";
    static readonly string GREEN_ICON = "green";

    static readonly string GREY_MATERIAL = "StarSparrow_Grey";
    static readonly string GREY_ICON = "grey";

    static readonly string PURPLE_MATERIAL = "StarSparrow_Purple";
    static readonly string PURPLE_ICON = "purple";

    static readonly string WHITE_MATERIAL = "StarSparrow_White";
    static readonly string WHITE_ICON = "white";

    public static PlayerColor Red = new PlayerColor(RED_MATERIAL, RED_ICON);
    public static PlayerColor Blue = new PlayerColor(BLUE_MATERIAL, BLUE_ICON);
    public static PlayerColor Green = new PlayerColor(GREEN_MATERIAL, GREEN_ICON);
    public static PlayerColor Grey = new PlayerColor(GREY_MATERIAL, GREY_ICON);
    public static PlayerColor Purple = new PlayerColor(PURPLE_MATERIAL, PURPLE_ICON);
    public static PlayerColor White = new PlayerColor(WHITE_MATERIAL, WHITE_ICON);

    public static PlayerColor GetPlayerColor(string color) {
        switch (color) {
            case "red":
                return Red;

            case "blue":
                return Blue;

            default:
                break;
        }
        
        return Red;
    }
}
