using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerController : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private string videoPath;

    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        FileBrowser.FileSelected += OnFileSelected;
    }

    private void OnFileSelected(string path)
    {
        videoPath = path;
    }

    public void Play()
    {
        videoPlayer.url = videoPath;
    }
}
