using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitLocalizationLoaded : MonoBehaviour
{
    public string scene;
    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { StartCoroutine(LoadGameScene());});
    }

    IEnumerator LoadGameScene()
    {
        #if UNITY_WEBGL
        yield return LocalizationSettings.InitializationOperation;
        while (!FMODUnity.RuntimeManager.HasBankLoaded("Master"))
        {
            yield return null;
        }
        while (!FMODUnity.RuntimeManager.HasBankLoaded("Musique"))
        {
            yield return null;
        }
        while (!FMODUnity.RuntimeManager.HasBankLoaded("Ambiance"))
        {
            yield return null;
        }
        while (!FMODUnity.RuntimeManager.HasBankLoaded("SFX"))
        {
            yield return null;
        }
        while (!FMODUnity.RuntimeManager.HasBankLoaded("Master.strings"))
        {
            yield return null;
        }
        #endif
        yield return null;
        SceneManager.LoadScene(scene);
    }
}
