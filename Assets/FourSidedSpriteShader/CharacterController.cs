/**
 * Not production ready ;)
 * YHX
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    public float MaxSpeed = 5f;
    public float Acceleration = 5f;
    public float GroundFriction = 1f;
    public float SpriteWalkFPS = 5f;


    private Vector3 speed;
    private Vector3 accelerationDirection;
    private bool isMoving = false;
    private bool wasMovingLastFrame = true;  // init trigger

    private Transform cameraTransform;
    private Quaternion cameraYRotation;

    private Material spriteMaterial;

    private Renderer spriteRenderer;
    private MaterialPropertyBlock spriteMaterialPropertyBlock;

    void Start() {
        cameraTransform = Camera.main.transform;
        spriteRenderer = GetComponentInChildren<Renderer>();
        spriteMaterialPropertyBlock = new MaterialPropertyBlock();
    }

    void Update() {

        cameraYRotation = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0);  // Get camera rotation around Y only


        // Collect input and sum corresponding 'impulses'
        accelerationDirection = Vector3.zero;

        if (Input.GetKey("w")) {
            accelerationDirection += cameraYRotation * Vector3.forward;
        }
        if (Input.GetKey("s")) {
            accelerationDirection += cameraYRotation * Vector3.back;
        }
        if (Input.GetKey("a")) {
            accelerationDirection += cameraYRotation * Vector3.left;
        }
        if (Input.GetKey("d")) {
            accelerationDirection += cameraYRotation * Vector3.right;
        }

        // Compute speed
        speed += accelerationDirection.normalized * Acceleration * Time.deltaTime;  // Add the acceleration given by controls
        speed -= GroundFriction * speed * Time.deltaTime * (accelerationDirection.magnitude > 0 ? 1 : 2);  // Slow down due to friction (double when no user input to have good breaks without drifting when turning)
        speed = Vector3.ClampMagnitude(speed, MaxSpeed);  // Ensure we don't break the speed of light whatever happens

        // Put the player to sleep if the velocity becomes small
        if (speed.magnitude > 0.1f) {
            isMoving = true;
        } else {
            speed = Vector3.zero;
            isMoving = false;
        }

        // Move! Finally!
        transform.position += speed * Time.deltaTime;

        // Update the character object direction according to last user input (face last requested acceleration)
        if (accelerationDirection.magnitude > 0) {  
            transform.rotation = Quaternion.LookRotation(accelerationDirection);
        } 

        // Play the sprite walk animation only when moving
        if (isMoving != wasMovingLastFrame) {
            spriteMaterialPropertyBlock.SetFloat("_AnimFPS", isMoving ? SpriteWalkFPS : 0);  // Mathf.Floor(SpriteWalkFPS * speed.magnitude / MaxSpeed));
            spriteRenderer.SetPropertyBlock(spriteMaterialPropertyBlock);
        }
        wasMovingLastFrame = isMoving;
    }
}
