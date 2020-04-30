using CombatDesigner;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizeBoxToggle : MonoBehaviour
{
    Toggle toggle;
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.isOn = false;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleColliderVisualizer();
        }
    }

    public void ToggleColliderVisualizer()
    {
        toggle.isOn = !toggle.isOn;

        ColliderVisualizer[] actors = FindObjectsOfType<ColliderVisualizer>();
        foreach (var item in actors)
        {
            item.EnableColliderVisualizer(toggle.isOn);
        }
    }
}














