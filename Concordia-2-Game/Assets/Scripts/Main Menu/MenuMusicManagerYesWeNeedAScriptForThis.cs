using con2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusicManagerYesWeNeedAScriptForThis : MonoBehaviour
{
    public GameObject MusicPrefab;

    // Start is called before the first frame update
    void Start()
    {
        var musicPlayer = GameObject.FindGameObjectWithTag(Tags.MENU_MUSIC);
        if (musicPlayer == null)
        {
            Instantiate(MusicPrefab, this.transform);
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == SceneNames.Game)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        var audio = GetComponent<AudioSource>();
        if (audio)
            audio.Stop();
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

}
