using System;
using System.Collections;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class LoadingUI : MonoBehaviour
    {
        public GameObject loadingCanvas;
        private TextMeshProUGUI _loadingText;

        private void Start()
        {
            _loadingText = loadingCanvas.GetComponentInChildren<TextMeshProUGUI>();

            StartCoroutine(LoadingText());
            
            loadingCanvas.SetActive(true);
            NetworkManager.Instance.OnConnectToMasterSuccess += () => { SceneManager.LoadScene("1. Init"); };
            NetworkManager.ConnectToMaster();
        }

        private IEnumerator LoadingText()
        {
            _loadingText.text = "LOADING.";
            yield return new WaitForSeconds(0.5f);
            _loadingText.text = "LOADING..";
            yield return new WaitForSeconds(0.5f);
            _loadingText.text = "LOADING...";
            yield return new WaitForSeconds(0.5f);
        }
    }
}