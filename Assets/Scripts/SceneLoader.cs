using System.Collections;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] protected float sceneLoadDelay = 1f;
    [SerializeField] public int loadingSceneNumber = 0;

    public void ChangeScene() => StartCoroutine(LoadScene());

    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(loadingSceneNumber);
    }
}