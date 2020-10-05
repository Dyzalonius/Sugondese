using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Range(0.5f, 1f)]
    private float maxWindowedResolutionRatio = 0.9f;

    [Header("References")]
    [SerializeField]
    private Toggle toggleFullscreen = null;

    [SerializeField]
    private Dropdown dropdownResolution = null;

    private bool fullscreen;
    private ScreenResolution nativeResolution;
    private ScreenResolution currentScreenResolution;
    private List<ScreenResolution> screenResolutionsCurrent = new List<ScreenResolution>();
    private List<ScreenResolution> screenResolutionsTotal = new List<ScreenResolution>
        {
            new ScreenResolution(1024, 576, "16:9"),
            new ScreenResolution(1280, 720, "16:9"),
            new ScreenResolution(1600, 900, "16:9"),
            new ScreenResolution(1920, 1080, "16:9"),
            new ScreenResolution(2560, 1440, "16:9"),
            new ScreenResolution(3840, 2160, "16:9"),
        };

    private void Awake()
    {
        // Set defaults
        nativeResolution = new ScreenResolution(Display.main.systemWidth, Display.main.systemHeight);
        currentScreenResolution = new ScreenResolution(Screen.width, Screen.height);
        fullscreen = Screen.fullScreen;
        toggleFullscreen.SetIsOnWithoutNotify(fullscreen);

        // Add listeners
        toggleFullscreen.onValueChanged.AddListener(SetFullscreen);
        dropdownResolution.onValueChanged.AddListener(delegate
        {
            SetResolution(dropdownResolution);
        });

        ApplyResolution();
    }

    /// <summary>Set fullscreen to the given boolean</summary>
    private void SetFullscreen(bool fullscreen)
    {
        this.fullscreen = fullscreen;

        ApplyResolution();
    }

    /// <summary>Set resolution based on the given dropdown</summary>
    private void SetResolution(Dropdown dropdown)
    {
        ScreenResolution selectedScreenResolution = screenResolutionsCurrent[dropdown.value];
        //nativeResolution = new ScreenResolution(Display.main.systemWidth, Display.main.systemHeight); //TODO: set native resolution to the currently used monitor, instead of the main monitor
        //currentScreenResolution = new ScreenResolution(Mathf.Min(selectedScreenResolution.Width, nativeResolution.Width), Mathf.Min(selectedScreenResolution.Height, nativeResolution.Height));
        currentScreenResolution = new ScreenResolution(selectedScreenResolution.Width, selectedScreenResolution.Height);
        ApplyResolution();
    }

    /// <summary>Apply resolution and fullscreen, and fill the dropdown</summary>
    private void ApplyResolution()
    {
        PopulateDropdownResolution();
        Screen.SetResolution(currentScreenResolution.Width, currentScreenResolution.Height, fullscreen);
    }

    /// <summary>Clear and populate the resolution dropdown</summary>
    private void PopulateDropdownResolution()
    {
        dropdownResolution.ClearOptions();
        screenResolutionsCurrent.Clear();
        List<string> options = new List<string>();

        // Add allowed resolutions to screenResolutionsCurrent
        foreach (ScreenResolution screenResolution in screenResolutionsTotal)
        {
            // Continue if windowed and resolution is too large
            if (!fullscreen && (screenResolution.Width > nativeResolution.Width * maxWindowedResolutionRatio || screenResolution.Height > nativeResolution.Height * maxWindowedResolutionRatio)) { continue; }

            screenResolutionsCurrent.Add(screenResolution);
        }

        // Clamp resolution to the largest of screenResolutionsCurrent
        if (!fullscreen && (currentScreenResolution.Width > nativeResolution.Width * maxWindowedResolutionRatio || currentScreenResolution.Height > nativeResolution.Height * maxWindowedResolutionRatio) && screenResolutionsCurrent.Count > 0)
        {
            currentScreenResolution = screenResolutionsCurrent[screenResolutionsCurrent.Count - 1];
        }

        // Add current resolution to screenResolutionsCurrent
        screenResolutionsCurrent.Insert(0, currentScreenResolution);

        // Add screenResolutions to options
        foreach (ScreenResolution screenResolution in screenResolutionsCurrent)
        {
            options.Add(screenResolution.ToString());
        }

        dropdownResolution.AddOptions(options);
        dropdownResolution.SetValueWithoutNotify(0);
    }
}

/// <summary>Screen resolution used by settings menu</summary>
public struct ScreenResolution
{
    public int Width;
    public int Height;
    public string AspectRatio;

    /// <summary>Constructor</summary>
    public ScreenResolution(int width, int height)
    {
        Width = width;
        Height = height;
        AspectRatio = string.Empty;
    }

    /// <summary>Constructor</summary>
    public ScreenResolution(int width, int height, string aspectRatio)
    {
        Width = width;
        Height = height;
        AspectRatio = aspectRatio;
    }

    /// <summary>ToString override</summary>
    public override string ToString()
    {
        return Width + " x " + Height + AspectRatioText();
    }

    /// <summary>Returns aspect ratio in a text form</summary>
    private string AspectRatioText()
    {
        if (AspectRatio == "")
        {
            return AspectRatio;
        }
        else
        {
            return " (" + AspectRatio + ")";
        }
    }
}
