using System.Collections;
using UnityEngine;

namespace Object.WeaponScripts
{
    public class Spear : Weapon
    {
        private bool _isKnockBack;
        public float knockBackDuration = 3f;
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
                    if (_isKnockBack) return false;
                    StartCoroutine(IKnockBack());
                    return true;
                default:
                    return true;
            }
        }   
        
        private IEnumerator IKnockBack()
        {
            _isKnockBack = true;
            _animator.SetBool(KNOCK_BACK, true);
            float wallTime  = 0f;

            while (wallTime < knockBackDuration)
            {
                wallTime += Time.deltaTime;
                yield return null;
            }
            _animator.SetBool(KNOCK_BACK, false);
            _isKnockBack = false;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && other.gameObject!=myPlayer.gameObject)
            {
                other.GetComponent<Player>().TakeDamage(_isKnockBack ? 0.1f : 1f);
            }
        }
    }
}