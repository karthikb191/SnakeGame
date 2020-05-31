using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10;
    public float rotateSpeed = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rotationValue = PlayerWheel.Instance.rotationValue;
        transform.Rotate(transform.up, rotationValue * rotateSpeed * Time.deltaTime);
        transform.position += speed * Time.deltaTime * gameObject.transform.forward;
        //Debug.Log("rotationvalue " + rotationValue);
    }
}
