using System;
using System.Collections;
using System.Collections.Generic;
using EffectSystem;
using ShootSystem;
using Spine.Unity;
using UnityEngine;
using SkillSystem;
using Unity.Mathematics;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class RealCharacterController : MonoBehaviour
{
    #region innerparam

    private PlayerManager pc;
    private Rigidbody2D rb;
    private Animator anim;
    private FireController fc;
    [HideInInspector]
    public SkillManager sm;
    private CDController cdController;
    [HideInInspector]
    public AfterImageEffects aie;
    [HideInInspector]
    public GameObject character;

    private AudioSource stepSource;
    private Material characMaterial;
    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock mpb;
    private Color color = Color.white;

    private AniStateInfoManager aniStateInfoManager;
    
    private UnityAction<SkillBuffArgs> hotSkillBuffAcyion;
    private UnityAction<UIButtonArgs> buttonDown, buttonUp;

    private int movementInputDirection;
    private int shootInputDirection;
    private float jumpTimer;
    private float turnTimer;
    private float wallJumpTimer;
    [HideInInspector]
    public float dashTimeLeft;
    private float lastImageXPos;
    private float lastDashTime = -100f;
    private float jumpDownRefreshTimerLeft;
    private float invincibleTimeLeft;
    private float hurtTimeLeft;
    [HideInInspector] 
    public float initMoveSpeed;
    [HideInInspector]
    public float skillDirection;

    [HideInInspector]
    public bool isFacingRight = true;
    private bool canNormalJump;
    private bool canWallJump;

    [HideInInspector]
    public int amountOfJumpsLeft;
    private int facingDirection = 1;
    private int lastWallJumpDirection;
    private int normalMask;
    private int controlIndex;
    [HideInInspector]
    public int amountOfJumps = 1;

    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isHeadOrBottomTouchingWall;
    [HideInInspector]
    public bool isWallSliding;
    private bool checkJumpMultiplier;
    private bool canMove = true;
    private bool canFlip = true;
    private bool hasWallJump;
    [HideInInspector]
    public bool isDashing;
    private bool isAttack;
    [HideInInspector]
    public bool isHurting;
    private bool isInvincible;
    [HideInInspector] 
    public bool isDead = false;
    [HideInInspector]
    public bool isSkilling = false;
    [HideInInspector] 
    public bool getSlideWall = false;
    
    #endregion

    #region outparam
    //[CustomLabel("角色名")] 
    public string characName = "";
    //[CustomLabel("移动速度")]
    public float movementSpeed = 30.0f;
    //[CustomLabel("向上跳跃力")]
    public float jumpForce = 20.0f;    
    //[CustomLabel("Y轴最大速度限制")]
    public float characterMaxVelocityY = 5.0f;
    //[CustomLabel("检测地面检测器的半径")]
    public float checkGroundRadius;
    //[CustomLabel("爬墙检测距离")]
    public float wallCheckDistance;
    //[CustomLabel("什么Layer层是地面")]
    public LayerMask whatIsGround;
    //[CustomLabel("什么Layer层是向下跳跃的平台（已弃用）")]
    public LayerMask whatIsCanJumpPlatform;
    //[CustomLabel("地面检测器")]
    public Transform checkGround;
    //[CustomLabel("爬墙检测器")]
    public Transform wallCheck;
    //[CustomLabel("爬墙检测器（头）")]
    public Transform headWallCheck;
    //[CustomLabel("爬墙检测器（脚）")]
    public Transform bottomWallCheck;
    //[CustomLabel("爬墙下滑速度")]
    public float walllSlideSpeed;
    //[CustomLabel("当横向无输入时，空中横向速度衰减")]
    public float airDragMultiplier = 0.95f;
    //[CustomLabel("当跳跃无输入时，空中跳跃速度衰减")]
    public float variableJumpHeightMultiplier = 0.5f;
    //[CustomLabel("墙跳的力")]
    public float wallJumpForce;
    //[CustomLabel("跳跃输入缓存时间")]
    public float jumpTimerSet = 0.15f;
    //[CustomLabel("墙跳转向缓存屏蔽时间")]
    public float turnTimerSet = 0.15f;
    //[CustomLabel("墙跳输入缓存时间")]
    public float wallJumpTimerSet = 0.5f;
    //[CustomLabel("冲刺时间")]
    public float dashTime;
    //[CustomLabel("冲刺速度")]
    public float dashSpeed;
    //[CustomLabel("残影生成间隔距离（未用）")]
    public float distanceBetweenImages;
    //[CustomLabel("冲刺冷却时间")]
    public float dashCoolDown;
    //[CustomLabel("下跳刷新时间（已弃用）")]
    public float jumpDownRefreshTime = 0.5f;
    //[CustomLabel("受伤无敌时间")]
    public float hurtInvincibleTime = 2f;
    //[CustomLabel("受伤无法控制时间")]
    public float hurtTime = 1f;
    //[CustomLabel("受伤击飞力")]
    public float hurtSpeed = 5;
    //[CustomLabel("技能冷却")]
    public float skillColdDown = 5f;
    //[CustomLabel("射速（已弃用）")]
    public float shootingSpeed = 1f;
    //[CustomLabel("无敌闪烁速度,需要为整数")] 
    public float invincibleSpeed = 1f;

    //[CustomLabel("墙跳方向")]
    public Vector2 wallJumpDirection;
    //[CustomLabel("受伤受力方向")]
    public Vector2 hurtDirection;
    
    #endregion

    private bool isStop = false;

    #region mono

    private void OnEnable()
    {        
        hotSkillBuffAcyion = HotSkillBuff;
        buttonDown = OnButtonDown;
        buttonUp = OnButtonUp;
        Game.Instance.EventCenter.AddEventListener(Consts.E_BUFF_FireSpeed, hotSkillBuffAcyion);
        Game.Instance.EventCenter.AddEventListener(Consts.M_Button_Down, buttonDown);
        Game.Instance.EventCenter.AddEventListener(Consts.M_Button_Up, buttonUp);
        Game.Instance.EventCenter.AddEventListener(Consts.E_StartShoot, OnStartShoot);
        Game.Instance.EventCenter.AddEventListener(Consts.C_OnSkillEnd, OnSkillEnd);
    }

    private void OnDisable()
    {
        Game.Instance.EventCenter.RemoveEventListener(Consts.E_BUFF_FireSpeed, hotSkillBuffAcyion);
        Game.Instance.EventCenter.RemoveEventListener(Consts.M_Button_Down, buttonDown);
        Game.Instance.EventCenter.RemoveEventListener(Consts.M_Button_Up, buttonUp);
        Game.Instance.EventCenter.RemoveEventListener(Consts.E_StartShoot, OnStartShoot);
        Game.Instance.EventCenter.RemoveEventListener(Consts.C_OnSkillEnd, OnSkillEnd);
    }

    public void Init(PlayerManager playerManager,CDController cc, int index)
    {
        pc = playerManager;
        controlIndex = index;
        character = this.gameObject;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        checkGround = this.transform.Find("GroundCheck");
        wallJumpDirection.Normalize();
        fc = GetComponent<FireController>();
        sm = GetComponent<SkillManager>();
        aniStateInfoManager = this.GetComponent<AniStateInfoManager>();
        //cdController = GetComponentInParent<CDController>();
        cdController = cc;
        aie = GetComponent<AfterImageEffects>();
        meshRenderer = GetComponent<MeshRenderer>();
        characMaterial = meshRenderer.material;
        mpb = new MaterialPropertyBlock();

        initMoveSpeed = movementSpeed;
        //SetAllAnimSpeed(1);
    }

    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimation();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckHurt();
        CheckInvincible();
        //CheckCanJumpPlatform();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
        CheckJump();
        CheckDash();
    }

    #endregion

    private void OnButtonDown(UIButtonArgs args)
    {
        Debug.Log(args.currentUI);
        if (args.currentUI == Consts.UIPhoneBattle)
        {
            switch (args.actionType)
            {
                case ActionType.Attack:
                    break;
                case ActionType.Dash:
                    AttempToDash();
                    break;
                case ActionType.Jump:
                    AttempToJump();
                    break;
                case ActionType.Skill:
                    AttempToSkill();
                    break;
            }
        }
    }
    
    private void OnButtonUp(UIButtonArgs args)
    {
        
    }
    
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(checkGround.position, checkGroundRadius, whatIsGround) || 
                     Physics2D.OverlapCircle(checkGround.position, checkGroundRadius, whatIsCanJumpPlatform);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        isHeadOrBottomTouchingWall =
            Physics2D.Raycast(headWallCheck.position, transform.right, wallCheckDistance, whatIsGround) ||
            Physics2D.Raycast(bottomWallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGrounded && !isSkilling && movementInputDirection == facingDirection && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CheckInput()
    {
        if (Game.Instance.InputManager.inputState != InputState.Gaming) return;
        movementInputDirection = Game.Instance.InputManager.HorizontalRaw;

        if (!isWallSliding)
            shootInputDirection = Game.Instance.InputManager.VerticalRaw;
        else
            shootInputDirection = 0;

        if (Game.Instance.InputManager.JumpPress)
        {
            AttempToJump();
        }

        if (movementInputDirection != 0)
        {
            if (!isGrounded && isTouchingWall && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }

        if (turnTimer >= 0)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && !Game.Instance.InputManager.Jump)
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }

        if (Game.Instance.InputManager.DashPress)
        {
            AttempToDash();
        }

        if (Game.Instance.InputManager.Attack)
        {
            AttempToAttack();
        }
        if (!Game.Instance.InputManager.Attack)
        {
            isAttack = false;
        }

        if (Game.Instance.InputManager.SkillPress)
        {
            AttempToSkill();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {

        }
    }

    private void AttempToJump()
    {
        if (isGrounded || (amountOfJumpsLeft > 0 && !isTouchingWall))
        {
            NormalJump();
        }
        else
        {
            jumpTimer = jumpTimerSet;
        }
    }
    
    private void CheckInvincible()
    {
        if (isInvincible)
        {
            if (invincibleTimeLeft > 0)
            {
                float factor = Mathf.Cos((hurtInvincibleTime - invincibleTimeLeft) / hurtInvincibleTime * (float)Math.PI * 5f * invincibleSpeed)*0.5f+0.5f;

                color.a = factor;
                mpb.SetColor ("_Color", color);
                meshRenderer.SetPropertyBlock(mpb);
                invincibleTimeLeft -= Time.deltaTime;
            }
            else
            {
                color = Color.white;
                mpb.SetColor ("_Color", color);
                meshRenderer.SetPropertyBlock(mpb);
                pc.ChangePlayerLayer(false);
                isInvincible = false;
            }
        }
    }
    
    #region shoot

    private void AttempToAttack()
    {
        if (!isWallSliding && Game.Instance.SaveSystem.currentSave.dialog >= 1)
        {
            isAttack = true;
        }
        else
        {
            isAttack = false;
        }
    }

    private void OnStartShoot()
    {
        float facingRight = isFacingRight ? 1f : -1f;
        fc.DoShoot(new Vector2(shootInputDirection == 0 ? facingRight : 0f, shootInputDirection));
    }
    
    #endregion
    
    #region Dash
    private void AttempToDash()
    {
        if (Time.time >= (lastDashTime + dashCoolDown))
        {
            if (!isWallSliding && !isSkilling && Game.Instance.SaveSystem.currentSave.dialog >= 1)
            {
                Game.Instance.MusicManager.PlaySound("Dash", this.gameObject, false, 1f);
                isDashing = true;
                Game.Instance.InputManager.canControl = false;
                aie.SetEnable(true);
                dashTimeLeft = dashTime;
                pc.ChangePlayerLayer(true);
                lastDashTime = Time.time;
            }
        }
    }
    
    private void CheckDash()
    {
#if UNITY_ANDROID
        (pc.uiBattle as UIPhoneBattle).SetDashCd((Time.time-lastDashTime)/dashCoolDown  > 1 ? 1f : (Time.time-lastDashTime)/dashCoolDown);
#endif
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection * (dashTimeLeft / dashTime), 0);
                dashTimeLeft -= Time.deltaTime;
            }
            if (dashTimeLeft <= 0 || isTouchingWall || isHeadOrBottomTouchingWall)
            {
                aie.SetEnable(false);
                pc.ChangePlayerLayer(false);
                isDashing = false;
                canMove = true;
                canFlip = true;
                Game.Instance.InputManager.canControl = true;
                rb.velocity = new Vector2(0, 0);
            }
        }
    }

    
    public void CheckScratch()
    {
        pc.TriggerWitchTime();
    }
    
    #endregion

    #region Jump
    
    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.1f)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if (isTouchingWall)
        {
            canWallJump = true;
        }

        if (amountOfJumpsLeft <= 0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
        }
    }
    
    private void CheckJump()
    {
        if (jumpTimer > 0)
        {
            //WallJump
            if (!isGrounded && isTouchingWall && movementInputDirection != 0)
            {
                WallJump();
            }
            else if (isGrounded)
            {
                NormalJump();
            }
            jumpTimer -= Time.deltaTime;
        }


        if (wallJumpTimer > 0)
        {
            if (hasWallJump)
            {
                hasWallJump = false;
            }
            else if (wallJumpTimer <= 0)
            {
                hasWallJump = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
    }

    private void NormalJump()
    {
        if (canNormalJump && Game.Instance.SaveSystem.currentSave.dialog >= 1)
        {
            amountOfJumpsLeft--;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            Game.Instance.MusicManager.PlaySound("Jump", this.gameObject, false);
            jumpTimer = 0;
            checkJumpMultiplier = true;
        }
    }

    private void WallJump()
    {
        if (canWallJump && getSlideWall && Game.Instance.SaveSystem.currentSave.dialog >= 1) //蹬墙跳
        {
            isWallSliding = false;
            amountOfJumpsLeft = amountOfJumps;
            amountOfJumpsLeft--;
            Game.Instance.MusicManager.PlaySound("Jump", this.gameObject, false);
            Vector2 forceAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceAdd, ForceMode2D.Impulse);
            jumpTimer = 0;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJump = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
        }
    }

    #endregion

    #region movement
    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if(!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (isGrounded)
        {
            if (Math.Abs(rb.velocity.x) <= 0.01f)
            {
                isWalking = false;
            }
            else
            {
                isWalking = true;
            }
        }
    }
    
    public void Flip()
    {
        if (!isWallSliding && canFlip && !isSkilling)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0f,180f,0f);
        }
    }

    private void ApplyMovement()
    {
        if (!isGrounded && !isWallSliding && movementInputDirection == 0 && !isHurting && !isDead)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, Mathf.Clamp(rb.velocity.y, -characterMaxVelocityY, characterMaxVelocityY));
        }
        else if(canMove && !isHurting && !isDead)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, Mathf.Clamp(rb.velocity.y, -characterMaxVelocityY, characterMaxVelocityY));
        }
        else if (isDead)
        {
            float x = Mathf.Lerp(rb.velocity.x, 0, Time.time);
            rb.velocity = new Vector2(x, Mathf.Clamp(rb.velocity.y, -characterMaxVelocityY, characterMaxVelocityY));
        }
        
        if (isWallSliding)
        {
            if (rb.velocity.y < -walllSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -walllSlideSpeed);
            }
        }
    }
    #endregion
    
    #region Hurt
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hurtFacing">敌人攻击朝向，如果在角色左边填-1， 右边填1</param>
    /// <param name="damage"></param>
    public void Hurt(int hurtFacing, int damage)
    {
        if (!isDashing && !isInvincible && !isDead && !isHurting)
        {
            if (isSkilling) isSkilling = false; //打断技能释放
            
            Game.Instance.CameraManager.DoShake(ShakeType.CameraHurtNoise, 1f, 1f, 1f);
            
            pc.GetHurt(damage);
            isHurting = true;
            isInvincible = true;
            pc.ChangePlayerLayer(true);
            hurtTimeLeft = hurtTime;
            invincibleTimeLeft = hurtInvincibleTime;
            rb.velocity = new Vector2(0f, 0f);
            Vector2 velAdd = new Vector2(hurtSpeed * hurtDirection.x * -hurtFacing, hurtSpeed * hurtDirection.y);
            rb.velocity = velAdd;
        }
    }

    private void CheckHurt()
    {
        if (isHurting)
        {
            if (hurtTimeLeft > 0)
            {
                if(!isDead) Game.Instance.InputManager.canControl = false;
                hurtTimeLeft -= Time.deltaTime;
            }
            else
            {
                isHurting = false;
                if(!isDead) Game.Instance.InputManager.canControl = true;
            }
        }
    }
    #endregion

    #region skill
    
    
    private void AttempToSkill()
    {
        if (cdController.GetNormalCD(sm.Skills[0].skillID) <= 0)
        {
            if (!isWallSliding && !isHurting && Game.Instance.SaveSystem.currentSave.dialog >= 1)
            {
                sm.StartSkillCD(sm.Skills[0].skillID);
                if (controlIndex == 0)
                {
                    Game.Instance.MusicManager.PlaySound("EnergyCannon", this.gameObject, false);
                }

                if (controlIndex == 1)
                {
                    Game.Instance.MusicManager.PlaySound("Gravity", this.gameObject, false);
                }
                skillDirection = shootInputDirection;
                isSkilling = true;
            }
        }
    }

    //响应技能开枪事件
    public void SpineBertheSkillEvent()
    {
        pc.OnSkill(controlIndex);
    }

    public void SpineIttaSkillEvent()
    {
        pc.OnSkill(controlIndex);
    }

    private void OnSkillEnd()
    {
        isSkilling = false;
    }

    public void UltimateSkillStart(int id, Transform buff)
    {
        sm.GenerateSkill(sm.JudgeSkill(id), buff.transform);
    }
    
    private void HotSkillBuff(SkillBuffArgs e)
    {
        if (e.buffType == BuffType.RifleShootSpeed && controlIndex == 0)
        {
            if (e.buffChange >= 0)
            {
                Debug.Log(e.buffChange);
                aniStateInfoManager.SetAniSpeed(anim, "ShootTree", 1, e.buffChange);
            }
            else
            {
                aniStateInfoManager.ResetAllSpeed();
            }
        }
    }

    public void SetAnimSpeed(string stateName, int layer, float speed)
    {
        aniStateInfoManager.SetAniSpeed(anim, stateName, layer, speed);
    }

    public void ResetAllAnimSpeed()
    {
        aniStateInfoManager.ResetAllSpeed();
    }
    
    #endregion

    private void UpdateAnimation()
    {
        anim.SetBool("isGrounded",isGrounded);
        anim.SetFloat("Vertical", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isWalking", isWalking);
        anim.SetFloat("Horizontal", shootInputDirection);
        anim.SetBool("isAttack", isAttack);
        anim.SetBool("isHurting", isHurting);
        anim.SetBool("isDead", isDead);
        anim.SetBool("isSkilling", isSkilling);
        anim.SetBool("isDashing", isDashing);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(checkGround.position, checkGroundRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
        Gizmos.DrawLine(headWallCheck.position, new Vector3(headWallCheck.position.x + wallCheckDistance, headWallCheck.position.y, headWallCheck.position.z));
        Gizmos.DrawLine(bottomWallCheck.position, new Vector3(bottomWallCheck.position.x + wallCheckDistance, bottomWallCheck.position.y, bottomWallCheck.position.z));
    }
}
