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

// controls our tangram puzzle
public class Puzzle : MonoBehaviour
{
    // the array of seven GamePieces
    private GamePiece[] gamePieces;

    // game piece starting positions
    private Vector3[] originalPositions;

    // game piece starting rotations
    private Quaternion[] originalRotations;

    private void OnEnable()
    {
        // don't allow the user to edit this object accidentally
        gameObject.hideFlags = HideFlags.NotEditable;
    }

    // cache the original positions and rotation values of the GamePieces
    private void Awake()
    {
        SaveOriginalTransforms();
    }

    // store the original GamePiece transforms
    private void SaveOriginalTransforms()
    {
        gamePieces = GetComponentsInChildren<GamePiece>();
        originalPositions = new Vector3[gamePieces.Length];
        originalRotations = new Quaternion[gamePieces.Length];

        for (int i =0; i < gamePieces.Length; i++)
        {
            if (gamePieces[i] != null)
            {
                originalPositions[i] = gamePieces[i].transform.position;
                originalRotations[i] = gamePieces[i].transform.rotation;
            }
        }
    }

    // return GamePieces to their original positions
    public void ResetPieces()
    {
        for (int i = 0; i < gamePieces.Length; i++)
        {
            if (gamePieces[i] != null)
            {
                gamePieces[i].transform.position = originalPositions[i];
                gamePieces[i].transform.rotation = originalRotations[i];
            }
        }
    }

}
