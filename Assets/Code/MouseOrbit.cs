using UnityEngine;
using System.Collections;
using System;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbit : MonoBehaviour
{
    public Vector3 target;
    
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float followSmoothing = 0.01f;
    public bool overrideAllowPan = true;
    public bool allowPan = true;
    public bool drawDebug = true;
    public float panSpeed;
    public Transform selectionIndicator;


    public float yMinLimit = -80f;
    public float yMaxLimit = 80f;

    public float distance = 5.0f;
    public float distanceMin, distanceMax;
    public float scrollSpeed = 1.0f;
    public float viewScale = 0.5f;

    public Vector2 firstMousePosition;
    public int unlockThreshold = 5;
    public bool spinUnlocked = false;

    float x = 0.0f;
    float y = 0.0f;

    private Vector3 wantedTarget;

    private void Awake()
    {
        Game.Events.disableMovement += disableMovment;
        Game.Events.enableMovement += enableMovement;
        Game.Events.sysButtonClicked += sysButtonClicked;
    }

    private void sysButtonClicked(eds.System system)
    {
        wantedTarget = system.position;
    }

    private void disableMovment()
    {
        overrideAllowPan = false;
    }

    private void enableMovement()
    {
        overrideAllowPan = true;
    }

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    public void SetTarget(Vector3 input)
    {
        allowPan = false;
        wantedTarget = input;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            firstMousePosition = Input.mousePosition;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (!spinUnlocked)
            {
                if (Vector2.Distance(Input.mousePosition, firstMousePosition) >= unlockThreshold)
                {
                    spinUnlocked = true;
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            spinUnlocked = false;
        }


        if (spinUnlocked)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        }

        y = ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);

        viewScale += -Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * viewScale;
        viewScale = Mathf.Clamp(viewScale, 0.001f, 1);

        distance = Mathf.Lerp(distanceMin, distanceMax, viewScale);

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target;

        transform.rotation = rotation;
        transform.position = position;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles);

        //Paning via WASD
        if (overrideAllowPan)
        {
            if (allowPan)
            {
                Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z) * Input.GetAxis("forward/back") * panSpeed * Time.deltaTime;
                Vector3 right = new Vector3(transform.right.x, 0, transform.right.z) * Input.GetAxis("left/right") * panSpeed * Time.deltaTime;
                Vector3 up = Vector3.up * Input.GetAxis("up/down") * panSpeed * Time.deltaTime; ;

                wantedTarget += forward + right + up;

                if (drawDebug)
                {
                    Debug.DrawRay(target, forward * 5, Color.magenta);
                    Debug.DrawRay(target, right * 5, Color.yellow);
                    Debug.DrawRay(target, up * 5, Color.cyan);
                }
            }
            else
            {
                allowPan = Vector3.Distance(target, wantedTarget) < 0.01f;
            }
        }

        //Smooth follow target
        target = Vector3.Lerp(target, wantedTarget, followSmoothing * Time.deltaTime);
        selectionIndicator.position = wantedTarget;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}