using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;

public class ASyncManager : MonoBehaviour
{
    [Header("Menu Screens")]
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _mainMenu;

    [Header("Slider")]
    [SerializeField] private Slider _loadingSlider;

    public void LoadLevel(string levelToLoad)
    {
        _mainMenu.SetActive(false);
        _loadingScreen.SetActive(true);

        LoadLevelAsync(levelToLoad);
    }

    //IEnumerator LoadLevelASync(string levelToLoad)
    //{
    //AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);

    //while (!loadOperation.isDone)
    //{
    //    float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
    //    _loadingSlider.value = progress;
    //    Debug.Log("Loading progress: " + (loadOperation.progress) + "%");
    //    yield return null;
    //}
    //}

    private async void LoadLevelAsync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        loadOperation.allowSceneActivation = false;

        while (loadOperation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            //_loadingSlider.value = progress;
            _loadingSlider.value = 0.5f;
            Debug.Log("Loading progress: " + (loadOperation.progress) + "%");
            await Task.Yield();
        }

        // 加載完成，設置進度條為 1
        _loadingSlider.value = 1f;
        Debug.Log("Loading complete");

        // 等待一段時間，然後激活場景
        await Task.Delay(1000);
        loadOperation.allowSceneActivation = true;
    }

}