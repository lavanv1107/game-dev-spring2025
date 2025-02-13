using UnityEngine;

public class Bricks : MonoBehaviour
{
    public GameObject brick;

    public float startX = -12f;
    public float startY = 11f;

    public int numberOfBricks = 9;
    public int numberOfRows = 4;

    public float brickSpacing = 1f;
    public float rowSpacing = 1f;

    public int brickWidth = 2;
    public int brickHeight = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnBricks();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject instance = GameObject.FindGameObjectWithTag("sizeIncreaseBrick");
        GameObject[] instances = GameObject.FindGameObjectsWithTag("brick");
        if (instances.Length == 0 && instance == null)
        {
            Time.timeScale = 0;
        }
    }

    void SpawnBricks()
    {
        int totalBricks = numberOfRows * numberOfBricks;
        int sizeIncreaseBrick = Random.Range(0, totalBricks);

        int currentBrick = 0;

        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfBricks; j++)
            {
                float x = startX + j * (brickWidth + brickSpacing);
                float y = startY - i * (brickHeight + rowSpacing);

                Vector3 position = new Vector3(x, y, 0);
                GameObject newBrick = Instantiate(brick, position, Quaternion.identity);

                if (currentBrick == sizeIncreaseBrick)
                {
                    newBrick.GetComponent<Renderer>().material.color = Color.red;
                    newBrick.tag = "sizeIncreaseBrick";
                }
                currentBrick++;
            }
        }
    }
}

