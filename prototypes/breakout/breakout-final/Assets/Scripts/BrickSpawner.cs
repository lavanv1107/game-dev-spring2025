using UnityEngine;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine.SceneManagement;

public class BrickSpawner : MonoBehaviour
{
    public static BrickSpawner Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject brickPrefab;
    public GameObject ballPrefab;

    [Header("Boundary Settings")]
    public Vector2 topLeft = new Vector2(-24.84f, 15.93f);
    public Vector2 bottomRight = new Vector2(9.34f, 1.3f);
    
    [Header("Layout Settings")]
    public float brickSpacing = 1f;
    public float rowSpacing = 1f;
    public Vector2 brickSize = new Vector2(2f, 1f);

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 180f;
    
    private int numberOfBricks;
    private int numberOfRows;
    private List<GameObject> lastRowBricks = new List<GameObject>();
    private GameObject flashingBrick;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CalculateLayout();
        SpawnBricks();
    }

    private void Update()
    {

    }

    private void CalculateLayout()
    {
        float availableWidth = bottomRight.x - topLeft.x;
        float availableHeight = topLeft.y - bottomRight.y;

        float spacePerBrick = brickSize.x + brickSpacing;
        numberOfBricks = Mathf.FloorToInt(availableWidth / spacePerBrick);

        float spacePerRow = brickSize.y + rowSpacing;
        numberOfRows = Mathf.FloorToInt(availableHeight / spacePerRow);

        float totalRowWidth = (numberOfBricks * brickSize.x) + ((numberOfBricks - 1) * brickSpacing);
    }

    private void SpawnBricks()
    {
        int[] rowDirections = new int[numberOfRows];
        for (int i = 0; i < numberOfRows; i++)
        {
            rowDirections[i] = Random.value > 0.5f ? 1 : -1;
        }

        float startX = topLeft.x;
        float startY = topLeft.y;

        for (int row = 0; row < numberOfRows; row++)
        {
            Color rowColor = Random.ColorHSV();  
            bool isLastRow = (row == numberOfRows - 1);

            for (int col = 0; col < numberOfBricks+1; col++)
            {
                SpawnSingleBrick(row, col, rowColor, rowDirections[row], new Vector2(startX, startY), isLastRow);
            }
        }
    }

    private Dictionary<float, List<GameObject>> bricksByRow = new Dictionary<float, List<GameObject>>();
    
    private void SpawnSingleBrick(int row, int col, Color color, int rowDirection, Vector2 startPos, bool isLastRow)
    {
        float x = startPos.x + col * (brickSize.x + brickSpacing);
        float y = startPos.y - row * (brickSize.y + rowSpacing);
        Vector3 position = new Vector3(x, y, 0);

        GameObject newBrick = Instantiate(brickPrefab, position, Quaternion.identity);
        
        Renderer brickRenderer = newBrick.GetComponent<Renderer>();
        if (brickRenderer != null)
        {
            brickRenderer.material.color = color;
        }

        newBrick.transform.Rotate(-120.0f, 0.0f, -188.0f, Space.Self);
        
        // Store brick in the dictionary organized by Y position
        if (!bricksByRow.ContainsKey(y))
        {
            bricksByRow[y] = new List<GameObject>();
        }
        bricksByRow[y].Add(newBrick);
        
        // Add BrickBehavior component
        BrickBehavior brickBehavior = newBrick.AddComponent<BrickBehavior>();
        brickBehavior.originalColor = color;

        if (isLastRow && col == numberOfBricks / 2)
        {
            SpawnBall(position);
        }
    }

    public void FlashRandomLastRowBrick(Color flashColor)
    {
        // Clean up null references
        CleanupBrickDictionary();
        
        // Find the lowest row that still has bricks
        float lowestRowY = float.MaxValue;
        List<GameObject> lastRowBricks = null;
        
        foreach (var row in bricksByRow)
        {
            if (row.Value.Count > 0 && row.Key < lowestRowY)
            {
                lowestRowY = row.Key;
                lastRowBricks = row.Value;
            }
        }
        
        if (lastRowBricks != null && lastRowBricks.Count > 0)
        {
            // Stop any existing flash coroutine
            if (flashingBrick != null)
            {
                var previousBrickBehavior = flashingBrick.GetComponent<BrickBehavior>();
                if (previousBrickBehavior != null)
                {
                    previousBrickBehavior.StopFlashing();
                }
            }
            
            // Select random brick from the lowest row with remaining bricks
            int randomIndex = Random.Range(0, lastRowBricks.Count);
            flashingBrick = lastRowBricks[randomIndex];
            
            // Start flashing coroutine on the brick
            var brickBehavior = flashingBrick.GetComponent<BrickBehavior>();
            if (brickBehavior != null)
            {
                brickBehavior.StartFlashing(flashColor);
            }
        }
    }

    private void CleanupBrickDictionary()
    {
        var rowsToUpdate = bricksByRow.Keys.ToList();
        foreach (var y in rowsToUpdate)
        {
            // Remove null references (destroyed bricks)
            bricksByRow[y].RemoveAll(brick => brick == null);
            
            // Remove empty rows
            if (bricksByRow[y].Count == 0)
            {
                bricksByRow.Remove(y);
            }
        }

        // Check if all bricks are gone
        if (bricksByRow.Count == 0)
        {
            Debug.Log("All bricks destroyed, loading GameWin scene");
            try
            {
                SceneManager.LoadScene("GameWin");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load GameWin scene: {e.Message}");
            }
        }
    }

    private void SpawnBall(Vector3 brickPosition)
    {
        Vector3 ballPosition = brickPosition + new Vector3(0, -2f, 0);
        Instantiate(ballPrefab, ballPosition, Quaternion.identity);
    }

    public void SpawnBallFromLastRow()
    {
        if (numberOfRows > 0)
        {
            float lastRowY = topLeft.y - ((numberOfRows - 1) * (brickSize.y + rowSpacing));
            int middleBrickIndex = numberOfBricks / 2;
            float middleX = topLeft.x + (middleBrickIndex * (brickSize.x + brickSpacing));
            Vector3 spawnPosition = new Vector3(middleX, lastRowY, 0);
            SpawnBall(spawnPosition);
        }
    }
}