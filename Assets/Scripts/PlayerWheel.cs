using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerWheel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static PlayerWheel Instance { get; private set; }
    private PlayerWheel instance;

    GameObject parent;
    bool pointerDown;
    float distanceFromCenter;
    Vector3 initialDirection;
    Quaternion initial;
    RectTransform rectTransform;
    RectTransform parentRectTransform;

    public float rotationValue;
    private void Start()
    {
        if (Instance == null)
        {
            instance = this;
            Instance = this;
        }
        else
            Destroy(this);

        parent = transform.parent.gameObject;
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = parent.GetComponent<RectTransform>();

        distanceFromCenter = Vector3.Distance(rectTransform.position, 
            parentRectTransform.position);

        initialDirection = rectTransform.position - parentRectTransform.position;
        initialDirection.Normalize();
       
    }
    private void Update()
    {
        //Debug.Log("Pivot: " + parentRectTransform.position);

        Vector3 lookDirection = rectTransform.position - parentRectTransform.position;
        lookDirection.Normalize();
        Vector3 directionToMove = initialDirection;

        if (pointerDown)
        {
            //Debug.Log("Mouse position: " + Input.mousePosition);
            //Debug.Log("Anchored pos: " + parentRectTransform.anchoredPosition);

            //Get the look direction and normalize it
            Vector3 mouseDirection = Input.mousePosition - parentRectTransform.position;
            
            mouseDirection.Normalize();


            directionToMove = Vector3.Lerp(lookDirection, mouseDirection, 0.75f);
        }
        else
        {
            directionToMove = Vector3.Lerp(lookDirection, initialDirection, 0.15f);
        }
        
        directionToMove.Normalize();
        transform.rotation = Quaternion.LookRotation(lookDirection, parent.transform.up);
        transform.Rotate(0, 90, 90);

        rectTransform.position = parentRectTransform.position + directionToMove * distanceFromCenter;

        rotationValue = -Mathf.Floor(Vector3.SignedAngle(initialDirection, directionToMove, parent.transform.forward) * 0.05f);
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("DOWN");
        pointerDown = true;
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Up");
        pointerDown = false;
    }
}
