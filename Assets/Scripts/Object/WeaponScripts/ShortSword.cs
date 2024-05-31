using UnityEngine;

namespace Object.WeaponScripts
{
    public class ShortSword : Weapon
    {
        public bool powerAttack;
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
                    _animator.SetTrigger(POWER_ATTACK);
                    return true;
                default:
                    return true;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && other.gameObject!=myPlayer.gameObject)
            {
                if(powerAttack) other.GetComponent<Player>().TakeDamage(2f);
                else other.GetComponent<Player>().TakeDamage(0.25f);
                
            }
        }
    }
}