using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Entity
{
    //public GameObject[] body = new GameObject[4];
    private PlayerSegmentSprite[] segSprites = new PlayerSegmentSprite[Player.body.Length];
    public GameObject tip;
    private SpriteRenderer tipSprite;
    public Vector3 snakeSpawn = new Vector3(0, 0, 0);

    public Sprite snakeHead;
    public Sprite snakeStraight;
    public Sprite snakeBent;
    public Sprite snakeTail;

    private LinkedList<Direction> directionQueue = new LinkedList<Direction>();
    private bool addedDirection = false;

    public bool isSpecialMovement;
    public static bool canMove = true;
    private bool canOpenChest = true;

    protected override void Awake()
    {
        base.Awake();

        tipSprite = tip.GetComponent<SpriteRenderer>();
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
        if (CheckCanInput())
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeDirection(Direction.right);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                ChangeDirection(Direction.down);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangeDirection(Direction.left);
            }
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                ChangeDirection(Direction.up);
            }

            //if (Input.GetKeyDown(KeyCode.LeftShift))
            //{
            //    isSprinting = true;
            //}
            isSpecialMovement = Input.GetKey(KeyCode.LeftShift);
            Player.playerWeaponManager.isSprinting = isSpecialMovement;

            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    RestartSnake();
            //}
        }
    }

    public void OnTick(int tick)
    {
        if (tick % 4 == 0 || (isSpecialMovement && tick % 2 == 0))
        {
            // move
            if (!canMove)
                return;

            // try moving in front of queue direction, else do nothing
            if (directionQueue.Count != 0 && CanMove(transform.position, directionQueue.First.Value))
            {
                currDir = directionQueue.First.Value;
                directionQueue.RemoveFirst();
            }
            // check if door/chest is in dQ.first
            if (directionQueue.Count != 0 && CheckUnlockDoor(directionQueue.First.Value))
            {
                directionQueue.RemoveFirst();
            }

            // clear queue if nothing was added
            if (!addedDirection)
                directionQueue.Clear();
            addedDirection = false;

            CheckUnlockDoor(currDir);

            // moving snake code
            if (CanMove(transform.position, currDir) || Player.playerEffects.GetExitingIndex() != -1)
            {
                MoveBody();
            }

            Player.playerEffects.UpdateMovementEffects();
        }
    }


    private void ChangeDirection(Direction dir)
    {
        // if same as last input OR queue has >=2, don't add
        if ((directionQueue.Count != 0 && directionQueue.Last.Value == dir) || directionQueue.Count >= 2)
        {
            return;
        }
        
        // if queue is empty, don't go backwards, OR if isn't empty, don't go in last queued opposite
        Direction lastDir = currDir;
        if (directionQueue.Count != 0)
            lastDir = directionQueue.Last.Value;

        if (!IsOppositeDirection(lastDir, dir))
        {
            directionQueue.AddLast(dir);
            addedDirection = true;
        }
    }

    private bool CheckUnlockDoor(Direction dir)
    {
        RaycastHit2D rh;
        Vector3 rcDir = Vector3.zero;
        switch (dir)
        {
            case Direction.right:
                rcDir = Vector3.right;
                break;
            case Direction.down:
                rcDir = Vector3.down;
                break;
            case Direction.left:
                rcDir = Vector3.left;
                break;
            case Direction.up:
                rcDir = Vector3.up;
                break;
        }
        rh = Physics2D.Raycast(transform.position, rcDir, 1, wallLayerMask);

        if (rh.collider != null && canOpenChest)
        {
            Door d = rh.collider.gameObject.GetComponent<Door>();
            //Debug.Log("checking door " + d.isLocked + "  " + d.isClosed + " " + PlayerInventory.HasKeys() + " " + PlayerInventory.keys);
            if (d != null && d.isLocked && d.isClosed && PlayerInventory.HasKeys())
            {
                PlayerInventory.AddKey(-1);
                d.UnlockDoor();

                return true;
            }

            LootActivatorTile l = rh.collider.gameObject.GetComponent<LootActivatorTile>();
            if (l != null)
            {
                canOpenChest = false;

                l.OpenLoot();

                return true;
            }
        }

        return false;
    }

    public void MoveBody()
    {
        canOpenChest = true;

        Vector3 headDir = DirectionUtil.Convert(currDir);
        if (Player.playerEffects.GetExitingIndex() > 0)
            headDir = Vector3.zero;
        Vector3 body2Dir = Player.body[1].transform.position - Player.body[0].transform.position;

        tipSprite.transform.position = Player.body[3].transform.position;
        Player.body[3].transform.position = Player.body[2].transform.position;
        Player.body[2].transform.position = Player.body[1].transform.position;
        Player.body[1].transform.position = Player.body[0].transform.position;
        transform.position += headDir;

        // setting sprites
        float headRot = Mathf.Atan2(headDir.y, headDir.x) * Mathf.Rad2Deg;

        // tip
        Vector3 tipDir = Player.body[3].transform.position - tip.transform.position;
        float tipRot = Mathf.Atan2(tipDir.y, tipDir.x) * Mathf.Rad2Deg;

        tipSprite.transform.rotation = Quaternion.Euler(0, 0, tipRot);

        // tail
        Vector3 tailBodyDir = Player.body[2].transform.position - Player.body[3].transform.position;
        float tailRot = Mathf.Atan2(tailBodyDir.y, tailBodyDir.x) * Mathf.Rad2Deg;

        segSprites[3].SetSprite(snakeTail, segSprites[2].isBent, tailRot, tailBodyDir, -tipDir);
        
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

    public void SetSnakeSpawn(Vector3 pos, Direction dir)
    {
        Player.body[0].transform.position = pos;
        for (int i = 1; i < Player.body.Length; i++)
        {
            Player.body[i].transform.position = pos - DirectionUtil.Convert(dir);
        }

        currDir = dir;
        directionQueue.Clear();
    }

    private bool CheckCanInput()
    {
        return !UIManager.stopPlayerInput && !UIEffects.stopPlayerInput;
    }

    public void RestartSnake()
    {
        currDir = Direction.right;
        directionQueue.Clear();
        for (int i = 0; i < 3; i++)
            Player.body[i].transform.position = snakeSpawn + new Vector3(-i, 0);
    }
}
