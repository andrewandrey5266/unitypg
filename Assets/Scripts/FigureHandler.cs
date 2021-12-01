using System;
using System.Linq;
using UnityEngine;
using Settings = GameManager.Settings;
public class FigureHandler : MonoBehaviour
{
    private static float ZStep = 0.01f;
    
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
        if (GameManager.CheckLevelCompleted())
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

    private Vector3 GetBoundedPosition(Vector3 position)
    {
        var size = collider.bounds.size;

        return new Vector3(
            position.x >= Settings.Bounds.MinX
                ? (position.x + size.x) <= Settings.Bounds.MaxX
                    ? position.x
                    : Settings.Bounds.MaxX - size.x
                : Settings.Bounds.MinX,
            position.y >= Settings.Bounds.MinY
                ? (position.y + size.y) <= Settings.Bounds.MaxY
                    ? position.y
                    : Settings.Bounds.MaxY - size.y 
                : Settings.Bounds.MinY,
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

public class Bounds
{
    public int MinX;
    public int MaxX;
    public int MinY;
    public int MaxY;

    public static Bounds GetCloser()
    {
        return new Bounds { MinX = -2, MaxX = 6, MinY = -6, MaxY = 9 };
    }

    public static Bounds GetFar()
    {
        return new Bounds { MinX = -4, MaxX = 8, MinY = -10, MaxY = 13 };
    }
}
