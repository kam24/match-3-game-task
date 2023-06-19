using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Root : MonoBehaviour
    {
        [SerializeField] private TilesRow _tilesRow;
        [SerializeField] private TileClickHandler _tileClickHandler;
        [SerializeField] private RectTransform _levelCompletePanel;
        [SerializeField] private RectTransform _levelFailedPanel;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _restartLevelButton;

        private void Awake()
        {
            _tileClickHandler.Init(_tilesRow);
        }

        private void OnEnable()
        {
            _tileClickHandler.AllTilesWithdrawn += _tilesRow.OnAllTilesWithdrawn;
            _tilesRow.AllTilesDestroyed += FinishGameWithSuccess;
            _tilesRow.BecameNotAvailableForNewTiles += FinishGameWithFail;
            _nextLevelButton.onClick.AddListener(LoadNextLevel);
            _restartLevelButton.onClick.AddListener(RestartLevel);
        }

        private void OnDisable()
        {
            _tileClickHandler.AllTilesWithdrawn -= _tilesRow.OnAllTilesWithdrawn;
            _tilesRow.AllTilesDestroyed -= FinishGameWithSuccess;
            _tilesRow.BecameNotAvailableForNewTiles -= FinishGameWithFail;
            _nextLevelButton.onClick.RemoveListener(LoadNextLevel);
            _restartLevelButton.onClick.RemoveListener(RestartLevel);
        }

        private void FinishGameWithSuccess()
        {
            _levelCompletePanel.gameObject.SetActive(true);
        }

        private void FinishGameWithFail()
        {
            _levelFailedPanel.gameObject.SetActive(true);
        }

        private void LoadNextLevel()
        {
            string levelPath = SceneManager.GetActiveScene().path;
            int levelsCount = SceneManager.sceneCountInBuildSettings;
            if (levelsCount <= 1)
                throw new InvalidOperationException();  
            string nextLevelPath = levelPath;
            while (nextLevelPath == levelPath)
            {
                int index = Random.Range(0, levelsCount);
                nextLevelPath = SceneUtility.GetScenePathByBuildIndex(index);
            }
            SceneManager.LoadScene(nextLevelPath);
        }

        private void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().path);
        }
    }
}
