using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeIsPause : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.timeControlEvent += timeIsPause;
    }

    /// <summary>
    /// 组件通过调用timeControlEvent来调用timeControlEvent事件之后调用timeIsPause
    /// </summary>
    /// <param name="isPause">isPause就是TimeManager.gameClockPause</param>
    public void timeIsPause(bool isPause)
    {
        TimeManager.gameClockPause = !isPause;
    }

    // 通过UI的Button组件调用来控制时间
    public void timeControlEvent()
    {
        EventHandler.CallUpTimeControlEvent(TimeManager.gameClockPause);
    }
}
