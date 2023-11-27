using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Action<int> OnBSplineParameterChanged, OnScanlineParameterChanged;
    public static Action OnRenderCube, OnSpawnCube, OnClearScreen, OnClearSelectedPixels;
    public static Action<bool, bool, bool, bool, bool, int, int, int> OnCubeValues;

    public static void SendBSplineParameterChanged(int param)
    {
        if (OnBSplineParameterChanged != null)
            OnBSplineParameterChanged.Invoke(param);
    }

    public static void SendScanlineParameterChanged(int param)
    {
        if (OnScanlineParameterChanged != null)
            OnScanlineParameterChanged.Invoke(param);
    }

    public static void SendRenderCube()
    {
        if (OnRenderCube != null)
            OnRenderCube.Invoke();
    }

    public static void SendSpawnCube()
    {
        if (OnSpawnCube != null)
            OnSpawnCube.Invoke();
    }

    public static void SendClearScreen()
    {
        if (OnClearScreen != null)
            OnClearScreen.Invoke();
    }

    public static void SendClearSelectedPixels()
    {
        if (OnClearSelectedPixels != null)
            OnClearSelectedPixels.Invoke();
    }

    public static void SendCubeValues(bool moving, bool rotating, bool scaling, bool perspective, bool display, int x, int y, int z)
    {
        if (OnCubeValues != null)
            OnCubeValues.Invoke(moving, rotating, scaling, perspective, display, x, y, z);
    }
}
