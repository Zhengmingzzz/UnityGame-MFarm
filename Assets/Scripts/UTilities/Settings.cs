using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public const float itemFadeDuration = 0.35f;
    public const float targetAlpah = 0.45f;

    //时间相关参数阈值
    public const float timeAccelerate = 60f;
    public const float secondThreshold = 0.01f;
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 10;
    public const int mouthHold = 3;
    public const int seasonHold = 10;

    public const float RotateDuration = 0.35f;

    /// <summary>
    /// 限制切换场景时加载的时间
    /// </summary>
    public const float loadingFadeDuration = 0.8f;

    public const int ReapItemSpawCount = 3;

    public const float ItemShakeTime = 1f;

    public const float baseCellSize = 1f;
    public const float baseCellDiagonalSize = 1.4f;

    public const float pixelSize = 0.025f;

    public const float NPCWatiEventTime = 10f;

    public const int maxGridSize = 9999;
}
