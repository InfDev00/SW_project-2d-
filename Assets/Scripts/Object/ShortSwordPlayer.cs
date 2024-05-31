using System.Collections;
using UnityEngine;

namespace Object
{
    public class ShortSwordPlayer : Player
    {
        public override void Dash()
        {
            if (_dashDirection == Vector3.zero) return;
            StartCoroutine(IDash());
        }
        private IEnumerator IDash()
        {
            _isDash = true;
            _moveDirection = Vector3.zero;
            dashEffect.SetActive(true);

            float dashTime = 0f;

            while (dashTime < dashDuration)
            {
                transform.Translate(_dashDirection * (dashSpeed * Time.deltaTime));
                dashTime += Time.deltaTime;
                yield return null;
            }

            _isDash = false;
            yield return new WaitForSeconds(0.5f);
            dashEffect.SetActive(false);
        }
    }
}