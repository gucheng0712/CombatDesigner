using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatDesigner;

/// <summary>
/// GameManager for Settings 
/// </summary>
public class GameManager_Settings : GameManager<GameManager_Settings>
{
    /// <summary>
    /// The default frame rate of the system
    /// </summary>
    public static int targetFrameRate = 60;

    /// <summary>
    /// A multiplier for the frame rate
    /// </summary>
    public float frameRangeMultiplier = 1;

    protected override void Awake()
    {
        base.Awake();
        QualitySettings.vSyncCount = 0; // not allow vSync
        Application.targetFrameRate = targetFrameRate;
        frameRangeMultiplier = (float)targetFrameRate / 60f;
    }

    private void Update()
    {
        if (Application.targetFrameRate != targetFrameRate)
            Application.targetFrameRate = targetFrameRate;
    }
}
