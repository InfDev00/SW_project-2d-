using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace Object
{
    public abstract class Weapon : MonoBehaviourPun
    {
        protected Animator _animator;
        public Player myPlayer;
        
        public float[] coolTimes;
        
        #region Animation Name

        protected const string ATTACK_TRIGGER = "AttackTrigger";
        protected const string WALL = "wall";
        protected const string KNOCK_BACK = "KnockBack";
        protected const string POWER_ATTACK = "PowerAttackTrigger";

        #endregion

        #region eventHandler

        public Action<int, float> OnSkillUse = null;

        #endregion

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public abstract bool Skill(int idx, float time);
    }
}