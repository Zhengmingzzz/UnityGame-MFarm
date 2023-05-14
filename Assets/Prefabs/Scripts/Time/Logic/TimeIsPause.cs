using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeIsPause : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.timeAccelerate += Accelerate;
    }

    public void Accelerate(bool isAccelerate)
    {
        TimeManager.isAccelerate = !isAccelerate;
    }
    public void timeControl()
    {
        EventHandler.CallUptimeAccelerate(TimeManager.isAccelerate) ;
    }
}
