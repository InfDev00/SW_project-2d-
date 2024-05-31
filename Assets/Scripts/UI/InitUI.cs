using System;
using System.Collections;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class InitUI : MonoBehaviour
    {
        [Header("Button")]
        public Button onlineButton;
        private void Awake()
        {
            onlineButton.onClick.AddListener(OnOnlineButtonClick);
            StartCoroutine(ButtonBlink());
        }

        private IEnumerator ButtonBlink()
        { 
            while (true)
            {
                onlineButton.image.color = Color.clear;
                yield return new WaitForSeconds(1f);
                onlineButton.image.color = Color.white;
                yield return new WaitForSeconds(1f);
            }
            // ReSharper disable once IteratorNeverReturns
        }
        
        private void OnOnlineButtonClick()
        {
            onlineButton.interactable = false;
            NetworkManager.SetMaxPlayer(2);
            SceneManager.LoadScene("2. Online");
        }
    }
}