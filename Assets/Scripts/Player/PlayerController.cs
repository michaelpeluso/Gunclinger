using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip landAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        [Space(15)]
        public float maxSpeed = 7;
        public float maxSprintSpeed = 15;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        
        [Space(15)]
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        /* CUSTOM VARIABLES */

        [Space(15)]
        public float playerHealth;
        public float playerEnergy;
        public float maxHealth;
        [HideInInspector] public float maxEnergy;
        public float energyPerSprint;
        public float energyPerJump;
        public float healthRegenStep;
        public float energyRegenStep;
        public float healthRegenDelay;
        public float energyRegenDelay;
        private bool RegeningHealth;
        private bool RegeningEnergy;
        private Coroutine RegeningHealthCoroutine;
        public bool canRun = true;

        private PlayerControllerPublic pcpScript;

        [Space(15)]
        public GameObject[] inv = new GameObject[3];
        public int currEqupt;
        public SceneLoader sceneLoader;

        /* CUSTOM VARIABLES */
       
        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            pcpScript = gameObject.GetComponent<PlayerControllerPublic>();
            currEqupt = 0;
            animator.SetInteger("weapon", currEqupt);
            
            pcpScript.playerHealth = playerHealth;
            pcpScript.maxHealth = maxHealth;
            maxHealth = playerHealth;
            maxEnergy = playerEnergy;
            pcpScript.energyRegenStep = energyRegenStep;
            pcpScript.healthRegenDelay = healthRegenDelay;
        }

        void Start() {
            maxHealth /= PlayerPrefs.GetInt("difficulty");
            pcpScript.maxHealth = maxHealth;
            PlayerPrefs.SetFloat("maxHealth", maxHealth);
            if (playerHealth != maxHealth) { playerHealth = maxHealth; }
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump")) {
                    jumpState = JumpState.PrepareToJump;
                    if (playerEnergy >= energyPerJump) { playerEnergy -= energyPerJump; }
                    StartCoroutine(regenDelay("energy", energyRegenDelay));
                }
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();


            CheckForPerks();
            ComputeHealthAndEnergy();
            FlipSprite();
            updateEquipt();
            SetPublicVariables();
            CheckForDeath();
        }

        void SetPublicVariables() {
            inv = pcpScript.inv;
            pcpScript.spriteIsFlipped = spriteRenderer.flipX;
            pcpScript.currEqupt = currEqupt;
            playerHealth = pcpScript.playerHealth;
            pcpScript.playerEnergy = playerEnergy;
            maxHealth = pcpScript.maxHealth;
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    else {
                        jumpState = JumpState.Grounded;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }
        

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded && playerEnergy >= energyPerJump)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (IsGrounded && velocity.x != 0) {
                if (Input.GetKey(KeyCode.LeftShift) && ((velocity.x > 0 && !spriteRenderer.flipX) || (velocity.x < 0 && spriteRenderer.flipX )) && playerEnergy > 1f + energyPerSprint && canRun) {
                    animator.SetBool("sprint", true);
                    animator.SetBool("walking", false);

                    animator.SetBool("grounded", IsGrounded);
                    animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSprintSpeed);
                    animator.SetFloat("velocityY", Mathf.Abs(velocity.y) / maxSprintSpeed);

                    targetVelocity = move * maxSprintSpeed;
                    return;
                }
                else {
                    animator.SetBool("sprint", false);
                    animator.SetBool("walking", true);
                    if ((velocity.x > 0 && spriteRenderer.flipX) || (velocity.x < 0 && !spriteRenderer.flipX )) {
                        animator.SetBool("walkingBackwards", true);
                    }
                    else {
                        animator.SetBool("walkingBackwards", false);
                    }
                }
            }
            else {
                animator.SetBool("walking", false);
                animator.SetBool("sprint", false);
            }

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            animator.SetFloat("velocityY", Mathf.Abs(velocity.y) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        void FlipSprite() {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousePos.x > gameObject.transform.position.x) {
                spriteRenderer.flipX = false;
            }
            else {
                spriteRenderer.flipX = true;
            }
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }

        void updateColliderSize() {
            BoxCollider2D bc = gameObject.GetComponent<BoxCollider2D>();
            bc.size = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;;
            bc.offset = new Vector2 ((bc.size.x / 2), bc.size.y);
        }

        void updateEquipt() {
            if (Input.mouseScrollDelta.y != 0) {
                int scrollData = (int)Input.mouseScrollDelta.y / Mathf.Abs((int)Input.mouseScrollDelta.y);

                if (inv[currEqupt]) { inv[currEqupt].SetActive(false); }

                currEqupt = (currEqupt + scrollData) % 3;
                if (currEqupt == 3) { currEqupt = 0; }
                if (currEqupt == -1) { currEqupt = 2; }

                if (inv[currEqupt] != null) { 
                    inv[currEqupt].SetActive(true);
                    animator.SetInteger("weapon", currEqupt);
                }
                else {
                    animator.SetInteger("weapon", 0);
                } 
            }
        }

        void ComputeHealthAndEnergy() 
        {

            if (canRun && animator.GetBool("sprint")) {
                if (playerEnergy < 2f) {
                    canRun = false;
                    StartCoroutine(sprintDelay(energyRegenDelay));
                }
                playerEnergy -= energyPerSprint;
                StartCoroutine(regenDelay("energy", energyRegenDelay));
                return;
            }
            if (playerEnergy != pcpScript.playerEnergy) {
                StartCoroutine(regenDelay("energy", energyRegenDelay));
            }
            
            if (RegeningEnergy && playerEnergy < 100f) {    
                if (animator.GetBool("walking")) {
                    playerEnergy += energyRegenStep / 2;
                }
                else if (GetComponent<Rigidbody2D>().velocity.x == 0 && GetComponent<Rigidbody2D>().velocity.y == 0) {
                    playerEnergy += energyRegenStep;
                }
                if (playerEnergy > maxEnergy) { playerEnergy = maxEnergy; }
            } 
        
            if (RegeningHealth && playerHealth < maxHealth && pcpScript.playerHealth >= playerHealth) {
                playerHealth += healthRegenStep;
                pcpScript.Regen(playerHealth);
                if (playerHealth > maxHealth) { playerHealth = maxHealth; }
            }
            if (playerHealth != pcpScript.playerHealth) {
                if (RegeningHealthCoroutine != null) { StopCoroutine(RegeningHealthCoroutine); }
                RegeningHealthCoroutine = StartCoroutine(regenDelay("health", healthRegenDelay));
            }
        }

        public IEnumerator regenDelay(string regenToggle, float delay) {
            if (regenToggle == "health") { 
                RegeningHealth = false;
                yield return new WaitForSeconds(delay);
                RegeningHealth = true;
            }
            else if (regenToggle == "energy") { 
                RegeningEnergy = false;
                yield return new WaitForSeconds(delay);
                RegeningEnergy = true;
            }
        }

        IEnumerator sprintDelay(float delay) {
            yield return new WaitForSeconds(delay);
            canRun = true;
        }

        void CheckForPerks() {
            if (pcpScript.energyRegenStep != energyRegenStep) {
                energyRegenStep = pcpScript.energyRegenStep;
            }
            
            if (pcpScript.healthRegenDelay != healthRegenDelay) {
                healthRegenDelay = pcpScript.healthRegenDelay;
            }
        }
    
        public void CheckForDeath() {
            if (playerHealth <= 0) {
                sceneLoader.LoadNextScene(4);
            }
        }
    }
}