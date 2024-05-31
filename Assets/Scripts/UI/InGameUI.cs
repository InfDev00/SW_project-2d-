using System;
using System.Collections;
using Managers;
using Object;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class InGameUI : MonoBehaviour
    {
        [Header("Image")] 
        public Image clock;
        public GameObject countGroup;
        private Image[] _killCount;
        private Image[] _lifeCount;
        private int _currentKill;
        private int _currentLife;

        public Image[] skillIcon;
        private Image[] _skillCoolImage;
        private TextMeshProUGUI[] _skillCoolTimes;
        
        [Header("Text")] 
        public TextMeshProUGUI timeText;
        public TextMeshProUGUI aliveText;
        
        [Tooltip("Time is Sec")]
        public float gamePlayTime = 300;
        private float _initTime;

        [Header("Ending")] 
        public GameObject endingGroup;
        private TextMeshProUGUI _endingText;
        private Button _endingButton;
        
        #region EventHandler

        public Action<InGameManager.GameCode> OnGameEnd = null;

        #endregion
        
        private void Awake()
        {
            timeText.text = TimeSetting((int)gamePlayTime);
            _initTime = gamePlayTime;

            _currentKill = 0;
            _currentLife = 3;
            _killCount = new Image[3];
            _lifeCount = new Image[3];
            var tmp = countGroup.GetComponentsInChildren<Image>();
            for (int i = 0; i < 3; ++i)
            {
                _killCount[i] = tmp[i];
                _lifeCount[i] = tmp[i + 3];

                _killCount[i].color = Color.black;
            }

            _skillCoolImage = new Image[3];
            _skillCoolTimes = new TextMeshProUGUI[3];
            for (int i = 0; i < 3; ++i)
            {
                _skillCoolImage[i] = skillIcon[i].transform.GetChild(0).GetComponent<Image>();
                _skillCoolTimes[i] = _skillCoolImage[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                
                _skillCoolImage[i].gameObject.SetActive(false);
            }

            _endingText = endingGroup.GetComponentInChildren<TextMeshProUGUI>();
            _endingButton = endingGroup.GetComponentInChildren<Button>();
            _endingButton.onClick.AddListener(BackToMain);
            endingGroup.SetActive(false);
            aliveText.gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            gamePlayTime -= Time.fixedDeltaTime;
            timeText.text = TimeSetting((int)gamePlayTime);

            if (gamePlayTime <= 0) OnGameEnd?.Invoke(InGameManager.GameCode.DRAW);

            clock.fillAmount = (gamePlayTime / _initTime);
        }

        private static string TimeSetting(int sec)
        {
            var min = sec / 60;
            sec = sec % 60;
            return $"{min:D2}:{sec:D2}";
        }
        
        public void PlayerDied()
        {
            StartCoroutine(PlayerDieCount());
            
            if (_currentLife <= 0)
            {
                OnGameEnd?.Invoke(InGameManager.GameCode.LOSS);
                return;
            }
            _lifeCount[3-_currentLife].color = Color.black;
            _currentLife--;
        }

        private IEnumerator PlayerDieCount()
        {
            aliveText.gameObject.SetActive(true);
            int count = 5;

            while (count > 0)
            {
                aliveText.text = $"Alive in {count}s...";
                count -= 1;
                yield return new WaitForSeconds(1f);
            }
            aliveText.gameObject.SetActive(false);
        }
        
        public void PlayerKill()
        {
            if (_currentKill >= 3)
            {
                OnGameEnd?.Invoke(InGameManager.GameCode.WIN);
                return;
            }
            _killCount[_currentKill].color = Color.white;
            _currentKill++;

            if (_currentKill == 3)
            {
                for(int i = 0;i<3;++i) _killCount[i].color = Color.red;
            }
        }

        public void OnUseSkill(int idx, float cooltime)
        {
            StartCoroutine(ISkillUse(idx, cooltime));
        }

        IEnumerator ISkillUse(int idx, float cooltime)
        {
            _skillCoolImage[idx].gameObject.SetActive(true);
            float initCooltime = cooltime;
            
            while (cooltime > 0f)
            {
                var intCoolTime = (int)cooltime;
                _skillCoolImage[idx].fillAmount = cooltime / initCooltime;
                _skillCoolTimes[idx].text = $"{intCoolTime + 1}";
                
                cooltime -= Time.deltaTime;
                yield return null;
            }
            
            _skillCoolImage[idx].gameObject.SetActive(false);
        }

        public void GameEnd(string text)
        {
            _endingText.text = text;
            endingGroup.SetActive(true);
        }

        private void BackToMain()
        {
            Time.timeScale = 1f;
            _endingButton.interactable = false;
            NetworkManager.LeaveRoom();
        }
    }
}