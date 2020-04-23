using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private GameObject m_camera;

    void Start()
    {
        m_camera = GameObject.Find("Main Camera");
    }

    void Update()
    {
        transform.LookAt(transform.position
                + m_camera.transform.rotation * Vector3.forward,
                m_camera.transform.rotation * Vector3.up);
    }
}
