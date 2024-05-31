using System;
using Photon.Pun;
using UnityEngine;

namespace Object
{
    public class SkillController : MonoBehaviourPun
    {
        public Weapon _weapon;
        public Player myPlayer;

        private float[] _timers;
        private void Awake()
        {
            _weapon = GetComponentInChildren<Weapon>();
            _weapon.myPlayer = myPlayer;

            _timers = new float[3];
            for (int i = 0; i < 3; ++i) _timers[i] = 100f;
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < 3; ++i) _timers[i] += Time.fixedDeltaTime;
        }

        public void UseSKill(int idx)
        {
            if (_weapon.Skill(idx, _timers[idx])) _timers[idx] = 0f;
        }
    }
}