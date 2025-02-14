using System.Collections;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] protected float sceneReloadDelay = 1f;
    [SerializeField] protected int loadingSceneNumber = 0;

    public IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(sceneReloadDelay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(loadingSceneNumber);
    }
}
