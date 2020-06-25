using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTests : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GameObject cylinder = GameObject.Find("Cylinder");
        Vector3 point = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.LeftArrow)) {
            cylinder.transform.Rotate(new Vector3(0, -.5f, 0));
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            cylinder.transform.Rotate(new Vector3(0, .5f, 0));
        }

        cylinder.transform.RotateAround(point, cylinder.transform.right,
                30 * Time.deltaTime);
        //cylinder.transform.RotateAround(point, cylinder.transform.right, 0);
    }
}
