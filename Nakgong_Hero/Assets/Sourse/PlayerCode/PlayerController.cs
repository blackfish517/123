using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private float startSpeed;
    [SerializeField] private float shiftSpeedPlus;
    [SerializeField] private float startGravityScale;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject AttackBox;
    [Header("대검 투척")]
    [SerializeField] private GameObject Sword;
    [SerializeField] private float ChainLength;
    //땅에 닿았는지를 판별하는 class
    private PlayerCollider _playerCollider;
    //수평이동
    private float horizontal;
    //인게임 내 적용 speed
    public static float speed;
    //인게임 내 적용 점프력
    public float jumpPower;
    //점프 중/ 낙공 중 판별 bool
    private bool isNakGonging = false;
    private bool isjumping = false;
    private bool isThrowing = false;
    public static float AttackPower;
    public static float stans;
    public static string AttackMode;
    public static Vector3 PlayerPos;
    public static Quaternion PlayerRotate;
    public static float AirBonePower;
    private void Start()
    {
        PlayerPos = transform.position;
        AttackMode = "Default";
        AttackBox.SetActive(false);
        speed = startSpeed;
        _playerCollider = GetComponent<PlayerCollider>();
        Debug.Log(rigid.gravityScale);
        AttackPower = 10f;//저장 파일에서 저장된 기본값 받아오자-
        stans = 3f;//몬스터의 스탠스 수치를 얼마나 깎나/기본값
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space) && _playerCollider.isOnGround)
        {
            rigid.AddForce(Vector2.up * jumpPower);
            isjumping = true;
            StartCoroutine(GroundedChecker());
        }
        if (Input.GetKey(KeyCode.LeftShift) && !isjumping)
        {
            speed = startSpeed + shiftSpeedPlus;
            anim.SetFloat("MoveSpeed",2f);
        }
        else
        {
            if (!isjumping)
            {
                speed = startSpeed;
                anim.SetFloat("MoveSpeed", 1f);
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.rotation = new Quaternion(0f, 0f,0f,0f);
            if (!isjumping && anim.GetCurrentAnimatorStateInfo(0).IsName("LeftMove") == false)
            {
                anim.Play("LeftMove");
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.rotation = new Quaternion(0f, 180f,0f,0f);
            if (!isjumping && anim.GetCurrentAnimatorStateInfo(0).IsName("LeftMove") == false)
            {
                anim.Play("LeftMove");
            }
        }
        if(isjumping || !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Default"))
            {
                anim.enabled = false;
                anim.enabled = true;
                anim.Play("Default");
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (!_playerCollider.isOnGround)
            {
                NakGong();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Throwing();
        }
    }
    private void FixedUpdate()
    {
        PlayerPos = gameObject.transform.position;
        PlayerRotate = gameObject.transform.rotation;
        rigid.velocity = new Vector2(horizontal * speed * Time.deltaTime, rigid.velocity.y);
    }

    private void NakGong()
    {
        if (!isNakGonging && isjumping)
        {
            CameraDefaultMove.CameraposPlus = -2f;
            AttackBox.SetActive(true);
            isNakGonging = true;
            rigid.gravityScale = rigid.gravityScale += 4f;
            StartCoroutine(GroundedChecker());
        }
    }

    private void Throwing()
    {
        if (!isThrowing)
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 
                Input.mousePosition.y, 0f));
            Sword.GetComponent<Deager>().ThrowAt_withThrowRange(MousePos, ChainLength);
        }
        else
        {
            
        }
    }
    IEnumerator GroundedChecker()
    {
        //bool tempforbug = _playerCollider.isOnGround;
        yield return new WaitForSeconds(0.04f);
        Debug.Log(_playerCollider.isOnGround);
        while (!_playerCollider.isOnGround)
        {
            if (speed > 0f)
            {
                speed -= 300f * Time.deltaTime;
            }

            if (isNakGonging)
            {
                if (AttackMode == "Default")
                {
                    AirBonePower += 300f * Time.deltaTime;
                    Debug.Log(AirBonePower);
                }
            }
            yield return null;
        }
        StopCoroutine("jumpSlower");
        if (isNakGonging)
        {
            isNakGonging = false;
            if (AttackMode == "Default")
            {
                AirBonePower = 0f;
                AttackBox.SetActive(false);
            }
        }

        CameraDefaultMove.CameraposPlus = 0f;
        isjumping = false;
        speed = startSpeed;
        rigid.gravityScale = startGravityScale;
        yield break;
    }
}
