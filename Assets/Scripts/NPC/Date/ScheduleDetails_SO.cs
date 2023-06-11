using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScheduleDetails_SO",menuName = "ScheduleDetails/ScheduleDetails_SO")]
public class ScheduleDetails_SO : ScriptableObject
{
    public List<ScheduleDetails> ScheduleList = new List<ScheduleDetails>();
}
