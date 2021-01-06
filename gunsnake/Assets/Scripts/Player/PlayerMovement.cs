using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private enum directions
    {
        right,
        down,
        left,
        up,
    }
    private directions currDir;
    private LinkedList<directions> directionQueue = new LinkedList<directions>();
    private bool queueCleared = false;

    public int right_bound;
    public int left_bound;
    public int bottom_bound;
    public int top_bound;

    public GameObject[] body = new GameObject[3];

    //public GameClock gameClock;
    //public TetrisManager tetrisManager;
    //public FruitManager fruitManager;
    //public int fruitColleced = 0;
    //public TextMeshProUGUI fruitScoreText;
    //public SoundManager soundManager;

    public Vector3 snakeSpawn = new Vector3(0, 0, 0);

    void Start()
    {
        TimeTickSystem.OnTick_4 += TimeTickSystem_OnTick;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeDirection(directions.right);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeDirection(directions.down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeDirection(directions.left);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeDirection(directions.up);
        }
    }


    private void TickUpdate(int tick)
    {
        // move
        if (directionQueue.Count != 0)
        {
            currDir = directionQueue.First.Value;
            directionQueue.RemoveFirst();
        }
        if (!CheckIfRunsIntoSomething((int)transform.position.x, (int)transform.position.y))
            return;
        MoveBody();
        switch (currDir)
        {
            case directions.right:
                transform.position += new Vector3(1, 0);
                break;
            case directions.down:
                transform.position += new Vector3(0, -1);
                break;
            case directions.left:
                transform.position += new Vector3(-1, 0);
                break;
            case directions.up:
                transform.position += new Vector3(0, 1);
                break;
        }
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        TickUpdate(e.tick);
    }


    private void ChangeDirection(directions dir)
    {
        // if pressed key and directions don't match, reset queue
        if (directionQueue.Count != 0 && directionQueue.First.Value != currDir && !queueCleared)
        {
            directionQueue.Clear();
            queueCleared = true;
        }
        // if queue is empty, don't go backwards
        if (directionQueue.Count == 0)
        {
            if (!IsOppositeDirection(currDir, dir))
                directionQueue.AddLast(dir);
        }
        // if queue isn't empty, don't go in last queued opposite
        else if (!IsOppositeDirection(directionQueue.Last.Value, dir))
            directionQueue.AddLast(dir);
    }

    private bool IsOppositeDirection(directions d1, directions d2)
    {
        return Mathf.Abs((int)d1 - (int)d2) == 2;
    }


    public void MoveBody()
    {
        body[2].transform.position = body[1].transform.position;
        body[1].transform.position = body[0].transform.position;
        body[0].transform.position = transform.position;
    }

    public bool CheckIfRunsIntoSomething(int x, int y)
    {
        switch (currDir)
        {
            case directions.right:
                return CheckSpotIsSafe(x + 1, y);
            case directions.down:
                return CheckSpotIsSafe(x, y - 1);
            case directions.left:
                return CheckSpotIsSafe(x - 1, y);
            case directions.up:
                return CheckSpotIsSafe(x, y + 1);
        }
        return false;
    }

    public bool CheckSpotIsSafe(int x, int y)
    {
        if (x < left_bound || right_bound < x || y < bottom_bound || top_bound < y)
        {
            //gameClock.GameOver();
            Debug.Log("game over!");
            return false;
        }
        return true;
    }

    public void RestartSnake()
    {
        currDir = 0;
        directionQueue.Clear();
        transform.position = snakeSpawn;
        for (int i = 0; i < 3; i++)
            body[i].transform.position = snakeSpawn + new Vector3(-i - 1, 0);
    }
}
