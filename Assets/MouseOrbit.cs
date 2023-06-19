using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbit : MonoBehaviour
{
    public Vector3 target;
    
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

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

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
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