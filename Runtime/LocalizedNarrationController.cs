using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using LocalizedAudio;

#if UNITY_EDITOR
using UnityEditor;
#endif


/* 
 * Suggestion: If you can, preload the tables you gonna use so the first time doen't have a load delay
 * 
 * 
 */
    public class LocalizedNarrationController : MonoBehaviour
{
    public UnityEvent OnPreloadedNarration;

    Coroutine _SpeechCoroutine;
    Coroutine _PreloadCoroutine;

    #region Singleton
    private static LocalizedNarrationController _Instance = null;
    public static LocalizedNarrationController Instance
    {
        get
        {
            return _Instance;
        }
    }
    #endregion

    private void Awake()
    {
        CheckSingleton();
    }

    #region Controls
    public static Coroutine Preload(Speech speech)
    {
        if (Instance == null)
        {
            Debug.LogWarning($"No {typeof(LocalizedNarrationController)} on the scene ");
            return null;
        }

        return Instance.PreloadNarration(speech);
    }

    public Coroutine PreloadNarration(Speech speech)
    {
        if (LocalizedAudioController.Instance == null)
        {
            Debug.LogWarning($"No {typeof(LocalizedAudioController)} on the scene ");
            return null;
        }

        if (LocalizedCaptionsController.Instance == null)
        {
            Debug.LogWarning($"No {typeof(LocalizedCaptionsController)} on the scene ");
            return null;
        }


        if (_PreloadCoroutine != null)
        {
            StopCoroutine(_PreloadCoroutine);
            _PreloadCoroutine = null;
            Debug.LogWarning("Another preload was on course");
        }

        _PreloadCoroutine = StartCoroutine(PreloadNarrationCoroutine(speech));

        return _PreloadCoroutine;
    }

    private IEnumerator PreloadNarrationCoroutine(Speech speech)
    {
        if (speech == null)
        {
            Debug.LogWarning($"Null speech.");
            _PreloadCoroutine = null;
            yield break;
        }

        Coroutine audioPreloadCoroutine = LocalizedAudioController.Preload(speech.LocalizedAudio);
        Coroutine captionsPreloadCoroutine = LocalizedCaptionsController.Preload(speech.LocalizedTextAsset);

        yield return audioPreloadCoroutine;
        yield return captionsPreloadCoroutine;


        _PreloadCoroutine = null;


        Debug.Log("Preloaded");
    }

    public static void Play(Speech speech)
    {
        if (Instance == null)
        {
            Debug.LogWarning($"No {typeof(LocalizedNarrationController)} on the scene ");
            return;
        }

        Instance.PlayNarration(speech);
    }

    public void PlayNarration(Speech speech)
    {
        if (_SpeechCoroutine != null) 
        {
            StopCoroutine(_SpeechCoroutine);
            _SpeechCoroutine = null;
            Debug.LogWarning("Another speech was beeing played");
        }

        _SpeechCoroutine = StartCoroutine(PlaySpeechCoroutine(speech));
    }

    private IEnumerator PlaySpeechCoroutine(Speech speech)
    {

        yield return PreloadNarration(speech);

        LocalizedAudioController.Play(speech.LocalizedAudio);
        LocalizedCaptionsController.Play(speech.LocalizedTextAsset);

        _SpeechCoroutine = null;
    }

    public static void Stop()
    {
        if (Instance == null)
        {
            Debug.LogWarning($"No {typeof(LocalizedNarrationController)} on the scene ");
            return;
        }

        Instance.StopLocalizedNarration();
    }

    public void StopLocalizedNarration()
    {
        if (_SpeechCoroutine != null)
        {
            Debug.LogWarning("Stopping the localized narration.");

            StopCoroutine(_SpeechCoroutine);
            _SpeechCoroutine = null;
        }

        LocalizedCaptionsController.Stop();
        LocalizedAudioController.Stop();
    }

    #endregion

    #region Init
    private void CheckSingleton()
    {
        // Check if this instance is a duplicated
        if (_Instance != null && _Instance != this)
        {
            Debug.LogWarning($"Multiple instances of <b>{GetType().Name}</b>\nDestroying the component in <b>{name}</b>.");
            Destroy(this);
            return;
        }

        // Set this instance as the selected
        _Instance = this;
    }
    #endregion

}
