using PlayerScripts;
using TMPro;
using UnityEngine;

namespace SystemScripts
{
    public class FollowPlayer : MonoBehaviour
    {
        public GameObject player;
        private float _furthestPlayerPosition;
        private float _currentPlayerPosition;

        private void Start()
        {
            if (player != null)
            {
                _currentPlayerPosition = player.transform.position.x;
            }
        }
        bool checkPlayerPos = false;
        void LateUpdate()
        {
            if (player == null)
            {
                if (PlayerController.Instance != null)
                {
                    player = PlayerController.Instance.gameObject;
                }
            }
            if (player == null) return;

            if (transform.position.x <= 1.5f&& !checkPlayerPos)
            {
                if (player.transform.position.x > 2)
                {
                    checkPlayerPos = true;
                }
                return;
            }
           if (player.transform.position.x > GameModController.Instance.OnGetLevelEndPos()
                && !GameModController.Instance.OnCheckBoosLevel()
                ) return;
            checkPlayerPos = false;
            if (GameStatusController.IsHidden && !GameStatusController.HiddenMove) return;
            float y = GameStatusController.IsHidden ? 32 : 5;
            bool isMoveForward = player.transform.position.x > transform.position.x;
            bool isMax = player.transform.position.x > PlayerController.Instance.bossPkPos.x;
            float targetX = GameStatusController.IsBossBattle && isMoveForward && isMax ? PlayerController.Instance.bossPkPos.x : player.transform.position.x;
            transform.position = Vector3.Lerp(
               transform.position,
              new Vector3(targetX, y, -10),
               30 * Time.deltaTime
           );
        }
    }
}