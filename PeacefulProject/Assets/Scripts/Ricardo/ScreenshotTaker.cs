using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotTaker : MonoBehaviour
{
    private int numshots;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            numshots++;
            string path = "Assets/Screenshots/screenshot" + numshots + ".png";
            ScreenCapture.CaptureScreenshot(path, 2);
            
        }
    }
}
