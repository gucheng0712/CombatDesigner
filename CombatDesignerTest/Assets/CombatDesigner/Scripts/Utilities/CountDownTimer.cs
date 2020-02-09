using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownTimer{
    /// <summary>
    /// If loop the timer
    /// </summary>    public bool IsAutoCycle { get; private set; }

    /// <summary>
    /// If timer is stoped
    /// </summary>
    public bool IsStoped { get; private set; }

    /// <summary>
    /// Get current time
    /// </summary>
    public float CurrentTime { get { return UpdateCurrentTime(); } }

    /// <summary>
    /// Check if the timer is finished
    /// </summary>
    public bool IsTimeUp { get { return CurrentTime <= 0; } }

    /// <summary>
    /// The length of timer
    /// </summary>
    public float Duration { get; private set; }        

    /// <summary>
    /// previous update time
    /// </summary>
    private float lastTime;
    /// <summary>
    /// Frame count of last update countdown (avoid multiple update timings in one frame)
    /// </summary>    private int lastUpdateFrame;  

    /// <summary>
    /// current update time
    /// </summary>
    private float currentTime;

    // Constructor
    public CountDownTimer(float duration, bool autocycle = false, bool autoStart = true)
    {
        IsStoped = true;
        Duration = Mathf.Max(0f, duration);
        IsAutoCycle = autocycle;
        Reset(duration, !autoStart);
    }


    // Update current time
    private float UpdateCurrentTime()
    {
        if (IsStoped || lastUpdateFrame == Time.frameCount) // if pause or this frame has been updated, return directly
            return currentTime;
        if (currentTime <= 0) // if current time is less than or equal to 0 and returns directly, if the loop resets the time
        {
            if (IsAutoCycle)
                Reset(Duration, false);
            return currentTime;
        }
        currentTime -= Time.time - lastTime;
        UpdateLastTimeInfo();
        return currentTime;
    }

    // Update list time information
    private void UpdateLastTimeInfo()
    {
        lastTime = Time.time;
        lastUpdateFrame = Time.frameCount;
    }

    /// <summary>
    ///  Start tht timer.
    /// </summary>
    public void Start()
    {
        Reset(Duration, false);
    }

    /// <summary>
    /// Reset the timer.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="isStoped"></param>
    public void Reset(float duration, bool isStoped = false)
    {
        UpdateLastTimeInfo();
        Duration = Mathf.Max(0f, duration);
        currentTime = Duration;
        IsStoped = isStoped;
    }

    /// <summary>
    /// Pause the timer.
    /// </summary>
    public void Pause()
    {
        UpdateCurrentTime(); // Update current timer before stopping it
        IsStoped = true;
    }

    /// <summary>
    /// Continue the timer.
    /// </summary>
    public void Continue()
    {
        UpdateLastTimeInfo();   // Update last time info before continuing
        IsStoped = false;
    }

    /// <summary>
    /// End the Timer.
    /// </summary>
    public void End()
    {
        IsStoped = true;
        currentTime = 0f;
    }

    /// <summary>
    /// Get the current timer percentage.
    /// (1 - current time) / duration.
    /// </summary>
    /// <returns></returns>
    public float GetPercent()
    {
        UpdateCurrentTime();
        if (currentTime <= 0 || Duration <= 0)
            return 1f;
        return 1f - currentTime / Duration;
    }}
