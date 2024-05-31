using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Object.WeaponScripts
{
    public class Sword : Weapon
    {
        public float wallDuration = 3f;

        private bool isWall = false;
        public override bool Skill(int idx, float time)
        {
            if (idx >= coolTimes.Length || coolTimes[idx] > time) return false;

            OnSkillUse?.Invoke(idx, coolTimes[idx]);
            switch (idx)
            {
                case 0:
                    _animator.SetTrigger(ATTACK_TRIGGER);
                    return true;
                case 1:
                    myPlayer.Dash();
                    return true;
                case 2:
                    if (isWall) return false;
                    StartCoroutine(IWall());
                    return true;
                default:
                    return true;
            }
        }

        private IEnumerator IWall()
        {
            isWall = true;
            var _parent = transform.parent;
            
            transform.SetParent(myPlayer.transform);
            _animator.SetBool(WALL, true);
            float wallTime  = 0f;

            while (wallTime < wallDuration)
            {
                wallTime += Time.deltaTime;
                yield return null;
            }
            _animator.SetBool(WALL, false);
            transform.SetParent(_parent);
            isWall = false;
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && other.gameObject!=myPlayer.gameObject)
            {
                other.GetComponent<Player>().TakeDamage(1f);
            }
        }
    }
}