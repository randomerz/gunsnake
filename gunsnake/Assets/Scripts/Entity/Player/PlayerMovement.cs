using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Entity
{
    //public GameObject[] body = new GameObject[4];
    private PlayerSegmentSprite[] segSprites = new PlayerSegmentSprite[Player.body.Length];
    public Vector3 snakeSpawn = new Vector3(0, 0, 0);

    public Sprite snakeHead;
    public Sprite snakeStraight;
    public Sprite snakeBent;
    public Sprite snakeTail;

    private LinkedList<directions> directionQueue = new LinkedList<directions>();
    private bool addedDirection = false;

    public bool isSprinting;


    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            segSprites[i] = Player.body[i].GetComponent<PlayerSegmentSprite>();
        }
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
        Player.playerWeaponManager.isSprinting = isSprinting;

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartSnake();
        }
    }

    public void OnTick(int tick)
    {
        if (tick % 4 == 0 || (isSprinting && tick % 2 == 0))
        {
            // move

            // try moving in front of queue direction, else do nothing
            if (directionQueue.Count != 0 && CanMove(transform.position, directionQueue.First.Value))
            {
                currDir = directionQueue.First.Value;
                directionQueue.RemoveFirst();
            }
            // clear queue if nothing was added
            if (!addedDirection)
                directionQueue.Clear();
            addedDirection = false;

            // moving snake code
            if (!CanMove(transform.position, currDir))
            {
                Debug.Log("bonk!");
                return;
            }
            MoveBody();

            //if (isSprinting && tick % 4 == 0 && !Input.GetKey(KeyCode.LeftShift))
            //{
            //    isSprinting = false;
            //}
        }
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
        Vector3 headDir = Vector3.zero;
        Vector3 body2Dir = Player.body[1].transform.position - Player.body[0].transform.position;

        switch (currDir)
        {
            case directions.right:
                headDir = Vector3.right;
                break;
            case directions.down:
                headDir = Vector3.down;
                break;
            case directions.left:
                headDir = Vector3.left;
                break;
            case directions.up:
                headDir = Vector3.up;
                break;
        }
        Player.body[3].transform.position = Player.body[2].transform.position;
        Player.body[2].transform.position = Player.body[1].transform.position;
        Player.body[1].transform.position = Player.body[0].transform.position;
        transform.position += headDir;

        // setting sprites
        float headRot = Mathf.Atan2(headDir.y, headDir.x) * Mathf.Rad2Deg;

        // tail
        Vector3 tailBodyDir = Player.body[2].transform.position - Player.body[3].transform.position;
        float tailRot = Mathf.Atan2(tailBodyDir.y, tailBodyDir.x) * Mathf.Rad2Deg;

        segSprites[3].SetSprite(snakeTail, segSprites[2].isBent, tailRot, tailBodyDir, Vector3.zero);
        
        // body
        bool body1ShouldBend = Vector3.Dot(headDir, body2Dir) == 0;
        float body1Rot = 0;

        segSprites[2].SetSprite(segSprites[1]);
        
        if (body1ShouldBend)
        {
            Vector3 bendDir = headDir + body2Dir;
            body1Rot = Mathf.Atan2(bendDir.y, bendDir.x) * Mathf.Rad2Deg - 45;
            // float forwardAngle = (headRot - body1Rot - 45) * 2 + body1Rot + 45
            segSprites[1].SetSprite(snakeBent, true, body1Rot, headDir, body2Dir);
        }
        else
        {
            body1Rot = Mathf.Atan2(headDir.y, headDir.x) * Mathf.Rad2Deg;
            segSprites[1].SetSprite(snakeStraight, false, body1Rot, headDir, body2Dir);
        }

        
        // head
        segSprites[0].SetSprite(snakeHead, false, headRot, Vector3.zero, -headDir);

    }

    public void RestartSnake()
    {
        currDir = directions.right;
        directionQueue.Clear();
        for (int i = 0; i < 3; i++)
            Player.body[i].transform.position = snakeSpawn + new Vector3(-i, 0);
    }
}
