using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    // public static SceneLoader Instance { get; private set; }

    [SerializeField] private float defaultDelay = 1f;
    [SerializeField] private int defaultSceneIndex = 0;
    [SerializeField] private string defaultSceneName = "";

    // private void Awake()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject); // Опционально, если нужен глобальный доступ
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    // Загрузка по индексу
    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"Scene index {sceneIndex} is out of range!");
            LoadDefaultScene();
            return;
        }

        SceneManager.LoadScene(sceneIndex);
    }

    // Загрузка по имени
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty!");
            LoadDefaultScene();
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    // Загрузка с задержкой
    public void LoadScene(int sceneIndex, float delay) => StartCoroutine(LoadSceneWithDelayCoroutine(sceneIndex, delay));

    public void LoadScene(string sceneName, float delay) => StartCoroutine(LoadSceneWithDelayCoroutine(sceneName, delay));

    public void LoadSceneWithDelay(int sceneIndex) => StartCoroutine(LoadSceneWithDelayCoroutine(sceneIndex, defaultDelay));

    public void LoadSceneWithDelay(string sceneName) => StartCoroutine(LoadSceneWithDelayCoroutine(sceneName, defaultDelay));

    private IEnumerator LoadSceneWithDelayCoroutine(int sceneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadScene(sceneIndex);
    }

    private IEnumerator LoadSceneWithDelayCoroutine(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadScene(sceneName);
    }

    /*
    // Асинхронная загрузка с прогрессом
    public void LoadSceneAsync(int sceneIndex, System.Action<float> onProgress = null) => StartCoroutine(LoadSceneAsyncCoroutine(sceneIndex, onProgress));

    private IEnumerator LoadSceneAsyncCoroutine(int sceneIndex, System.Action<float> onProgress)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!asyncOperation.isDone)
        {
            onProgress?.Invoke(asyncOperation.progress);
            yield return null;
        }
    }
    */

    // Загрузка дефолтной сцены (можно настроить в инспекторе)
    private void LoadDefaultScene()
    {
        if (!string.IsNullOrEmpty(defaultSceneName))
        {
            LoadScene(defaultSceneName);
        }
        else
        {
            LoadScene(defaultSceneIndex);
        }
    }

    // Перезагрузка текущей сцены
    public void ReloadCurrentScene() => LoadScene(SceneManager.GetActiveScene().buildIndex);
}