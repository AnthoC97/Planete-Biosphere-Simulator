using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Private Serializable Fields
    [SerializeField]
    private float translation_speed = 10;
    [SerializeField]
    private float rotation_speed = 10;
    [SerializeField]
    private float cam_threshold_yaw = 0.3f;
    [SerializeField]
    private float cam_threshold_pitch = 0.3f;
    [SerializeField]
    private float yaw_speed = 10;
    [SerializeField]
    private float pitch_speed = 10;
    #endregion

    #region Private Fields
    private Vector3 direction;
    private Vector3 rotation;
    private Camera cam;
    private float yaw;
    private float pitch;
    private float yawChange;
    private float pitchChange;
    #endregion

    #region MonoBehavior Callback Methods
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        direction = Vector3.zero;
        rotation = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Input.GetAxis("Mouse X"));
        //Free Rotations
        if (Input.GetMouseButton(1))
        {
            Debug.Log("iciiii");
            yawChange = Input.GetAxis("Mouse X");
            pitchChange = Input.GetAxis("Mouse Y");
            yaw += yawChange * Time.deltaTime * yaw_speed;
            pitch -= pitchChange * Time.deltaTime * pitch_speed;
            rotation = new Vector3(pitch, yaw, 0);
        }
        else
        {
            Debug.Log("laaaa");
            //Move forward
            if (Input.GetKey(KeyCode.Z))
            {
                direction += Vector3.forward * Time.deltaTime * translation_speed;
            }
            //Move backward
            else if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back * Time.deltaTime * translation_speed;
            }
            //Move Left
            else if (Input.GetKey(KeyCode.Q))
            {
                direction += Vector3.left * Time.deltaTime * translation_speed;
            }
            //Move Right
            else if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right * Time.deltaTime * translation_speed;
            }
            //Move up
            else if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.up * Time.deltaTime * rotation_speed;
            }
            //Move down
            else if (Input.GetKey(KeyCode.E))
            {
                direction -= Vector3.up * Time.deltaTime * rotation_speed;
            }
        }
        transform.rotation = Quaternion.Euler(rotation);
        transform.position = direction;
        Debug.Log("euler : " + transform.eulerAngles);
    }
    #endregion

    #region Private Methods
    private static bool FastApproximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }
    #endregion
}
