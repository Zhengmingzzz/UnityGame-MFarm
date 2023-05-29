using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeIsPause : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.timeControl += timeIsPause;
    }

    public void timeIsPause(bool isPause)
    {
        TimeManager.gameClockPause = !isPause;
    }

    public void timeControl()
    {
        EventHandler.CallUptimeControl(TimeManager.gameClockPause) ;
    }
}
