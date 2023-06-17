using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScheduleDetails: IComparable<ScheduleDetails>
{
    public int hour, minute, day;
    public Season season;
    public int priority;
    public string targetScene;
    public Vector3Int targetGridPos;
    public AnimationClip targetAnimation;
    public bool isInteractive;
    public int Time => (hour * 100) + minute;


    public ScheduleDetails(int hour, int minute, int day, Season season, int priority, string targetScene, Vector3Int targetGridPos, AnimationClip targetAnimation, bool isInteractive)
    {
        this.hour = hour;
        this.minute = minute;
        this.day = day;
        this.season = season;
        this.priority = priority;
        this.targetScene = targetScene;
        this.targetGridPos = targetGridPos;
        this.targetAnimation = targetAnimation;
        this.isInteractive = isInteractive;
    }

    public int CompareTo(ScheduleDetails other)
    {
        if (this.Time == other.Time)
        {
            return this.Time > other.Time ? 1 : -1;
        }
        return this.Time > other.Time ? 1 : -1;
    }
}
