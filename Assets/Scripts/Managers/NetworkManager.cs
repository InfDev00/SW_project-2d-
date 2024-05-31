using System;
using System.Collections;
using Bases;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class NetworkManager : PSingleton<NetworkManager>
    {
        private static byte _maxPlayerPerRoom = 2;
        public string gameVersion = "1";

        public string[] playerPrefabNames;
        private static string _currentPlayerPrefabName;

        private GameObject player;
        
        #region NetworkActions

        public Action OnConnectToMasterSuccess = null;
        public Action OnPlayerKilled = null;
        public Action OnEnemyLeftRoom = null;
        
        #endregion
        
        private void Awake()
        {
            CreateSingleton(this);

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = gameVersion;
        }

        #region Server

        public static void ConnectToMaster() => PhotonNetwork.ConnectUsingSettings();

        public override void OnConnectedToMaster() => OnConnectToMasterSuccess?.Invoke();
        public static void DisconnectToMaster() => PhotonNetwork.Disconnect();

        public static void SetMaxPlayer(byte maxPlayer) => _maxPlayerPerRoom = maxPlayer;
        #endregion

        #region Room

        public void ConnectToRoom(string nickname, int idx)
        {
            PhotonNetwork.LocalPlayer.NickName = nickname;
            _currentPlayerPrefabName = playerPrefabNames[idx];
            
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomOrCreateRoom(roomOptions:new RoomOptions{MaxPlayers = _maxPlayerPerRoom});
            }
            else PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnJoinedRoom()
        {
            photonView.RPC(nameof(PlayerJoined), RpcTarget.MasterClient);
        }

        [PunRPC]
        private void PlayerJoined()
        {
            if(PhotonNetwork.PlayerList.Length==_maxPlayerPerRoom) PhotonNetwork.LoadLevel("4. Game");
        }

        #endregion

        #region InGame

        public static string GetPlayerNickname(int idx)
        {
            return PhotonNetwork.PlayerList[idx].NickName;
        }

        public static int GetPlayerIndex()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber - 1 >= _maxPlayerPerRoom) return _maxPlayerPerRoom - 1;
            else return PhotonNetwork.LocalPlayer.ActorNumber - 1;
        }
        public GameObject InstantiatePlayer(Transform playerTransform)
        {
            player = PhotonNetwork.Instantiate(_currentPlayerPrefabName, playerTransform.position, playerTransform.rotation, 0);
            return player;
        }
        
        public void PlayerDieBegin() => photonView.RPC(nameof(PlayerKill), RpcTarget.Others);

        [PunRPC]
        private void PlayerKill() => OnPlayerKilled?.Invoke();
        
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            OnEnemyLeftRoom?.Invoke();
        }

        public static void LeaveRoom() => PhotonNetwork.LeaveRoom();
        
        public override void OnLeftRoom()
        {
            Debug.Log("Leave");
            SceneManager.LoadScene("1. Init");
        }
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("Disconnected from Photon with reason {0}", cause);
        }

        #endregion
    }
}