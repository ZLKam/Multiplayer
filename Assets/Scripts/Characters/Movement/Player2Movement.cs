using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using PGGE;
using Photon.Pun;

public class Player2Movement : MonoBehaviour
{
    [HideInInspector]
    public CharacterController mCharacterController;
    public Animator mAnimator;

    public float mWalkSpeed = 1.5f;
    public float mRotationSpeed = 50.0f;
    public bool mFollowCameraForward = false;
    public float mTurnRate = 10.0f;

#if UNITY_ANDROID
    public FixedJoystick mJoystick;
#endif

    private float hInput;
    private float vInput;
    private float speed;
    private bool jump = false;
    private bool attacking = false;
    public float mGravity = -30.0f;
    public float mJumpHeight = 1.0f;

    private Vector3 mVelocity = new Vector3(0.0f, 0.0f, 0.0f);

    public Canvas canvas;
    public GameObject character;

    private PhotonView mPhotonView;

    void Start()
    {
        mCharacterController = GetComponent<CharacterController>();
        mPhotonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!mPhotonView.IsMine) return;
        // get the wasd value pressed and check if spacebar is press
        HandleInputs();
        // when the character is not dying, then the character can move
        if (!mAnimator.GetBool("Dying"))
        {
            Move();
        }
        HandleAttack();

        if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Ground Movement"))
        {
            mAnimator.SetBool("Dying", false);
        }
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Die") ||
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName("DieRecover"))
        {
            mAnimator.SetBool("Dying", true);
        }
        //if (!mAnimator.GetBool("Dying"))
        //{
        //    canvasController.dieButton.onClick.RemoveAllListeners();
        //    canvasController.dieButton.onClick.AddListener(Die);
        //}
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    public void HandleInputs()
    {
        // We shall handle our inputs here.
#if UNITY_STANDALONE
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
#endif

#if UNITY_ANDROID
        hInput = 2.0f * mJoystick.Horizontal;
        vInput = 2.0f * mJoystick.Vertical;
#endif

        speed = mWalkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = mWalkSpeed * 2.0f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jump = false;
        }
    }

    public void Move()
    {
        // when character attacking, the character cannot move
        if (attacking) return;

        // when no animation, return also
        if (mAnimator == null) return;
        // this can be ignore as this if statement is for the FollowIndependentCamera
        if (mFollowCameraForward)
        {
            // rotate Player towards the camera forward.
            Vector3 eu = Camera.main.transform.rotation.eulerAngles;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.Euler(0.0f, eu.y, 0.0f),
                mTurnRate * Time.deltaTime);
        }
        // rotate the player when "A" and "D" press --> rotate left or right
        else
        {
            transform.Rotate(0.0f, hInput * mRotationSpeed * Time.deltaTime, 0.0f);
        }

        // create a normalized vector forward and the y axis value to be always 0
        Vector3 forward = transform.TransformDirection(Vector3.forward).normalized;
        forward.y = 0f;
        // character move forward or backward when "W" and "S" pressed
        // when W press, vInput is positive and then the player will move forward on the speed
        // when S press, character will move backwards
        mCharacterController.Move(forward * vInput * speed * Time.deltaTime);
        
        // this is just to reset the y position of character if character is underground
        // not really need, but just in case
        if (mCharacterController.transform.position.y < 0)
        {
            mCharacterController.transform.position = new Vector3(mCharacterController.transform.position.x, 0, mCharacterController.transform.position.z); 
        }
        // set the value of animator parameters based on the vInput
        mAnimator.SetFloat("PosX", 0);
        mAnimator.SetFloat("PosZ", vInput * speed / (2.0f * mWalkSpeed));

        // if spacebar is pressed, the character will jump
        if (jump)
        {
            Jump();
            jump = false;
        }
    }

    void Jump()
    {
        // set the y velocity to be 2 times of gravity
        mVelocity.y += Mathf.Sqrt(mJumpHeight * -2f * mGravity);
        mCharacterController.Move(mVelocity * Time.deltaTime);
    }

    void ApplyGravity()
    {
        // apply gravity
        mVelocity.y += mGravity * Time.deltaTime;
        // when character is on the ground, stop continuously adding gravity
        if (!jump && mCharacterController.transform.position.y <= 0)
            mVelocity.y = 0f;
        mCharacterController.Move(mVelocity * Time.deltaTime);
    }

    void HandleAttack()
    {
        if (Input.GetButton("Fire1"))
        {
            // if the pointer is on any ui object
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            // set character is attacking, so the character will not move
            attacking = true;
            mAnimator.SetBool("Attack1", true);
        }
        else
        {
            mAnimator.SetBool("Attack1", false);
        }

        // same as Fire1
        if (Input.GetButton("Fire2"))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            attacking = true;
            mAnimator.SetBool("Attack2", true);
        }
        else
        {
            mAnimator.SetBool("Attack2", false);
        }
        // when no attacking button pressing, attacking to false, then the character can start to move
        if (!Input.GetButton("Fire1") && !Input.GetButton("Fire2"))
        {
            attacking = false;
        }
    }

    //void Die()
    //{
    //    // when character is not "dying"
    //    if (!mAnimator.GetBool("Dying"))
    //    {
    //        // run the die animation and change the text of the button
    //        mAnimator.SetTrigger("Die");
    //        canvasController.dieButton.GetComponentInChildren<TextMeshProUGUI>().text = "Revive";
    //    }
    //    else if (canvasController.dieButton.GetComponentInChildren<TextMeshProUGUI>().text == "Revive")
    //    {
    //        // when the text is revive --> character is dying
    //        // run revive animation
    //        // and change button text back
    //        mAnimator.SetTrigger("Revive");
    //        canvasController.dieButton.GetComponentInChildren<TextMeshProUGUI>().text = "Die";
    //    }
    //}
}
