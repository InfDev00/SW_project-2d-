using System;
using System.Collections;
using Interface;
using Managers;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Object
{
    public abstract class Player : MonoBehaviourPun, IPunObservable, IDamageable
    {
        private Vector3 _remotePos;
        private Quaternion _remoteRot;
        private float _remoteHp;
        
        private Vector3 _initPosition;
        
        protected Vector3 _moveDirection;
        protected Vector3 _dashDirection;
        [SerializeField] protected float moveSpeed = 4f;
        [SerializeField] private float maxHp = 10;
        [SerializeField] protected float dashDuration = 0.4f;
        [SerializeField] protected float dashSpeed = 10f;
        private float _hp;
        protected bool _isDash = false;
        private bool _die = false;
        
        [Header("Objects")]
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private Slider playerHp;
        [SerializeField] protected GameObject dashEffect;
        public SkillController _skillController;
        private Animator _animator;

        public Action DieEvent = null;
        
        private void Awake()
        {
            dashEffect.SetActive(false);
            _initPosition = transform.position;
            
            _skillController = GetComponentInChildren<SkillController>();
            _skillController.myPlayer = this;
            _skillController.transform.localRotation = transform.rotation;
            
            _dashDirection = Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward) * Vector3.up;
            transform.rotation = Quaternion.identity;
            _animator = GetComponent<Animator>();
            
            _hp = maxHp;
            _remoteHp = maxHp;
            playerHp.value = _hp / maxHp;
        }

        protected virtual void Start()
        {
            var idx = NetworkManager.GetPlayerIndex();
            if (!photonView.IsMine) idx = -1 * (idx - 1);
            playerName.text = NetworkManager.GetPlayerNickname(idx);
        }

        private void FixedUpdate()
        {
            RotateSkillController();
            Move();
            
            _hp = Mathf.Lerp(_hp, _remoteHp, 10 * Time.fixedDeltaTime);
            playerHp.value = _hp / maxHp;
        }

        private void Move()
        {
            if (!photonView.IsMine)
            {
                transform.position = Vector3.Lerp(transform.position, _remotePos, 10 * Time.deltaTime);
            }
            else
            {
                transform.position += _moveDirection * (Time.deltaTime * moveSpeed);
            }
        }
        
        private void RotateSkillController()
        {
            if (!photonView.IsMine)
            {
                _skillController.transform.rotation =
                    Quaternion.Lerp(_skillController.transform.rotation, _remoteRot, 10 * Time.deltaTime);   
            }
            else
            {
                var angle = Mathf.Atan2(_dashDirection.y, _dashDirection.x) * Mathf.Rad2Deg;
                var targetRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                _skillController.transform.rotation = Quaternion.Lerp(_skillController.transform.rotation, targetRotation,
                    10 * Time.deltaTime);
            }
        }
        
        #region PlayerInput

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!photonView.IsMine) return;

            var inputVec = context.ReadValue<Vector2>();
            _moveDirection = new Vector3(inputVec.x, inputVec.y, 0);
            if (inputVec != Vector2.zero) _dashDirection = _moveDirection;
        }

        public void OnSkill(InputAction.CallbackContext context)
        {
            if (!photonView.IsMine) return;
            var keyPressed = context.control.displayName;
            
            if (context.started) photonView.RPC("SkillRPC", RpcTarget.All, keyPressed);
        }
        [PunRPC]
        protected void SkillRPC(string keyName)
        {
            var idx = keyName switch
            {
                "Q" => 0,
                "W" => 1,
                "E" => 2,
                _ => 0
            };

            _skillController.UseSKill(idx);
        }

        public abstract void Dash();

        #endregion
        
        #region BattleHandler
        
        public void TakeDamage(float damage)
        {
            _remoteHp -= damage;
            if (_remoteHp <= 0 && photonView.IsMine && !_die) StartCoroutine(PlayerDied());
        }

        protected virtual IEnumerator PlayerDied()
        {
            _die = true;
            _animator.SetTrigger("PlayerDieTrigger");
            DieEvent?.Invoke();
            
            yield return new WaitForSeconds(0.3f);
            transform.position = new Vector3(100, 100, 100);
            yield return new WaitForSeconds(5f);
            _hp = maxHp;
            _remoteHp = maxHp;
            playerHp.value = _hp / maxHp;
            transform.position = _initPosition;
            _die = false;
        }
        
        #endregion

        #region Synchronize

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(_skillController.transform.rotation);
                stream.SendNext(_hp);
            }
            else
            {
                _remotePos = (Vector3)stream.ReceiveNext();
                _remoteRot = (Quaternion)stream.ReceiveNext();
                _remoteHp = (float)stream.ReceiveNext();
            }
        }

        #endregion
    }
}