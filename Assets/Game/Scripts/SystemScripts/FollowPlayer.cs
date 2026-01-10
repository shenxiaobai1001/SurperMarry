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
            checkPlayerPos = false;
            if (GameStatusController.IsHidden &&!GameStatusController.HiddenMove) return;
            float y = GameStatusController.IsHidden ? 32 : 5;

            transform.position = Vector3.Lerp(
               transform.position,
               !GameStatusController.IsBossBattle
                ? new Vector3(player.transform.position.x, y, -10)
                : new Vector3(PlayerController.Instance.bossPkPos.x, 5, -10),
               30 * Time.deltaTime
           );
        }
    }
}