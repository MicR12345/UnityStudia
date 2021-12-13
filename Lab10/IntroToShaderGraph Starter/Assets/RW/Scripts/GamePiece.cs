/*
 * Copyright (c) 2019 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;

// controls one of the seven tangram game pieces

public class GamePiece : MonoBehaviour
{
    // difference between this Transform and mouse down position
    private Vector3 mouseDownOffset;

    // screen z
    private float zDepth;

    // rotation degrees per second
    [SerializeField]
    private float rotationSpeed = 720;

    // our desired rotation
    private Quaternion targetRotation;

    // is the GamePiece currently rotating?
    private bool isRotating = false;

    // is the GamePiece currently being moved?
    private bool isActive = false;

    // default y value 
    private const float yValue = 0.014f;

    private void OnMouseDown()
    {
        // calculate screen Z value for this GameObject
        zDepth = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        // store the offset between the mouse down position and the GameObject position
        mouseDownOffset = gameObject.transform.position - MouseToWorldPoint();
    }

    // return the mouse position as a world space position
    private Vector3 MouseToWorldPoint()
    {
        // mouse screen point
        Vector3 mouseScreenPoint = Input.mousePosition;

        // use the screen Z calculated on mouse down
        mouseScreenPoint.z = zDepth;

        // convert to world space
        return Camera.main.ScreenToWorldPoint(mouseScreenPoint);
    }

    // drag the GameObject along the xz plane
    void OnMouseDrag()
    {

        // move object with mouse
        transform.position = mouseDownOffset + MouseToWorldPoint();

        // keep GamePiece on xz plane 
        transform.position = new Vector3(transform.position.x, yValue, transform.position.z);
    }

    // set a target rotation of 15 degrees clockwise
    public void RotateCounterClockwise()
    {
        isRotating = true;

        float newY = Mathf.RoundToInt(transform.rotation.eulerAngles.y - 15f);
        targetRotation = Quaternion.Euler(0f, newY, 0f);
    }

    // set a target rotation of 15 degress counter clockwise
    public void RotateClockwise()
    {
        isRotating = true;

        float newY = Mathf.RoundToInt(transform.rotation.eulerAngles.y + 15f);
        targetRotation = Quaternion.Euler(0f, newY, 0f);
    }

    // rotate the GamePiece to its target rotation
    void RotateToTarget()
    {

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // if we are close enough, then complete the rotation
        if (Mathf.Abs(Quaternion.Angle(targetRotation, transform.rotation)) < 1f)
        {
            transform.rotation = targetRotation;
            isRotating = false;
        }
    }

    // activate the GamePiece
    public void OnMouseEnter()
    {
        isActive = true;
    }

    // deactivate the GamePiece
    public void OnMouseExit()
    {
        isActive = false;
    }

    private void Update()
    {
        // finish rotation motion
        if (isRotating)
        {
            RotateToTarget();
        }

        // trigger a CW or CCW rotation
        if (isActive && !isRotating)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                RotateCounterClockwise();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                RotateCounterClockwise();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                RotateClockwise();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                RotateClockwise();
            }
        }
    }
}
