using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FigureHandler : MonoBehaviour
{
    private static float ZStep = 0.01f;
    
    //#hardcode
    const int LowerBoundX = -2;
    const int HigherBoundX = 6;
    const int LowerBoundY = -6;
    const int HigherBoundY = 9;

    private Vector3 mOffset;
    private float mZCoord;

    private MeshCollider collider;

    private void Start()
    {
        collider = gameObject.GetComponent<MeshCollider>();
    }

    void OnMouseDown()
    {
        if (GameManager.IsLevelCompleted)
        {
            return;
        }
        GameManager.FigureUpAudio.Play(0);
        mZCoord = FindObjectOfType<Camera>().WorldToScreenPoint(transform.position).z;
        mOffset = transform.position - GetMouseAsWorldPoint();
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
    }
    void OnMouseUp()
    {
        if (GameManager.IsLevelCompleted)
        {
            return;
        }
        
        //ButtonHandler.FigureDownAudio.Play(0);
        Vector3 roughVector = GetMouseAsWorldPoint() + mOffset;

        float x = (float)Math.Round(roughVector.x);
        float y = (float)Math.Round(roughVector.y);

        transform.position = GetBoundedPosition(new Vector3(x, y, - ZStep * 10));
        GameManager.NumberOfMoves++;
        GameManager.TrackProgress();
        if (IsLevelCompleted())
        {
            GameManager.LevelCompletedText.SetActive(true);
   
            GameManager.NextLevelButton.SetActive(true);
            
            GameManager.LevelCompletedAudio.Play(0);
            GameManager.CompleteLevel();
        }
        else
        {
            RearrangeFigures();
        }
        
    }
    void OnMouseDrag()
    {
        if (GameManager.IsLevelCompleted)
        {
            return;
        }
        Vector3 currentPosition = GetMouseAsWorldPoint() + mOffset;
        transform.position = GetBoundedPosition(new Vector3(currentPosition.x, currentPosition.y, -0.5f));
    }
    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;
        // z coordinate of game object on screen
        mousePoint.z = mZCoord;
        // Convert it to world points
        return FindObjectOfType<Camera>().ScreenToWorldPoint(mousePoint);
    }

    private bool IsLevelCompleted()
    {
        string backgroundAreaTagName = "backgroundArea";
        var backgroundArea = GameObject.FindWithTag(backgroundAreaTagName);
        if (backgroundArea != null)
        {
            var scale = backgroundArea.transform.localScale;
            var position = backgroundArea.transform.position;

            float startX =  position.x - scale.x / 2;
            float startY =  position.y - scale.y / 2;
            float endX =  startX + scale.x;
            float endY =  startY + scale.y;

            float sizeOfSquare = 1f;
            float stepOffset = 0.5f;

            for (float y = startY; y < endY; y += sizeOfSquare)
            {
                for (float x = startX; x < endX; x += sizeOfSquare)
                {
                    Vector3 rayCastPosition = new Vector3(x + stepOffset, y + stepOffset, -2.5f);
                    if (Physics.Raycast(rayCastPosition, Vector3.forward, out RaycastHit hit, 5.0f))
                    {
                        if (hit.collider.gameObject.CompareTag(backgroundAreaTagName))
                        {
                            return false;
                        }
                    }
                    //Debug.Log($"point {rayCastPosition.x}, {rayCastPosition.y} - {hitName}");
                }
            }

            return true;
        }

        //Debug.Log("background area not found");

        return false;
    }

    private Vector3 GetBoundedPosition(Vector3 position)
    {
        var size = collider.bounds.size;

        return new Vector3(
            position.x >= LowerBoundX
                ? (position.x + size.x) <= HigherBoundX
                    ? position.x
                    : HigherBoundX - size.x
                : LowerBoundX,
            position.y >= LowerBoundY
                ? (position.y + size.y) <= HigherBoundY
                    ? position.y
                    : HigherBoundY - size.y 
                : LowerBoundY,
            position.z);
    }

    private void RearrangeFigures()
    {
        var orderedFigures = GameManager.Figures.OrderByDescending(x => x.transform.position.z).ToList();
        for (int i = 0; i < orderedFigures.Count; i++)
        {
            var position = orderedFigures[i].transform.position;
            orderedFigures[i].transform.position = new Vector3(position.x, position.y, -ZStep * (i + 2));
        }
    }
}
