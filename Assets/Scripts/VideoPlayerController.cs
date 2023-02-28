using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;
using TMPro;

[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerController : MonoBehaviour
{
    #region EDITOR

    [HideInInspector] public VideoPlayer videoPlayer;

    #endregion

    #region UI

    [SerializeField] private TextMeshProUGUI textCurrentTime;
    [SerializeField] private TextMeshProUGUI textDuration;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Slider volume;

    #endregion

    #region VIDEO PLAYER PROPERTIES

    private string videoPath;
    private double progressTime;
    private bool enableLooping;
    private bool enableMute;

    #endregion

    #region Unity Events

    private void Awake()
    {
        if(videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();
        else
            Debug.LogError("<b>[Video Player Controller]</b> object is missing.");

        ResetRenderTexture();
    }

    private void Start()
    {
        FileBrowser.FileSelected += OnFileSelected;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void Update()
    {
        UpdateProgressBar();
    }

    private void OnDestroy()
    {
        FileBrowser.FileSelected -= OnFileSelected;
        videoPlayer.loopPointReached -= OnVideoFinished;
    }

    #endregion

    #region VIDEO UI

    public void Play()
    {
        if(string.IsNullOrEmpty(videoPath))
        {
            Debug.LogWarning("Make sure there is a video path to play.");
            return;
        }

        videoPlayer.errorReceived += OnerrorReceived;
        
        videoPlayer.Play();
    }

    public void Pause()
    {
        videoPlayer.Pause();
    }

    public void Stop()
    {
        ResetPlayback();
    }

    public void ToogleLooping()
    {
        enableLooping = !enableLooping;
        videoPlayer.isLooping = enableLooping;
    }

    public void ToggleMute()
    {
        enableMute = !enableMute;
        for (ushort i = 0; i < videoPlayer.audioTrackCount; i++)
            videoPlayer.SetDirectAudioMute(i, enableMute);
    }

    public void SeekPlaybackTime()
    {
        if (videoPlayer && progressBar)
        {
            videoPlayer.Pause();
            progressTime = progressBar.value * videoPlayer.length;
        }
    }

    public void JumpToTime()
    {
        videoPlayer.time = progressTime;
        videoPlayer.Play();
    }

    private void ResetPlayPuase()
    {
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }

    public void ResetPlayback()
    {
        if (enableLooping)
            return;

        videoPlayer.Stop();
        videoPlayer.time = 0f;
        progressBar.value = 0f;

        ResetPlayPuase();
        ResetRenderTexture();
    }

    private void ResetRenderTexture()
    {
        videoPlayer.targetTexture.Release();
    }

    #endregion

    #region VIDEO PLAYER METHODS PROPERTIES

    private void OnFileSelected(string path)
    {
        videoPath = path;
        videoPlayer.url = videoPath;
        ResetPlayPuase();
    }

    private void OnerrorReceived(VideoPlayer vp, string error)
    {
        Debug.LogError("Error playing video: " + error);
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        textDuration.text = GetDurationFormatted(videoPlayer.length);
        Debug.Log("video player is ready");
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Playback has finished");
        ResetPlayback();
    }

    private void UpdateProgressBar()
    {
        if (progressBar && videoPlayer.isPlaying && videoPlayer.time > 0)
        {
            progressBar.interactable = true;
            progressBar.value = (float)(videoPlayer.time / videoPlayer.length);
            textCurrentTime.text = GetDurationFormatted(videoPlayer.time);
        }
    }

    public void UpdateVolume()
    {
        if (!videoPlayer.isPlaying)
            volume.interactable = false;
        else
        {
            volume.interactable = true;
            for (ushort i = 0; i < videoPlayer.audioTrackCount; i++)
                videoPlayer.SetDirectAudioVolume(i, volume.value);
        }
    }

    public string GetDurationFormatted(double val)
    {
        TimeSpan time = TimeSpan.FromSeconds(val);
        string timeString = time.ToString(@"hh\:mm\:ss");
        return timeString;
    }

    #endregion
}
