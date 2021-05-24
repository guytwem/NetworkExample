using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Battlecars
{
    public class LevelManager : MonoBehaviour
    {
        private static LevelManager instance = null;

        private void Awake()
        {
            if(instance == null)
                instance = this;
            else if(instance != this)
                Destroy(gameObject);
        }

        public static void LoadLevel(string _level) => instance.LoadLevelInternal(_level);

        private void LoadLevelInternal(string _level) => StartCoroutine(LoadLevel_CR(_level));

        private IEnumerator LoadLevel_CR(string _level)
        {
            yield return SceneManager.LoadSceneAsync(_level, LoadSceneMode.Additive);

            Scene scene = SceneManager.GetSceneByName(_level);
            SceneManager.SetActiveScene(scene);
        }
    }
}