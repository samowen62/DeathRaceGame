using UnityEngine;
using System.Collections;

public static class AppConfig {

    public static int groundMask = 1 << 8;
    public static int wallMask = 1 << 11;

    //delete??
    public static int trackPointMask = 1 << 12;

    public static float hoverHeight = 1.8f;
}
