using UnityEngine;
using UnityEditor;
using UnityEngine.Video;

[CustomEditor(typeof(VideoPlayerController))]
public class VideoPlayerControllerEditor : Editor
{
    private VideoPlayerController videoPlayerController;

    private void OnEnable()
    {
        videoPlayerController = (VideoPlayerController)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        if (videoPlayerController.videoPlayer)
        {
            EditorGUILayout.LabelField("Duration: " + videoPlayerController.GetDurationFormatted(videoPlayerController.videoPlayer.length));
            EditorGUILayout.LabelField("Resolution: " + videoPlayerController.videoPlayer.width + " x " + videoPlayerController.videoPlayer.height);
            EditorGUILayout.LabelField("Frames: " + videoPlayerController.videoPlayer.frameCount);
            EditorGUILayout.LabelField("Frame rate: " + videoPlayerController.videoPlayer.frameRate);
            EditorGUILayout.LabelField("Audio tracks: " + videoPlayerController.videoPlayer.audioTrackCount);
        }
    }
}