using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class TimeUI : MonoBehaviour
{
    public RectTransform DayNightImage;
    public RectTransform ClockParent;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;
    public Image SeasonImage;

    public Sprite[] seasonImages;

    List<GameObject> clockBlockes = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < ClockParent.childCount; i++)
        {
            clockBlockes.Add(ClockParent.GetChild(i).gameObject);
            ClockParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        EventHandler.UpdataTime += OnMinuteChange;
        EventHandler.UpdataDate += OnDateChange;
    }
    private void OnDisable()
    {
        EventHandler.UpdataTime -= OnMinuteChange;
        EventHandler.UpdataDate -= OnDateChange;
    }


    public void OnMinuteChange(int minute, int hour,int day,Season season)
    {
        RotateTimeImage(hour);
        SwitchClockBlockes(hour);
        ChangeTimeUI(hour, minute);

    }

    private void RotateTimeImage(int hour)
    {
        DayNightImage.DORotate(new Vector3(1, 1, hour * 15), Settings.RotateDuration);

    }

    private void SwitchClockBlockes(int hour)
    {
        int Index = hour / 4;
        if (Index == 0)
        {
            foreach (GameObject g in clockBlockes)
            {
                g.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < Index; i++)
            {
                clockBlockes[i].SetActive(true);
            }
        }
    }

    private void ChangeTimeUI(int hour,int minute)
    {
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
    }

    public void OnDateChange(int year,int mouth,int day,Season season)
    {
        dateText.text = year.ToString() + "Äê" + mouth.ToString("00") + "ÔÂ" + day.ToString("00") + "ÈÕ";

        int seasonIndex = (int)season;
        SeasonImage.sprite = seasonImages[seasonIndex];

    }
}
