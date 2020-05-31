using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10;
    public float rotateSpeed = 10;
    Vector3 turnDirection;  //This is used to determine the alternate direction once snake hits the wall
    Vector3 currentDirection;  //This is used to determine the alternate direction once snake hits the wall
    bool hitTheWall;
    float sign = 0;
    float prevSign = 0;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        turnDirection = transform.forward;
        animator = GetComponentInChildren<Animator>();
    }
    
    private void FixedUpdate()
    {
        //Check for wall hit and depending on the direction of hit, decide the alternate direction to travel
        RaycastHit hitInfo;
        if (Physics.Raycast(gameObject.transform.position, transform.forward, out hitInfo, 0.05f))
        {
            if (hitInfo.collider.tag == "wall")
            {
                if(!hitTheWall)
                    sign = Mathf.Sign(Vector3.SignedAngle(hitInfo.normal, transform.forward, transform.up));
                hitTheWall = true;
                //Debug.Log(-sign);
                
                turnDirection = Quaternion.AngleAxis( -sign * 125, gameObject.transform.up) * transform.forward;
                
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.paused)
            return;


        //player movement and rotation logic. Very simple turn animation included
        float rotationValue = PlayerWheel.Instance.rotationValue;
        animator.SetInteger("Turn", 0);
        if (!hitTheWall)
        {
            transform.Rotate(transform.up, rotationValue * rotateSpeed * Time.deltaTime);
            transform.position += speed * Time.deltaTime * transform.forward;
        }
        else
        {
            currentDirection = Vector3.Lerp(gameObject.transform.forward, turnDirection, 0.1f);
            if(Vector3.Dot(currentDirection, gameObject.transform.forward) > 0.95f)
            {
                hitTheWall = false;
            }
            animator.SetInteger("Turn", (int)sign);

            //Debug.Log("Wall hit");
            transform.Rotate(transform.up, (40  + rotationValue)* -sign * Time.deltaTime);
            transform.position -=  0.05f * Time.deltaTime * currentDirection;
        }

    }
}
