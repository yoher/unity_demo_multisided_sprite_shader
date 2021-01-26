﻿/**
 * Based on OpenXcell-Studio script shared on http://answers.unity.com/answers/1257534/view.html
 * Enriched by YHX
 * Not production ready ;)
 */

using UnityEngine;
using System.Collections;

public class CameraControllerDragMouseOrbit : MonoBehaviour {
    public Transform target;
    public float distance = 2.0f;
    public float xSpeed = 20.0f;
    public float ySpeed = 20.0f;
    public float yMinLimit = -90f;
    public float yMaxLimit = 90f;
    public float distanceMin = 10f;
    public float distanceMax = 10f;
    public float Friction = 2f;
    public bool ZoomToPreventOcclusion = false;

    float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;
    float velocityX = 0.0f;
    float velocityY = 0.0f;


    // Use this for initialization
    void Start() {
        Vector3 angles = transform.eulerAngles;
        rotationYAxis = angles.y;
        rotationXAxis = angles.x;
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>()) {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }
    void LateUpdate() {
        if (target) {

            if (Input.GetKey(KeyCode.Escape)) {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }

            // Hide and lock cursor when right mouse button pressed
            if (Input.GetMouseButtonDown(0)) {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            // Unlock and show cursor when right mouse button released
            if (Input.GetMouseButtonUp(0)) {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if (Input.GetMouseButton(0)) {
                velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
                velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
            }
            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;
            rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
            Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);


            if (ZoomToPreventOcclusion) {
                RaycastHit hit;
                if (Physics.Linecast(target.position, transform.position, out hit)) {
                    distance -= hit.distance;
                }
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * Friction);
            velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * Friction);
        }
    }
    public static float ClampAngle(float angle, float min, float max) {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}