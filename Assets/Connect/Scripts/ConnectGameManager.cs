using Connect.Common;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Connect.Core
{
    public class ConnectGameManager : MonoBehaviour
    {
        #region START_METHODS

        public static ConnectGameManager Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Init();
                DontDestroyOnLoad(gameObject);
                return;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Init()
        {
            CurrentStage = 1;
            CurrentLevel = 1;

            Levels = new Dictionary<string, ConnectLevelData>();

            foreach (var item in _allLevels.Levels)
            {
                Levels[item.ConnectLevelName] = item;
            }
        }
        #endregion
        #region GAME_VARIABLES

        [HideInInspector]
        public int CurrentStage;

        [HideInInspector]
        public int CurrentLevel;

        [HideInInspector]
        public string StageName;

        public bool IsLevelUnlocked(int level)
        {
            string levelName = "Level" + CurrentStage.ToString() + level.ToString();

            if (level == 1)
            {
                PlayerPrefs.SetInt(levelName, 1);
                return true;
            }

            if (PlayerPrefs.HasKey(levelName))
            {
                return PlayerPrefs.GetInt(levelName) == 1;
            }

            PlayerPrefs.SetInt(levelName, 0);
            return false;
        }

        public void UnlockLevel()
        {
            CurrentLevel++;

            if (CurrentLevel == 51)
            {
                CurrentLevel = 1;
                CurrentStage++;

                if (CurrentStage == 8)
                {
                    CurrentStage = 1;
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                }
            }

            string levelName = "Level" + CurrentStage.ToString() + CurrentLevel.ToString();
            PlayerPrefs.SetInt(levelName, 1);
        }

        #endregion
        #region LEVEL_DATA
        [SerializeField]
        private ConnectLevelData DefaultLevel;

        [SerializeField]
        private ConnectLevelList _allLevels;

        private Dictionary<string, ConnectLevelData> Levels;

        public ConnectLevelData GetLevel()
        {
            return DefaultLevel;
        }
        #endregion
        #region SCENE_LOAD
        private const string Gameplay = "Gameplay";
        public void GoToGameplay()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ConnectGameplay");
        }
        #endregion
    }
}
