using UnityEngine;
using KeyMouse.MoHide;

public class Player : MonoBehaviour
{
    [SerializeField] private new Transform camera;

    [Header("Player components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;

    [Header("Parent")]
    [SerializeField] private HidingCharacter hidingCharacter;

    [Header("Character stats")]
    public float Health = 100;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float jumpForce = 10;
    private bool _onGround;
    
    [Header("Combat")]
    [SerializeField] private Weapon rightHandWeapon;
    [SerializeField] private Weapon leftHandWeapon;
    [SerializeField] private float Damage = 34;
    [SerializeField] private float attackRange = 100f; // How far the player can shoot
    [SerializeField] private LayerMask shootableLayers; // Layers the player can hit (Default, Enemy, etc.)
    [SerializeField] private AudioSource AudioSource;

    [Header("Death")]
    [SerializeField] private GameObject DeathEffect;
    [SerializeField] private float DeathTimeScaler = 0.2f;

    [Header("Audio")]
    [SerializeField] private AudioSource PlayerAudioSource;
    [SerializeField] private AudioClip DeathClip;

    private const string MOVE_AMOUNT_ANIMATION_VARIABLE = "Move amount";
    private const string JUMP_ANIMATION_VARIABLE = "Jump";
    private const string AIM_ANIMATION_VARIABLE = "Aim"; 
    private const string SHOOT_TRIGGER_VARIABLE = "Shoot"; 

    #region Performing

    private void FixedUpdate()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(vertical) + Mathf.Abs(horizontal));
        Vector3 forwardLook = new Vector3(camera.forward.x, 0, camera.forward.z);
        Vector3 moveDirection = forwardLook * vertical + camera.right * horizontal;

        Movement(moveDirection);

        //Animation
        animator.SetFloat(MOVE_AMOUNT_ANIMATION_VARIABLE, moveAmount);

        //Rotation
        moveDirection += camera.right * horizontal;
        RotationNormal(moveDirection);
    }

    private void Update()
    {
        Jump();
        HandleShooting(); 
    }

    #endregion

    #region Movement

    private void Movement(Vector3 moveDirection)
    {
        Vector3 velocityDir = moveDirection * moveSpeed;

        velocityDir.y = rb.linearVelocity.y;
        rb.linearVelocity = velocityDir;
    }

    private void RotationNormal(Vector3 rotationDirection)
    {
        Vector3 targetDir = rotationDirection;
        targetDir.y = 0;
        if (targetDir == Vector3.zero)
            targetDir = transform.forward;
        Quaternion loolDir = Quaternion.LookRotation(targetDir);
        Quaternion targetRot = Quaternion.Slerp(transform.rotation, loolDir, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRot;
    }

    private void Jump()
    {
        if (!_onGround) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    #endregion

    #region Combat

    private void HandleShooting()
    {
        // Handle aiming animation separately (optional, enables aim pose without shooting)
        

        // SHOOTING LOGIC:
        // Removed the requirement to be aiming. Now just checks for Fire button (Left Click).
        if (Input.GetButtonDown("Fire2")) 
        {
            // If you want the character to snap to aim pose when shooting, uncomment the line below:
            // animator.SetBool(AIM_ANIMATION_VARIABLE, true);

            animator.SetTrigger(SHOOT_TRIGGER_VARIABLE);
            Shoot();
        }
    }

    public void Shoot()
    {
        // 1. Play Visuals
        rightHandWeapon.PlayMuzzleFlash();
        leftHandWeapon.PlayMuzzleFlash();

        // 2. Play Audio
        AudioSource.pitch = Time.timeScale; 
        AudioSource.Play();

        // 3. Detect Hit
        Ray ray = new Ray(camera.position, camera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange, shootableLayers))
        {
            // 4. Apply Damage logic
            if (hit.collider.TryGetComponent(typeof(Enemy), out Component enemyComponent))
            {
                Enemy enemyScript = enemyComponent.GetComponent<Enemy>();
                enemyScript.Health -= Damage;
                enemyScript.CheckIfDead(); 
            }
        }
    }

    #endregion

    #region Checks

    public void CheckIfDead()
    {
        if (Health <= 0)
        {
            // Disable player
            hidingCharacter.currentObject.gameObject.SetActive(false);

            // Disable transformation
            hidingCharacter.BlockTransformation = true;

            // Play audio clip
            PlayerAudioSource.clip = DeathClip;
            PlayerAudioSource.Play();

            // Instantiate particle death effect
            Destroy(Instantiate(DeathEffect, hidingCharacter.currentObject.transform.position, Quaternion.Euler(-90, 0, 0)), 2);

            // Set time to slow down
            Time.timeScale = DeathTimeScaler;

            // Disable this script
            this.enabled = false;
        }
    }

    #endregion

    #region On ground

    private void OnCollisionStay(Collision collision)
    {
        SetJumpState(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        SetJumpState(false);
    }

    private void SetJumpState(bool onGround)
    {
        _onGround = onGround;
        animator.SetBool(JUMP_ANIMATION_VARIABLE, !onGround);
    }

    #endregion
}