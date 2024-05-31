using System.Collections;
using UnityEngine;

namespace Object
{
    public class SpearPlayer : Player
    {
        private IEnumerator _dash;
        private float _initSpeed;

        protected override void Start()
        {
            _initSpeed = moveSpeed;
            base.Start();
        }

        public override void Dash()
        {
            if (_dashDirection == Vector3.zero) return;
            _dash = IDash();
            
            StartCoroutine(_dash);
        }

        protected override IEnumerator PlayerDied()
        {
            if (_dash != null) StopCoroutine(_dash);
            _isDash = false;
            dashEffect.SetActive(false);
            moveSpeed = _initSpeed;
            return base.PlayerDied();
        }

        private IEnumerator IDash()
        {
            _isDash = true;
            _moveDirection = Vector3.zero;
            dashEffect.SetActive(true);

            moveSpeed = dashSpeed;
            float dashTime = 0f;

            while (dashTime < dashDuration)
            {
                dashTime += Time.deltaTime;
                yield return null;
            }

            _isDash = false;
            dashEffect.SetActive(false);
            moveSpeed = _initSpeed;
        }
    }
}