using System;
using UnityEngine;
using UnityEngine.Localization.Components;

/*
    This script it's linked with LocalizedDefaultAsset and UnityEventDefaultAsset
    Everyone has to be [Serializable] to work. Also every script has to be an independent script (.cs)
 */

[AddComponentMenu("Localization/Asset/Localize DefaultAsset Event")]
[Serializable] // It's mandatory
public class LocalizeSpeechEvent : LocalizedAssetEvent<Speech, LocalizedSpeech, UnityEventSpeech> { }

