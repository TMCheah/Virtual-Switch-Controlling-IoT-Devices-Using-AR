using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dragController : MonoBehaviour
{
    private bool left, right, up, down = false;
    private float deadZone = 0.2f;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private Vector2 direction;
    public int Scene;
    
    void Update()
    {
        left = right = up = down = false;

        #region Standalone Inputs

        //Touch touch = Input.GetTouch(0);
        if (Input.GetMouseButtonDown(0))
        {
            startPoint = new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);
        }
        if (Input.GetMouseButtonUp(0))
        {
            endPoint = new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);
            direction = new Vector2(endPoint.x - startPoint.x, endPoint.y - startPoint.y);

            if (direction.magnitude < deadZone)
                return;

            //horizontal
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                    right = true;
                else
                    left = true;

            }
            else
            {
                if (direction.y > 0)
                    up = true;
                else
                    down = true;
            }
        }
        #endregion

        #region Mobile Inputs
        if (Input.touches.Length > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                startPoint = new Vector2(touch.position.x / (float)Screen.width, touch.position.y / (float)Screen.height);
            }
            if (touch.phase == TouchPhase.Ended)
            {
                endPoint = new Vector2(touch.position.x / (float)Screen.width, touch.position.y / (float)Screen.height);
                direction = new Vector2(endPoint.x - startPoint.x, endPoint.y - startPoint.y);

                if (direction.magnitude < deadZone)
                    return;

                //horizontal
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    if (direction.x > 0)
                        right = true;
                    else
                        left = true;

                }
                else
                {
                    if (direction.y > 0)
                        up = true;
                    else
                        down = true;
                }

            }
        }
        #endregion

    }

    public bool swipeLeft { get { return left; } }
    public bool swipeRight { get { return right; } }
    public bool swipeUp { get { return up; } }
    public bool swipeDown { get { return down; } }


}
