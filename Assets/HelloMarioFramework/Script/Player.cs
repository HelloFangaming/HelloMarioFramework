/*
 *  Copyright (c) 2024 Hello Fangaming
 *
 *  Use of this source code is governed by an MIT-style
 *  license that can be found in the LICENSE file or at
 *  https://opensource.org/licenses/MIT.
 *  
 * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace HelloMarioFramework
{
    public class Player : MonoBehaviour
    {

        //Components
        public static Player singleton;
        private Rigidbody myRigidBody;
        private Animator animator;
        private AudioSource audioPlayer;
        private Transform cameraTransform;
        private TransformSwapper faceController;
        private VoicePack voicePack;

        //Audio clips
        [SerializeField]
        private AudioClip jumpSFX;
        [SerializeField]
        private AudioClip specialJumpSFX;
        [SerializeField]
        private AudioClip flipSFX;
        [SerializeField]
        private AudioClip poundSFX;
        [SerializeField]
        private AudioClip hurtSFX;

        //Input
        [SerializeField]
        public InputActionReference jumpAction;
        [SerializeField]
        private InputActionReference crouchAction;
        [SerializeField]
        private InputActionReference movementAction;

        //Death y value
        [SerializeField]
        private float deathPlane;

        //Z azis lock
        [SerializeField]
        private bool zAxisLock = false;
        private float zLockValue = 0f;

        //Hat attach transform
        [SerializeField]
        public Transform hatAttachTransform;

        //Game
        private int health = 3;
        private bool onGround = true;
        private bool isCrouch = false;
        private bool groundPound = false;
        private bool wallJump = false;
        private bool wallPush = false;
        private int jumpAnim = 0;
        private Vector3 direction;
        private bool variableJump = false;
        private bool forceVariableJump = false;
        private bool controlsEnabled = true;
        private float yPrevious;
        private float gravity = 0f;
        private float poundBugFix = 0f;
        private bool poundHack = false;
        private bool hurtCooldown = false;
        private bool dead = false;
        private bool autoWallJump = false;
        private bool hatOn = false;
        private Transform hat = null;
        private Propeller hatPropeller = null;
        private bool hasDoubleJump = true;
        private int jumpButtonAlt = 0;
        private Vector3 prevVelocity;
        private bool disableFloorSnap = false;

        //Moving platform handler
        private GameObject mover;
        private Vector3 moverPrev;
        private float moverRotPrev;
        private int collisionCount = 0;
        private int wallCount = 0;
        private int pushCount = 0;
        private int poundCount = 0;
        private bool speedCap = true;

        //Animator hash values
        private static int speedHash = Animator.StringToHash("Speed");
        private static int groundHash = Animator.StringToHash("onGround");
        private static int crouchHash = Animator.StringToHash("Crouch");
        private static int poundHash = Animator.StringToHash("Pound");
        private static int jumpHash = Animator.StringToHash("Jump");
        private static int wallJumpHash = Animator.StringToHash("WallJump");
        private static int wallPushHash = Animator.StringToHash("WallPush");

        //Button checker
        private bool jumpDown = false;
        private bool jumpPress = false;
        private bool jumpRelease = false;
        private bool crouchDown = false;
        private bool crouchPress = false;

        //Rumble support
        private Rumbler rumble;

        void Awake()
        {
            singleton = this;
#if UNITY_EDITOR
            //Null check
            SaveData.NullCheck();

            //Give the dev a warning if there are multiple cameras in scene, or the camera is set up incorrectly
            CinemachineBrain brain = null;
            foreach (Camera c in FindObjectsByType<Camera>(FindObjectsSortMode.None))
            {
                if (brain == null) brain = c.GetComponent<CinemachineBrain>();
                if (brain == null)
                {
                    Debug.LogWarning("Hello Mario Framework: Extra camera found at " + c.transform.position + ". Please delete this camera!");
                    if (UnityEditor.EditorUtility.DisplayDialog("Hello Mario Framework", "Extra camera found at " + c.transform.position + ". Please delete this camera!", "Select GameObject", "Ignore"))
                    {
                        UnityEditor.Selection.activeGameObject = c.gameObject;
                        UnityEditor.EditorGUIUtility.PingObject(c.gameObject.GetInstanceID());
                    }
                }
            }
            if (brain == null)
            {
                Debug.LogWarning("Hello Mario Framework: Your scene is not set up correctly. Place down a Control prefab! (Assets > HelloMarioFramework > Prefab > Menu)");
                UnityEditor.EditorUtility.DisplayDialog("Hello Mario Framework", "Your scene is not set up correctly. Place down a Control prefab! (Assets > HelloMarioFramework > Prefab > Menu)", "Ok");
            }
#endif
        }
        
        void Start()
        {
            //Add a hide near camera object
            gameObject.AddComponent<HideNearCamera>();

            myRigidBody = GetComponent<Rigidbody>();
            animator = transform.GetChild(0).GetComponent<Animator>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
            cameraTransform = Camera.main.transform;
            faceController = transform.GetChild(0).GetComponent<TransformSwapper>();
            voicePack = GetComponent<VoicePack>();

            yPrevious = transform.position.y;
            mover = new GameObject();

            //For hub only, move to respawn point
            if (LoadingScreen.IsHubScene())
            {
                //If position is set, move to position
                if (SaveData.hubPositionSet)
                {
                    transform.position = SaveData.save.GetHubPosition();
                    transform.rotation = SaveData.save.GetHubRotation();
                }
                //If position is not set, set default position
                else
                {
                    SaveData.hubPositionSet = true;
                    SaveData.save.SetHubPosition(transform.position);
                    SaveData.save.SetHubRotation(transform.rotation);
                }
            }

            //Handle checkpoints
            if (SaveData.checkpoint)
            {
                transform.position = SaveData.checkpointPos;
                transform.rotation = SaveData.checkpointRot;
            }

            jumpAction.action.Enable();
            crouchAction.action.Enable();
            movementAction.action.Enable();
            movementAction.action.Reset(); //Fix camera facing wrong direction when holding direction at start

            //Handle gamepad rumble
            rumble = gameObject.AddComponent<Rumbler>();

            if (FreeLookHelper.singleton != null)
                FreeLookHelper.singleton.WarpCameraFix(transform.position);

            myRigidBody.freezeRotation = true;

            if (zAxisLock) zLockValue = transform.position.z;
        }

        //Plysics update
        void FixedUpdate()
        {

            //Handle input
            if (jumpAction.action.IsPressed() && !jumpDown) jumpPress = true;
            if (!jumpAction.action.IsPressed() && jumpDown) jumpRelease = true;
            if (crouchAction.action.IsPressed() && !crouchDown) crouchPress = true;

            //Remember previous values
            jumpDown = jumpAction.action.IsPressed();
            crouchDown = crouchAction.action.IsPressed();

            //Ignore jump presses for alt jump button functionality
            if (jumpButtonAlt > 0)
            {
                jumpButtonAlt--;
                jumpPress = false;
            }

            //Make sure mover is not destroyed
            if (mover == null)
            {
                onGround = false;
                mover = new GameObject();
            }

            //Hack to make sure you stay off the ground when jumping/bouncing
            if (disableFloorSnap) onGround = false;

            //Force upright
            ForceUpright();

            //Direction
            if (controlsEnabled && !isCrouch && !wallJump)
            {
                Vector2 dirInput = GetInputDirection();
                if (zAxisLock) direction = new Vector3(dirInput.x, 0f, 0f);
                else direction = new Vector3(dirInput.x, 0f, dirInput.y);
            }
            else direction = Vector3.zero;

            //Change drag when on ground
            if (onGround && !variableJump)
            {
                if (direction.sqrMagnitude < 0.0001f && new Vector3(myRigidBody.velocity.x, 0f, myRigidBody.velocity.z).sqrMagnitude < 4f)
                    myRigidBody.drag = 100f;
                else if (direction.sqrMagnitude > 0.0001f)
                    myRigidBody.drag = 2f;
                else myRigidBody.drag = 8f; //v1.0: 6f
            }
            else if (!onGround)
            {
                myRigidBody.drag = 0f;
                mover.transform.parent = null;
                mover.transform.localScale = Vector3.one;
            }

            //Rotate
            if (controlsEnabled && onGround && direction.sqrMagnitude > 0.0001f)
            {
                Vector3 rot = new Vector3(myRigidBody.velocity.x, 0f, myRigidBody.velocity.z);
                if (rot.sqrMagnitude > 1f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rot), 25f * Time.fixedDeltaTime);
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z)), 5f * Time.fixedDeltaTime);
                }
            }

            //If controls are enabled
            if (controlsEnabled)
            {
                //Movement

                //Acceleration multiplier to use
                float accel = Vector3.Dot(direction, new Vector3(myRigidBody.velocity.x, 0, myRigidBody.velocity.z).normalized) * -12.5f * Time.fixedDeltaTime + (37.5f * Time.fixedDeltaTime);
                if (!onGround) accel *= 0.5f;

                //Speed cap
                Vector2 mvmntSpeed = new Vector2(myRigidBody.velocity.x + direction.x * accel, myRigidBody.velocity.z + direction.z * accel);
                if (speedCap && mvmntSpeed.sqrMagnitude > 81f)
                {
                    mvmntSpeed.Normalize();
                    mvmntSpeed = mvmntSpeed * 9f;
                }
                else if (!speedCap)
                {
                    Vector2 anotherCheck = new Vector2(mvmntSpeed.x, mvmntSpeed.y);
                    mvmntSpeed.Normalize();
                    mvmntSpeed = mvmntSpeed * 18f;
                    if (anotherCheck.sqrMagnitude < mvmntSpeed.sqrMagnitude) mvmntSpeed = anotherCheck;
                }

                //Reenable speed cap
                if (!speedCap && mvmntSpeed.sqrMagnitude <= 81f)
                    speedCap = true;

                //Walking
                myRigidBody.velocity = new Vector3(mvmntSpeed.x, myRigidBody.velocity.y, mvmntSpeed.y);

                //Crouching
                if (onGround && crouchDown) isCrouch = true;
                else if (!crouchDown || !onGround) isCrouch = false;

                //Propeller hat double jump
                if (hasDoubleJump && !onGround && jumpPress && !wallJump && hatPropeller != null)
                {
                    hasDoubleJump = false;
                    variableJump = true;
                    forceVariableJump = true;

                    Vector3 v = new Vector3(myRigidBody.velocity.x, 0f, myRigidBody.velocity.z);

                    hatPropeller.Rotate();
                    StartCoroutine(JumpAnim(1));
                    PlayRandomSound(voicePack.jumpVoiceSFX);
                    myRigidBody.velocity = v + hatPropeller.propellerJumpVector;
                }

                //Jumping
                else if (onGround && jumpPress)
                {
                    onGround = false;
                    variableJump = true;
                    forceVariableJump = true;
                    myRigidBody.drag = 0f;

                    StopCoroutine(DisableFloorSnap());
                    StartCoroutine(DisableFloorSnap());

                    audioPlayer.PlayOneShot(jumpSFX);
                    transform.position += Vector3.up * 0.1f;

                    Vector3 v = new Vector3(myRigidBody.velocity.x, 0f, myRigidBody.velocity.z);
                    float i = v.sqrMagnitude;

                    //Crouch jumps
                    if (isCrouch)
                    {

                        //Long jump
                        if (i > 8f && GetInputDirection().sqrMagnitude > 0.25f)
                        {
                            speedCap = false;
                            StartCoroutine(JumpAnim(6));
                            Vector3 dirSpeed = transform.forward * 18f;
                            myRigidBody.velocity = new Vector3(dirSpeed.x, 12f, dirSpeed.z);
                        }

                        //Backflip
                        else
                        {
                            StartCoroutine(JumpAnim(5));
                            myRigidBody.velocity = v + Vector3.up * 18f - transform.forward * 5f;
                        }

                        PlayRandomSound(voicePack.specialJumpVoiceSFX);
                        audioPlayer.PlayOneShot(specialJumpSFX);

                    }

                    //Side flip (Negative dot product)
                    else if (Vector3.Dot(direction, v) < 0f)
                    {
                        StartCoroutine(JumpAnim(4));
                        PlayRandomSound(voicePack.specialJumpVoiceSFX);
                        audioPlayer.PlayOneShot(specialJumpSFX);
                        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
                        myRigidBody.velocity = v + Vector3.up * 18f + transform.forward * 5f;
                    }

                    //Normal jump
                    else
                    {
                        StartCoroutine(JumpAnim(1));
                        PlayRandomSound(voicePack.jumpVoiceSFX);
                        if (i > 64f) myRigidBody.velocity = v + Vector3.up * 16f;
                        else myRigidBody.velocity = v + Vector3.up * 15f;
                        forceVariableJump = false;
                    }

                }

                //End variable jumping
                else if (variableJump && ((jumpRelease && !forceVariableJump) || transform.position.y < yPrevious)) variableJump = false;

                //Ground pound
                if (!onGround && crouchPress && !wallJump && !groundPound && (!variableJump || jumpAnim == 0))
                {
                    StartCoroutine(Pound());
                }

                //Wall jump
                if (wallJump && jumpPress)
                {
                    wallJump = false;
                    variableJump = true;
                    forceVariableJump = false;
                    autoWallJump = true;
                    myRigidBody.drag = 0f;
                    StartCoroutine(JumpAnim(1));
                    audioPlayer.PlayOneShot(jumpSFX);
                    PlayRandomSound(voicePack.jumpVoiceSFX);
                    myRigidBody.velocity = Vector3.up * 16f - transform.forward * 9f;
                    transform.rotation = Quaternion.LookRotation(-transform.forward);
                }

            }

            //Handle death plane
            if (!dead && transform.position.y < deathPlane)
            {
                audioPlayer.PlayOneShot(voicePack.fallVoiceSFX);
                cameraTransform.GetComponent<CinemachineBrain>().enabled = false;
                Kill();
            }

            //Change gravity to simulate variable jumping
            if (groundPound && !onGround)
            {
                if (poundHack) gravity = 1200f;
                else gravity = 0f; //Ground pound hack
            }
            else if (collisionCount > 6) gravity = 0f;
            else if (variableJump) gravity = 30f;
            else gravity = 75f;

            //Raycast and move down if needed
            if (onGround && !disableFloorSnap && collisionCount < 8) SnapToFloor();

            //Handle gravity artifically + cap speed
            if (!myRigidBody.isKinematic)
                myRigidBody.velocity = new Vector3(myRigidBody.velocity.x, Mathf.Max(myRigidBody.velocity.y - (gravity * Time.fixedDeltaTime), -24f), myRigidBody.velocity.z);

            //Prevent player from getting stuck in ground pound state
            if (groundPound && poundHack)
            {
                if (Mathf.Abs(transform.position.y - poundBugFix) < 23 * Time.fixedDeltaTime)
                {
                    poundCount--;
                    if (poundCount <= 0) StartCoroutine(PoundEnd());
                }
                else poundCount = 4;
                poundBugFix = transform.position.y;
            }

            //Collision delay
            if (collisionCount > 0)
            {
                if (!onGround) collisionCount = 0;
                else
                {
                    collisionCount--;
                    if (collisionCount == 0) onGround = false;
                }
            }

            //Wall delay
            if (wallCount > 0)
            {
                if (!wallJump) wallCount = 0;
                else
                {
                    wallCount--;
                    if (wallCount == 0) wallJump = false;
                }
            }

            //Push delay
            if (pushCount > 0)
            {
                if (!wallPush) pushCount = 0;
                else
                {
                    pushCount--;
                    if (pushCount == 0 || isCrouch) wallPush = false;
                }
            }

            //Moving platform handler
            if (onGround && mover.transform.parent != null)
            {
                transform.position += mover.transform.position - moverPrev;
                mover.transform.position = transform.position;
                moverPrev = transform.position;
            }

            //Handle z axis lock
            if (zAxisLock)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, zLockValue);
                myRigidBody.velocity = new Vector3(myRigidBody.velocity.x, myRigidBody.velocity.y, 0f);
            }

            //Remember previous y
            yPrevious = transform.position.y;

            //Remember previous velocity
            prevVelocity = myRigidBody.velocity;

            //Reset buttons
            jumpPress = false;
            jumpRelease = false;
            crouchPress = false;

        }

        //Before draw calls
        private void LateUpdate()
        {
            Vector2 i = new Vector2(myRigidBody.velocity.x, myRigidBody.velocity.z);
            animator.SetFloat(speedHash, i.magnitude);
            animator.SetBool(groundHash, onGround);
            animator.SetBool(crouchHash, isCrouch);
            animator.SetBool(poundHash, groundPound);
            animator.SetInteger(jumpHash, jumpAnim);
            animator.SetBool(wallJumpHash, wallJump);
            animator.SetBool(wallPushHash, wallPush);
        }

        //Bugfix
        void OnCollisionEnter(Collision collision)
        {
            OnCollisionStay(collision);
        }

        //Collision with rigid body
        void OnCollisionStay(Collision collision)
        {
            //All collisions
            foreach (ContactPoint contact in collision.contacts)
            {
                //If collision was from below
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.6f)
                {
                    if (!onGround)
                    {
                        moverPrev = transform.position;
                        mover.transform.position = moverPrev;
                        forceVariableJump = false;
                        autoWallJump = false;
                        if (!disableFloorSnap && myRigidBody.velocity.y <= 0f) myRigidBody.velocity = new Vector3(prevVelocity.x, 0f, prevVelocity.z);
                    }
                    else
                    {
                        transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + mover.transform.eulerAngles.y - moverRotPrev, transform.eulerAngles.z));
                    }
                    moverRotPrev = mover.transform.eulerAngles.y;
                    mover.transform.parent = contact.otherCollider.transform;
                    onGround = true;
                    variableJump = false;
                    collisionCount = 8;
                    hasDoubleJump = true;
                }

                //If collision was from on front
                else if (!groundPound && !hurtCooldown && Vector3.Dot(contact.normal, transform.forward) < -0.8f)
                {
                    //Wall push
                    if (onGround)
                    {
                        //Prioritize crouching
                        if (!isCrouch)
                        {
                            wallPush = true;
                            pushCount = 8;
                            transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(-contact.normal).eulerAngles.y, 0f);
                        }
                    }
                    //Wall jump
                    else if (!onGround && (wallJump || autoWallJump || Vector3.Dot(contact.normal, direction) < -0.5f))
                    {
                        myRigidBody.velocity = new Vector3(-contact.normal.x, myRigidBody.velocity.y, -contact.normal.z);
                        wallJump = true;
                        wallCount = 8;
                        transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(-contact.normal).eulerAngles.y, 0f);
                    }
                }

                //Allow the first initial wall jump as long as you are moving in the right direction
                else if (!groundPound && !hurtCooldown && !onGround && !wallJump && !autoWallJump && !forceVariableJump && Vector3.Dot(contact.normal, direction) < -0.5f)
                {
                    transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(-contact.normal).eulerAngles.y, 0f);
                }

                //Dampen ceiling collisions
                if (Vector3.Dot(contact.normal, Vector3.down) > 0.6f && prevVelocity.y > 0f)
                {
                    myRigidBody.velocity = new Vector3(prevVelocity.x, 0f, prevVelocity.z);
                }
            }
        }

        //Get imput direction relative to camera
        private Vector2 GetInputDirection()
        {
            Vector2 dir = movementAction.action.ReadValue<Vector2>();
            if (dir.sqrMagnitude > 1) dir.Normalize();
            return Rotate(dir, -cameraTransform.eulerAngles.y);
        }

        private Vector2 Rotate(Vector2 aPoint, float aDegree)
        {
            return Quaternion.Euler(0f, 0f, aDegree) * aPoint;
        }

        private void ForceUpright()
        {
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }

        //Raycast and snap to floor
        private void SnapToFloor()
        {
            RaycastHit hit;
            if (myRigidBody.SweepTest(Vector3.down, out hit, 0.25f, QueryTriggerInteraction.Ignore))
            {
                transform.position += Vector3.down * hit.distance;
            }
        }

        public void PlaySound(AudioClip clip)
        {
            audioPlayer.PlayOneShot(clip);
        }

        public void PlayRandomSound(AudioClip[] a)
        {
            audioPlayer.PlayOneShot(a[Random.Range(0, a.Length)]);
        }

        public IEnumerator JumpAnim(int i)
        {
            jumpAnim = i;
            yield return new WaitForSeconds(0.2f);
            if (jumpAnim == i) jumpAnim = 0;
        }

        public IEnumerator Pound()
        {
            groundPound = true;
            controlsEnabled = false;
            variableJump = false;
            myRigidBody.velocity = Vector3.zero;
            audioPlayer.PlayOneShot(flipSFX);
            StartCoroutine(JumpAnim(7));

            poundHack = false;
            yield return new WaitForSeconds(0.3f);
            poundHack = true;
            poundBugFix = transform.position.y + 24f;
            poundCount = 4;
            myRigidBody.velocity = Vector3.down * 24f;
        }

        public IEnumerator PoundEnd()
        {
            poundHack = false;
            myRigidBody.velocity = Vector3.zero;
            audioPlayer.PlayOneShot(poundSFX);
            yield return new WaitForSeconds(0.25f);
            if (!dead)
            {
                groundPound = false;
                if (!hurtCooldown) controlsEnabled = true;
            }
        }

        //The part of the ground pound when you're moving downwards to smash stuff
        public bool IsPound()
        {
            return (groundPound && poundHack && !dead);
        }

        //The beginning of the ground pound where player is spinning before the drop
        public bool IsPoundStart()
        {
            return (groundPound && !poundHack && !dead);
        }

        public void UndoPound()
        {
            if (groundPound && !dead)
            {
                StopCoroutine(Pound());
                StopCoroutine(PoundEnd());
                groundPound = false;
                poundHack = false;
                if (!hurtCooldown) controlsEnabled = true;
            }
        }

        public bool IsDead()
        {
            return dead;
        }

        public void BounceUp(float amount)
        {
            onGround = false;
            transform.position += Vector3.up * 0.25f;
            StopCoroutine(DisableFloorSnap());
            StartCoroutine(DisableFloorSnap());
            wallJump = false;
            if (!hurtCooldown)
            {
                if (jumpDown) variableJump = true;
                StartCoroutine(JumpAnim(1));
            }
            myRigidBody.drag = 0f;
            myRigidBody.velocity = new Vector3(myRigidBody.velocity.x, amount, myRigidBody.velocity.z);
        }

        public void CollectItemVoice()
        {
            audioPlayer.PlayOneShot(voicePack.collectItemVoiceSFX);
        }

        public void EnableControls(bool b)
        {
            controlsEnabled = b;
        }

        public int GetHealth()
        {
            return health;
        }

        public void Heal()
        {
            health++;
            if (health > 3) health = 3;
        }

        //Take damage
        public void Hurt(bool burn, Vector3 knockback)
        {
            if (!hurtCooldown && !dead)
            {
                if (health > 1)
                {
                    health--;
                    StartCoroutine(HurtTimer());
                    StartCoroutine(JumpAnim(8));
                    audioPlayer.PlayOneShot(hurtSFX);
                    if (burn) audioPlayer.PlayOneShot(voicePack.burnVoiceSFX);
                    else audioPlayer.PlayOneShot(voicePack.hurtVoiceSFX);
                    Knockback(knockback);
                    rumble.StartRumble(0.5f);
                }
                else
                {
                    PlayRandomSound(voicePack.dieVoiceSFX);
                    myRigidBody.drag = 0f;
                    myRigidBody.velocity = Vector3.zero;
                    myRigidBody.detectCollisions = false;
                    myRigidBody.isKinematic = true; //Prevent weird bugs when dying
                    groundPound = true;
                    Kill();
                }
                //Hurt face
                if (faceController != null) faceController.Change(2);
            }
        }

        public void Knockback(Vector3 knockback)
        {
            if (Vector3.Dot(knockback, Vector3.down) > 0.9f)
            {
                onGround = false;
                autoWallJump = false;
                myRigidBody.drag = 0f;
                transform.position += Vector3.up * 0.1f;
                myRigidBody.velocity = new Vector3(myRigidBody.velocity.x, 9f, myRigidBody.velocity.z);
            }
            myRigidBody.velocity += knockback * -9f;
        }

        //For dash panels and bullies
        public void BreakSpeedCap()
        {
            speedCap = false;
        }

        public bool CanAcceptHat()
        {
            return !hatOn;
        }

        //Whether player can be chased by enemies
        public bool CanBeChased(Vector3 otherPosition, float distance)
        {
            return (enabled && (controlsEnabled != groundPound) && Vector3.Distance(otherPosition, transform.position) < distance);
        }

        //Make another transform look towards the player
        public void LookAtMe(Transform looker)
        {
            looker.LookAt(new Vector3(transform.position.x, looker.position.y, transform.position.z));
        }

        public void AcceptHat(Transform h, Propeller propeller)
        {
            hatOn = true;
            hat = h;
            if (propeller != null) hatPropeller = propeller;
            hat.parent = hatAttachTransform;
            hat.localScale = Vector3.one;
            hat.localPosition = new Vector3(0f, 0.25f, 0f);
            hat.localRotation = Quaternion.Euler(180f, 90f, 0f);
        }

        public void RemoveHat()
        {
            if (hatOn)
            {
                hatOn = false;
                Respawner r = hat.GetComponent<Respawner>();
                if (r != null) r.RespawnThis(); //Respawn hat if it has a respawner
                Destroy(hat.gameObject);
                hat = null;
                hatPropeller = null;
            }
        }

        //Place this in OnTriggerStay
        //Check if this is true to disable jump button to make jump button do something else, like talking to NPCs
        public bool AltJumpButtonCheck()
        {
            if (onGround) jumpButtonAlt = 4;
            return (onGround && controlsEnabled && enabled);
        }

        //Stop mover from being used
        public void GetOffGround()
        {
            onGround = false;
        }

        //If rumbler needs to be used
        public void Rumble(float t)
        {
            rumble.StartRumble(t);
        }

        //Temporarily disable and enable physics and collisions
        public void DisablePhysics()
        {
            enabled = false;
            myRigidBody.velocity = Vector3.zero;
            myRigidBody.detectCollisions = false;
            myRigidBody.isKinematic = true;

            animator.SetFloat(speedHash, 0f);
            prevVelocity = Vector3.zero;
        }
        public void EnablePhysics(Vector3 vel)
        {
            enabled = true;
            myRigidBody.detectCollisions = true;
            myRigidBody.isKinematic = false;
            myRigidBody.velocity = vel;
        }

        //Switch between 3D and 2D modes (Warp box)
        public void LockZAxis(bool b)
        {
            zAxisLock = b;
            if (zAxisLock) zLockValue = transform.position.z;
        }

        public AudioClip GetStartVoiceClip()
        {
            return voicePack.startSFX;
        }

        private IEnumerator HurtTimer()
        {
            wallJump = false;
            variableJump = false;
            hurtCooldown = true;
            controlsEnabled = false;
            yield return new WaitForSeconds(1.25f);
            if (!dead)
            {
                controlsEnabled = true;
                yield return new WaitForSeconds(1f);
                hurtCooldown = false;

                //Default face
                if (faceController != null) faceController.Change(0);
            }
        }

        private IEnumerator DisableFloorSnap()
        {
            disableFloorSnap = true;
            yield return new WaitForSeconds(0.2f);
            disableFloorSnap = false;
        }

        //Instakill
        public void Kill()
        {
            if (!dead)
            {
                dead = true;
                health = 0;
                hurtCooldown = true;
                StartCoroutine(JumpAnim(9));
                controlsEnabled = false;
                MusicControl.singleton.Death();
                rumble.StartRumble(1f);

                //Prevent hat coin spawner coins after death
                if (hat != null)
                {
                    HatCoinSpawner spawner = hat.GetComponent<HatCoinSpawner>();
                    if (spawner != null) Destroy(spawner);
                }
            }
        }

        //Victory pose
        public void Victory(bool levelClear)
        {
            UndoPound();
            dead = true;
            health = 3;
            hurtCooldown = true;
            variableJump = false;
            jumpAnim = 10;
            controlsEnabled = false;

            audioPlayer.PlayOneShot(voicePack.victoryVoiceSFX);
            myRigidBody.velocity = new Vector3(0f, myRigidBody.velocity.y, 0f);
            transform.LookAt(new Vector3(cameraTransform.position.x, transform.position.y, cameraTransform.position.z));

            //Level clear
            if (levelClear)
            {
                //Camera zoom
                if (FreeLookHelper.singleton != null)
                    FreeLookHelper.singleton.VictoryZoom();

                RemoveHat();
            }

            //Continue later
            else StartCoroutine(VictoryEnd());

            //Happy face
            if (faceController != null) faceController.Change(1);

        }

        //End short victory pose
        private IEnumerator VictoryEnd()
        {
            yield return new WaitForSeconds(4f);
            dead = false;
            jumpAnim = 0;
            controlsEnabled = true;

            //Default face
            if (faceController != null) faceController.Change(0);

            //Prevent getting hurt right after victory pose
            yield return new WaitForSeconds(0.1f);
            hurtCooldown = false;
        }

    }
}
