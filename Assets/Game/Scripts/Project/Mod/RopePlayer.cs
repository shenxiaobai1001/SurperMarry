using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class RopePlayer : MonoBehaviour
{
    public Animator _playerAnim;
   public Rigidbody2D _playerRb;
    public GameObject collider1;
    public GameObject collider2;

    public float speed = 410f;
    public float slideDownSpeed = 410f;
    public float jumpForce = 1050;
    public bool _isOnGround;

    private static readonly int IdleB = Animator.StringToHash("Idle_b");
    private static readonly int WalkB = Animator.StringToHash("Walk_b");
    private static readonly int RunB = Animator.StringToHash("Run_b");
    private static readonly int JumpTrig = Animator.StringToHash("Jump_trig");

    private void Start()
    {
        EventManager.Instance.AddListener(Events.OnLazzerHit, OnLazzerHit);
        _playerAnim = GetComponent<Animator>();
        _playerRb = GetComponent<Rigidbody2D>();
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.OnLazzerHit, OnLazzerHit);
    }
    private void OnEnable()
    {
        OnInitCollider();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isOnGround)
        {
            OnJumpAuto();
        }
    }
    public void OnJumpAuto(bool auto = false)
    {
        Sound.PlaySound("smb_jump-small");
        _isOnGround = false;
        _playerRb.velocity = Vector3.zero;
        float jumpFor = auto ? 300 : jumpForce;
        _playerRb.AddForce(new Vector2(0f, jumpFor));
        _playerAnim.SetBool(IdleB, false);
        _playerAnim.SetBool(WalkB, false);
        _playerAnim.SetBool(RunB, false);
        _playerAnim.SetTrigger(JumpTrig);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("RopeGround"))
        {
            _isOnGround = true;
            _playerAnim.SetBool(IdleB, true);
            _playerAnim.SetBool(WalkB, true);
            _playerAnim.SetBool(RunB, true);
        }
    }

    void OnInitCollider()
    {

        if (GameStatusController.IsDaoPlayer)
        {
            _playerAnim.SetTrigger("TDao");
            collider1.SetActive(false);
            collider2.SetActive(true);
        }
        else if (GameStatusController.IsQiangPlayer)
        {
            _playerAnim.SetTrigger("TQiang");
            collider1.SetActive(false);
            collider2.SetActive(true);
        }
        else if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            _playerAnim.Play("PlayerFireBigIdle");
            collider1.SetActive(false);
            collider2.SetActive(true);
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            _playerAnim.Play("Idle_Big");
            collider1.SetActive(false);
            collider2.SetActive(true);
        }
        else
        {
            _playerAnim.Play("Idle", 0);
            collider1.SetActive(true);
            collider2.SetActive(false);
        }


    }

    void OnLazzerHit(object msg)
    {
        if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            _playerAnim.SetTrigger("redLazzer");
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            _playerAnim.SetTrigger("bigLazzer");
        }
        else
        {
            _playerAnim.SetTrigger("smallLazzer");
        }
    }
}
