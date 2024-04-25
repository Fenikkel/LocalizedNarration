using LocalizedNarration;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace Fenikkel.LocalizedNarration.Example
{
    public class LocalizedNarrationExample : MonoBehaviour
    {
        const string AUDIO_TABLE_COLLECTION_NAME = "AudioDemoTable";
        const string CAPTIONS_TABLE_COLLECTION_NAME = "CaptionsDemoTable";
        const string TABLE_ENTRY_NAME = "DemoEntry";

        [SerializeField] Speech _Speech;

        [SerializeField] AudioClip _EnglishClip;
        [SerializeField] AudioClip _FrenchClip;
        [SerializeField] AudioClip _SpanishClip;

        [SerializeField] TextAsset _EnglishCaptions;
        [SerializeField] TextAsset _FrenchCaptions;
        [SerializeField] TextAsset _SpanishCaptions;

        Coroutine _Coroutine;

        private void Awake()
        {
            InitAndSetupLocalization();
        }

        public void PreloadSpeech()
        {
            if (_Speech == null)
            {
                Debug.LogWarning("Please, assign a Speech");
            }

            LocalizedNarrationController.Preload(_Speech); // Or -> LocalizedNarrationController.Instance.PreloadNarration(_Speech);
        }

        public void PlaySpeech()
        {
            if (_Speech == null)
            {
                Debug.LogWarning("Please, assign a Speech");
            }

            LocalizedNarrationController.Play(_Speech); // Or -> LocalizedNarrationController.Instance.PlayNarration(_LocalizedTextAsset);
        }

        public void StopSpeech()
        {
            LocalizedNarrationController.Stop();
        }

        private void InitAndSetupLocalization()
        {
            // Localization settings
            LocalizationSettings localizationSettings = LocalizationUtilities.CreateLocalizationSettings("Assets/Settings/Localization", new SystemLanguage[] { SystemLanguage.Spanish, SystemLanguage.English, SystemLanguage.French });

            if (localizationSettings != null)
            {
                if (_Coroutine != null)
                {
                    StopCoroutine(_Coroutine);
                }

                _Coroutine = StartCoroutine(CreateTables(localizationSettings));
            }
        }

        IEnumerator CreateTables(LocalizationSettings localizationSettings)
        {
            yield return new WaitUntil(() => localizationSettings.GetInitializationOperation().IsDone);

            List<Locale> locales = localizationSettings.GetAvailableLocales().Locales;


            /* Audio */
            AssetTableCollection assetTableCollection = LocalizationUtilities.CreateAssetTableCollection(AUDIO_TABLE_COLLECTION_NAME, "AudioDemo", "Assets/Settings/Localization/Tables", locales);

            Dictionary<LocaleIdentifier, AudioClip> audioDictionary = new Dictionary<LocaleIdentifier, AudioClip>();
            audioDictionary.Add(new LocaleIdentifier(SystemLanguage.Spanish), _SpanishClip);
            audioDictionary.Add(new LocaleIdentifier(SystemLanguage.French), _FrenchClip);
            audioDictionary.Add(new LocaleIdentifier(SystemLanguage.English), _EnglishClip);

            bool success = LocalizationUtilities.CreateAssetTableEntry<AudioClip>(assetTableCollection, TABLE_ENTRY_NAME, audioDictionary);

            if (success)
            {
                _Speech.LocalizedAudio.SetReference(assetTableCollection.TableCollectionNameReference, TABLE_ENTRY_NAME);

                EditorUtility.SetDirty(this);
                EditorUtility.SetDirty(_Speech);
            }

            /* Captions */
            assetTableCollection = LocalizationUtilities.CreateAssetTableCollection(CAPTIONS_TABLE_COLLECTION_NAME, "CaptionsDemo", "Assets/Settings/Localization/Tables", locales);

            Dictionary<LocaleIdentifier, TextAsset> captionsDictionary = new Dictionary<LocaleIdentifier, TextAsset>();
            captionsDictionary.Add(new LocaleIdentifier(SystemLanguage.Spanish), _SpanishCaptions);
            captionsDictionary.Add(new LocaleIdentifier(SystemLanguage.French), _FrenchCaptions);
            captionsDictionary.Add(new LocaleIdentifier(SystemLanguage.English), _EnglishCaptions);

            success = LocalizationUtilities.CreateAssetTableEntry<TextAsset>(assetTableCollection, TABLE_ENTRY_NAME, captionsDictionary);

            if (success)
            {
                _Speech.LocalizedTextAsset.SetReference(assetTableCollection.TableCollectionNameReference, TABLE_ENTRY_NAME);

                EditorUtility.SetDirty(this);
                EditorUtility.SetDirty(_Speech);
            }


            _Coroutine = null;
        }
    }
}
