using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Action<int> OnBSplineParameterChanged;

    public static void SendBSplineParameterChanged(int param)
    {
        if (OnBSplineParameterChanged != null)
            OnBSplineParameterChanged.Invoke(param);
    }
}
