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
    public Sprite snakeBodyStraight;
    public Sprite snakeBodyBent;
    public Sprite snakeTailStraight;
    public Sprite snakeTailBent;

    private LinkedList<Direction> directionQueue = new LinkedList<Direction>();
    private bool addedDirection = false;

    public bool isSpecialMovement;
    public static bool canMove = true;
    public static bool canSpecialMove = true;
    private bool canOpenChest = true;
    private bool isSprinting;
    private bool isReversing;

    // temp
    private bool didBeep = false;

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


            isSprinting = canSpecialMove && !isReversing && Input.GetKey(KeyCode.LeftShift);

            bool wasReversing = isReversing;
            isReversing = canSpecialMove && !isSprinting && Input.GetKey(KeyCode.LeftControl);

            if (isReversing != wasReversing)
            {
                if (isReversing) // start reversing
                {
                    currDir = DirectionUtil.Convert(tip.transform.position - Player.body[3].transform.position);
                }
                else // stop reversing
                {
                    currDir = DirectionUtil.Convert(transform.position - Player.body[1].transform.position);
                }
            }

            isSpecialMovement = isSprinting || isReversing;
            Player.playerWeaponManager.isSprinting = isSpecialMovement;

            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    RestartSnake();
            //}


        }
    }

    public void OnTick(int tick)
    {
        if ((!isReversing && tick % 4 == 0) || (isSprinting && tick % 2 == 0) || (isReversing && tick % 8 == 0))
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

            // moving snake code
            if (!isReversing)
            {
                CheckUnlockDoor(currDir);

                if (CanMove(transform.position, currDir) || Player.playerEffects.GetExitingIndex() != -1)
                {
                    MoveBody();
                }
            }
            else
            {
                if (CanMove(tip.transform.position, currDir))
                {
                    ReverseBody();
                }
            }

            Player.playerEffects.UpdateMovementEffects();
        }

        // sfx
        if (isReversing && tick % 8 == 0)
        {
            if (!didBeep)
            {
                didBeep = true;
                AudioManager.Play("player_reversing");
            }
            else
            {
                didBeep = false;
            }
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

            ShopActivatorTile s = rh.collider.gameObject.GetComponent<ShopActivatorTile>();
            if (s != null)
            {
                canOpenChest = false;

                s.OpenShop();

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

        if (segSprites[2].isBent)
        {
            segSprites[3].SetSprite(segSprites[2], snakeTailBent);
        }
        else
        {
            segSprites[3].SetSprite(segSprites[2], snakeTailStraight);
        }
        
        // body
        bool body1ShouldBend = Vector3.Dot(headDir, body2Dir) == 0;
        float body1Rot = 0;

        segSprites[2].SetSprite(segSprites[1]);
        
        if (body1ShouldBend)
        {
            Vector3 bendDir = headDir + body2Dir;
            body1Rot = Mathf.Atan2(bendDir.y, bendDir.x) * Mathf.Rad2Deg - 45;
            // float forwardAngle = (headRot - body1Rot - 45) * 2 + body1Rot + 45
            segSprites[1].SetSprite(snakeBodyBent, true, body1Rot, headDir, body2Dir);
        }
        else
        {
            body1Rot = Mathf.Atan2(headDir.y, headDir.x) * Mathf.Rad2Deg;
            segSprites[1].SetSprite(snakeBodyStraight, false, body1Rot, headDir, body2Dir);
        }

        
        // head
        segSprites[0].SetSprite(snakeHead, false, headRot, Vector3.zero, -headDir);

    }

    public void ReverseBody()
    {
        //AudioManager.Play("player_reversing");

        canOpenChest = true;

        //Vector3 headDir = DirectionUtil.Convert(currDir);
        Vector3 tipDir = DirectionUtil.Convert(currDir);
        //if (Player.playerEffects.GetExitingIndex() > 0)
        //    headDir = Vector3.zero;
        //Vector3 body2Dir = Player.body[1].transform.position - Player.body[0].transform.position;
        Vector3 body2TailDir = Player.body[2].transform.position - Player.body[3].transform.position;

        //tipSprite.transform.position = Player.body[3].transform.position;
        //Player.body[3].transform.position = Player.body[2].transform.position;
        //Player.body[2].transform.position = Player.body[1].transform.position;
        //Player.body[1].transform.position = Player.body[0].transform.position;
        //transform.position += headDir;
        Player.body[0].transform.position = Player.body[1].transform.position;
        Player.body[1].transform.position = Player.body[2].transform.position;
        Player.body[2].transform.position = Player.body[3].transform.position;
        Player.body[3].transform.position = tipSprite.transform.position;
        tipSprite.transform.position += tipDir;

        // setting sprites
        //float headRot = Mathf.Atan2(headDir.y, headDir.x) * Mathf.Rad2Deg;
        //float tailRot = Mathf.Atan2(tipDir.y, tipDir.x) * Mathf.Rad2Deg;


        // head
        //Vector3 tipDir = Player.body[3].transform.position - tip.transform.position;
        //float tipRot = Mathf.Atan2(tipDir.y, tipDir.x) * Mathf.Rad2Deg;
        Vector3 headDir = transform.position - Player.body[1].transform.position;
        float headRot = Mathf.Atan2(headDir.y, headDir.x) * Mathf.Rad2Deg;

        //Player.body[0].transform.rotation = Quaternion.Euler(0, 0, tipRot);
        segSprites[0].SetSprite(snakeHead, false, headRot, Vector3.zero, -headDir);


        // body
        //bool body1ShouldBend = Vector3.Dot(headDir, body2Dir) == 0;
        //float body1Rot = 0;

        //segSprites[2].SetSprite(segSprites[1]);

        //if (body1ShouldBend)
        //{
        //    Vector3 bendDir = headDir + body2Dir;
        //    body1Rot = Mathf.Atan2(bendDir.y, bendDir.x) * Mathf.Rad2Deg - 45;
        //    // float forwardAngle = (headRot - body1Rot - 45) * 2 + body1Rot + 45
        //    segSprites[1].SetSprite(snakeBent, true, body1Rot, headDir, body2Dir);
        //}
        //else
        //{
        //    body1Rot = Mathf.Atan2(headDir.y, headDir.x) * Mathf.Rad2Deg;
        //    segSprites[1].SetSprite(snakeStraight, false, body1Rot, headDir, body2Dir);
        //}

        segSprites[1].SetSprite(segSprites[2]);

        Sprite s = segSprites[3].isBent ? snakeBodyBent : snakeBodyStraight;
        segSprites[2].SetSprite(segSprites[3], s);


        // tail
        Vector3 tailBodyDir = Player.body[2].transform.position - Player.body[3].transform.position;
        Vector3 tailTipDir = tip.transform.position - Player.body[3].transform.position;
        bool tailShouldBend = Vector3.Dot(tailTipDir, tailBodyDir) == 0;
        float tailRot;
        if (tailShouldBend)
        {
            Vector3 bendDir = tailTipDir + tailBodyDir;
            tailRot = Mathf.Atan2(bendDir.y, bendDir.x) * Mathf.Rad2Deg - 45;
            segSprites[3].SetSprite(snakeTailBent, true, tailRot, tailTipDir, tailBodyDir);
        }
        else
        {
            tailRot = Mathf.Atan2(-tipDir.y, -tipDir.x) * Mathf.Rad2Deg;
            segSprites[3].SetSprite(snakeTailStraight, false, tailRot, tailTipDir, tailBodyDir);
        }

        //segSprites[3].SetSprite(snakeTail, segSprites[2].isBent, tailRot, tailBodyDir, -tipDir);


        // tip
        float tipRot = 180 + Mathf.Atan2(tipDir.y, tipDir.x) * Mathf.Rad2Deg;
        tipSprite.transform.rotation = Quaternion.Euler(0, 0, tipRot);

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
