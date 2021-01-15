using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Entity
{
    public GameObject[] body = new GameObject[3];
    public Vector3 snakeSpawn = new Vector3(0, 0, 0);

    private LinkedList<directions> directionQueue = new LinkedList<directions>();
    private bool addedDirection = false;

    public bool isSprinting;

    void Start()
    {
        TimeTickSystem.OnTick += TimeTickSystem_OnTick;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeDirection(directions.right);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeDirection(directions.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeDirection(directions.left);
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeDirection(directions.up);
        }

        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    isSprinting = true;
        //}
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartSnake();
        }
    }


    private void TickUpdate(int tick)
    {
        if (tick % 4 == 0 || (isSprinting && tick % 2 == 0))
        {
            // move

            // try moving in front of queue direction, else do nothing
            if (directionQueue.Count != 0 && !IsWallAhead(transform.position, directionQueue.First.Value))
            {
                currDir = directionQueue.First.Value;
                directionQueue.RemoveFirst();
            }
            // clear queue if nothing was added
            if (!addedDirection)
                directionQueue.Clear();
            addedDirection = true;

            // moving snake code
            if (IsWallAhead(transform.position, currDir))
            {
                Debug.Log("bonk!");
                return;
            }
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

            //if (isSprinting && tick % 4 == 0 && !Input.GetKey(KeyCode.LeftShift))
            //{
            //    isSprinting = false;
            //}
        }
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        TickUpdate(e.tick);
    }


    private void ChangeDirection(directions dir)
    {
        // if same as last input OR queue has >=2, don't add
        if ((directionQueue.Count != 0 && directionQueue.Last.Value == dir) || directionQueue.Count >= 2)
        {
            return;
        }
        
        // if queue is empty, don't go backwards, OR if isn't empty, don't go in last queued opposite
        directions lastDir = currDir;
        if (directionQueue.Count != 0)
            lastDir = directionQueue.Last.Value;

        if (!IsOppositeDirection(lastDir, dir))
        {
            directionQueue.AddLast(dir);
            addedDirection = true;
        }
    }

    public void MoveBody()
    {
        body[2].transform.position = body[1].transform.position;
        body[1].transform.position = body[0].transform.position;
        body[0].transform.position = transform.position;
    }

    public void RestartSnake()
    {
        currDir = directions.right;
        directionQueue.Clear();
        transform.position = snakeSpawn;
        for (int i = 0; i < 3; i++)
            body[i].transform.position = snakeSpawn + new Vector3(-i - 1, 0);
    }
}
