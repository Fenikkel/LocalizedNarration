using LocalizedAudio;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "Speech", menuName = "Fenikkel/Speech", order = 2)]
public class Speech : ScriptableObject
{
    public LocalizedAudioClip LocalizedAudio;
    public LocalizedTextAsset LocalizedTextAsset; // .srt file (or .txt)
}
