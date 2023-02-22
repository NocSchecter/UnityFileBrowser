using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseApp : MonoBehaviour
{
    private bool isClosing = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isClosing)
        {
            isClosing = true;
            Application.Quit();
        }
    }

    private void OnApplicationQuit()
    {
        isClosing = true;
    }
}
