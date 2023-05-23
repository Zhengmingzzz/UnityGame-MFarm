using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private int second, minute, hour,day, mouth, year;
    private Season gameSeason = Season.春天;
    private int seasonInMouth = 3;

    public static bool gameClockPause;
    private float tickTime = 0f;

    public static bool isAccelerate = false;

    private void newGameTime()
    {
        second = 0;
        minute = 0;
        hour = 20;
        day = 1;
        mouth = 1;
        year = 2022;
        gameSeason = Season.春天;
    }

    private void Awake()
    {
        newGameTime();
    }

    private void Start()
    {
        EventHandler.CallUpUpdataDate(year, mouth, day, gameSeason);
        EventHandler.CallUpUpdataTimeUI(minute, hour);
    }

    private void FixedUpdate()
    {
        if (!gameClockPause)
        {
            float timeThreshold = Settings.secondThreshold;
            tickTime += Time.deltaTime;
            if (tickTime > timeThreshold)
                UpdataTime();
        }
    }

    public void UpdataTime()
    {
        if (isAccelerate)
        {
            second += (int)Settings.timeAccelerate;
        }
        else
        {
            second++;
        }
        if (second > Settings.secondHold)
        {
            second = 0;
            minute++;
            if (minute > Settings.minuteHold)
            {
                minute = 0;
                hour++;
                if (hour > Settings.hourHold)
                {
                    hour = 0;
                    day++;
                    if (day > Settings.dayHold)
                    {
                        day = 1;
                        mouth++;
                        if (mouth > Settings.mouthHold)
                        {
                            mouth = 1;
                            seasonInMouth--;
                            if (seasonInMouth == 0)
                            {
                                seasonInMouth = 3;
                                int seasonIndex = (int)gameSeason;
                                seasonIndex++;
                                if (seasonInMouth > Settings.seasonHold)
                                {
                                    seasonIndex = 0;
                                    year++;
                                }
                                gameSeason = (Season)seasonIndex;
                            }
                        }
                    }
                    EventHandler.CallUpUpdataGameDayEvent(day, gameSeason);
                }
                //小时更新
                EventHandler.CallUpUpdataDate(year, mouth, day, gameSeason);

            }

        }
                //分钟更新
                EventHandler.CallUpUpdataTimeUI(minute, hour);
    }
}
