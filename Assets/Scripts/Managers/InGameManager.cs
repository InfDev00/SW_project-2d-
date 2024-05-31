using System;
using System.Collections;
using Object;
using Photon.Pun;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class InGameManager : MonoBehaviourPun
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform spawnPoints;
        public float zoomDuration = 2f;

        public InGameUI inGameUI;
        
        private Transform[] _spawnPointArray;
        private int _playerIndex;
        
        private GameObject _player;
        private bool _gameEnd;
        
        private void Awake()
        {
            _spawnPointArray = new Transform[spawnPoints.childCount];
            for (int i = 0; i < _spawnPointArray.Length; ++i) _spawnPointArray[i] = spawnPoints.GetChild(i);

            inGameUI.OnGameEnd += GameEnd;
        }

        private void OnDestroy()
        {
            inGameUI.OnGameEnd -= GameEnd;
        }

        private void Start()
        {
            _playerIndex = NetworkManager.GetPlayerIndex();
            _player = NetworkManager.Instance.InstantiatePlayer(_spawnPointArray[_playerIndex]);
            _player.GetComponent<Player>()._skillController._weapon.OnSkillUse += inGameUI.OnUseSkill;
            _player.GetComponent<Player>().DieEvent += PlayerDieEvent;
            
            NetworkManager.Instance.OnEnemyLeftRoom += OnEnemyLeftRoom;
            NetworkManager.Instance.OnPlayerKilled += PlayerKilled;
            
            StartCoroutine(Zoom());
        }
        private IEnumerator Zoom()
        {
            float elapsedTime = 0f;
            mainCamera.orthographicSize = 1f;

            while (elapsedTime < zoomDuration)
            {
                mainCamera.orthographicSize = Mathf.Lerp(1f, 5f, elapsedTime / zoomDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            mainCamera.orthographicSize = 5f;
        }

        public void PlayerDieEvent()
        {
            inGameUI.PlayerDied();
            NetworkManager.Instance.PlayerDieBegin();
        }
        private void PlayerKilled()
        {
            inGameUI.PlayerKill();
        }

        private IEnumerator Shake(float shakeAmount, float shakeTime)
        {
            float timer = 0;
            while (timer <= shakeTime)
            {
                var shake = Random.insideUnitCircle * shakeAmount;
                mainCamera.transform.position = new Vector3(shake.x, shake.y, -10);
                timer += Time.deltaTime;
                yield return null;
            }
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);
        }
        
        private void OnEnemyLeftRoom()
        {
            if (!_gameEnd) GameEnd(GameCode.RUN);
        }

        public enum GameCode
        {
            DRAW,
            WIN,
            LOSS,
            RUN
        };
        public void GameEnd(GameCode code)
        {
            _gameEnd = true;
            Time.timeScale = 0f;
            switch (code)
            {
                case GameCode.DRAW:
                    inGameUI.GameEnd("DRAW");
                    break;
                case GameCode.WIN:
                    inGameUI.GameEnd("WIN");
                    break;
                case GameCode.LOSS:
                    inGameUI.GameEnd("LOSS");
                    break;
                case GameCode.RUN:
                    inGameUI.GameEnd("Enemy Run");
                    break;
            }
        }
    }
}