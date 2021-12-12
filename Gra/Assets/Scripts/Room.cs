using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int RoomHeight = 10;
    public int RoomWidth = 18;
    //maximum number of attempts at creating a solid platform
    public int PlatformCount = 15;
    //Solid platform size
    static public int MinPlatformSize = 3;
    public int MaxPlatformSize = 7;
    //What is considered jump range
    //Number of gaps before it becomes unreachable horizontally
    static public int JumpRangeHorizontal = 3;
    //Number of gaps before it becomes unreachable vertically
    static public int JumpRangeVertical = 3;

    static public int MaxMoveDistance = 3;

    static public int MaxNonSolidPlatformDistance = 3;

    public int MaxEnemies = 7;

    public int MaxInteractable = 4;

    public float RoomMultiplier = 1;

    WorldManager manager;
    GameObject worldHandler;

    public GameObject roomObject;
    Block[,] blocks;
    GameObject[,] blockObjects;
    List<Block> aviableBlocks;
    string roomTheme;

    List<Block> floors;
    List<Block> walls;
    List<Block> platforms;
    List<Block> interactables;
    List<Block> traps;
    //List of enemies attached to room
    List<Enemy> enemies;

    public int roomNumber;
    public Room(GameObject WorldHandler, ThemedBlockList theme, int x, int y,int roomNb,int roomSizeX,int roomSizeY)
    {
        blocks = new Block[roomSizeY, roomSizeX];
        blockObjects = new GameObject[roomSizeY, roomSizeX];
        RoomHeight = roomSizeY;
        RoomWidth = roomSizeX;
        RoomMultiplier = (RoomWidth / 18) * (RoomHeight / 10);
        PlatformCount = Mathf.FloorToInt(15f * RoomMultiplier);
        //MaxPlatformSize = Mathf.FloorToInt(7f * (RoomWidth / 18) * (RoomHeight / 10));
        MaxEnemies = Mathf.FloorToInt(7f * RoomMultiplier);
        MaxInteractable = Mathf.FloorToInt(4f * RoomMultiplier);
        worldHandler = WorldHandler;
        manager = WorldHandler.GetComponent<WorldManager>();

        roomTheme = theme.type;
        aviableBlocks = theme.blocks;

        roomNumber = roomNb;

        roomObject = new GameObject("Room " + roomTheme);
        roomObject.transform.parent = worldHandler.transform;
        roomObject.transform.localPosition = new Vector2(x + 0.5f, y + 0.5f);

        GenerateRoom();
        DrawRoom();
    }
    void DebugGenerateRoom()
    {
        List<Block> floors = FindPossibleFloors();
        for (int i = 0; i < RoomWidth; i++)
        {
            int randomFloor = UnityEngine.Random.Range(0, floors.Count);
            blocks[0, i] = floors[randomFloor];
        }
    }
    void GenerateRoom()
    {
        floors = FindPossibleFloors();
        walls = FindPossibleWalls ();
        platforms = FindPossiblePlatforms();
        interactables = FindPossibleInteractables();
        traps = FindPossibleTraps();
        //pick a random floor
        int randomFloor = UnityEngine.Random.Range(0, floors.Count);
        //pick a random wall
        int randomWall = UnityEngine.Random.Range(0, walls.Count);

        //generate wall outline around room
        for (int x = 0; x < RoomWidth; x++)
        {
            for (int y = 0; y < RoomHeight; y++)
            {
                if (x == 0 || y == 0 || x == RoomWidth - 1 || y == RoomHeight - 1) blocks[y, x] = walls[randomWall];
            }
        }
        //create door
        blocks[2, RoomWidth - 1] = null;
        CreateDoorGO(RoomWidth - 1,2);
        //create bottom floor
        for (int x = 1; x < RoomWidth; x++)
        {
            blocks[1, x] = floors[randomFloor];
        }
        int numberOfPlatforms = UnityEngine.Random.Range((PlatformCount+1)/2, PlatformCount+1);
        //attempt to create platforms
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            int posX = UnityEngine.Random.Range(1, RoomWidth - 1);
            int posY = UnityEngine.Random.Range(3, RoomHeight - 2);
            int platformSize = UnityEngine.Random.Range(MinPlatformSize, MaxPlatformSize+1);
            bool cantCreate = false;
            if (!IsInJumpRange(posX, posY, platformSize))
            {
                int moveX = CanWeMovePlatformHorizontallyToMoveRange(posX, posY, platformSize);
                posX += moveX;
                if (!IsInJumpRange(posX, posY, platformSize))
                {
                    int moveY = CanWeMovePlatformVerticallyToMoveRange(posX, posY, platformSize);
                    posY += moveY;
                    if (!IsInJumpRange(posX, posY, platformSize)) cantCreate = true;
                }
            }
            
            if (CheckForCollidingPlatforms(posX,posY,platformSize))
            {
                int moveX = CanWeMovePlatformHorizontally(posX, posY, platformSize);
                posX += moveX;
                if (CheckForCollidingPlatforms(posX, posY, platformSize))
                {
                    int moveY = CanWeMovePlatformVertically(posX, posY, platformSize);
                    posY += moveY;
                    if (CheckForCollidingPlatforms(posX, posY, platformSize)) cantCreate = true;
                }
            }
            if (!cantCreate)
            {
                CreateSolidPlatformAt(posX, posY, platformSize);
            }
        }
        //Create non-solid platforms
        CreateInteractables();
        CreateNonSolidPlatforms();
        CreateEnemies();
    }
    //True if platforms touch directly
    bool CheckForCollidingPlatforms(int x,int y,int size)
    {
        for (int i = x; i <= x + size; i++)
        {
            for (int j = y-1; j <= y+1; j++)
            {
                if (blocks[j,i] !=null)
                {
                    return true;
                }
            }
        }
        return false;
    }
    //0 or X distance to aviable spot
    int CanWeMovePlatformHorizontally(int x, int y,int size)
    {
        //Attempt moving right
        for (int i = 1; i <= MaxMoveDistance; i++)
        {
            if (x + i < RoomWidth - 1 && x+i+size<RoomWidth)
            {
                if (!CheckForCollidingPlatforms(x + i, y, size))
                {
                    return i;
                }
            }
        }
        //Attempt moving left
        for (int i = 1; i <= MaxMoveDistance; i++)
        {
            if (x - i > 1 && x - i + size > 0)
            {
                if (!CheckForCollidingPlatforms(x - i, y, size))
                {
                    return -i;
                }
            }
        }
        return 0;
    }
    //0 or Y distance to aviable spot
    int CanWeMovePlatformVertically(int x, int y, int size)
    {
        //Attempt moving down
        for (int i = 1; i <= MaxMoveDistance; i++)
        {
            if (y + i < RoomHeight - 1)
            {
                if (!CheckForCollidingPlatforms(x, y + i, size))
                {
                    return i;
                }
            }
        }
        //Attempt moving up
        for (int i = 1; i <= MaxMoveDistance; i++)
        {
            if (y - i > 2)
            {
                if (!CheckForCollidingPlatforms(x, y - i, size))
                {
                    return -i;
                }
            }
        }
        return 0;
    }
    int CanWeMovePlatformVerticallyToMoveRange(int x, int y, int size)
    {
        //Attempt moving down
        for (int i = 1; i <= MaxMoveDistance; i++)
        {
            if (y + i < RoomHeight - 1)
            {
                if (IsInJumpRange(x, y + i, size))
                {
                    return i;
                }
            }
        }
        //Attempt moving up
        for (int i = 1; i <= MaxMoveDistance; i++)
        {
            if (y - i > 2)
            {
                if (IsInJumpRange(x, y - i, size))
                {
                    return -i;
                }
            }
        }
        return 0;
    }
    int CanWeMovePlatformHorizontallyToMoveRange(int x, int y, int size)
    {
        //Attempt moving right
        for (int i = 1; i <= MaxMoveDistance; i++)
        {
            if (x + i < RoomWidth - 1 && x + i + size < RoomWidth)
            {
                if (IsInJumpRange(x + i, y, size))
                {
                    return i;
                }
            }
        }
        //Attempt moving left
        for (int i = 1; i <= MaxMoveDistance; i++)
        {
            if (x - i > 1 && x - i + size > 0)
            {
                if (IsInJumpRange(x - i, y, size))
                {
                    return -i;
                }
            }
        }
        return 0;
    }
    bool IsInJumpRange(int x, int y, int size)
    {
        if (x-2>0)
        {
            for (int j = x - 2; j < x; j++)
            {
                int i = -2;
                if (y+i>0)
                {
                    if (IsThereABlock(j, y + i) && blocks[y + i, j].canBeUsedAsFloor)
                    {
                        return true;
                    }
                }
                
                i = 2;
                if (y+i<RoomHeight)
                {
                    if (IsThereABlock(j, y + i) && blocks[y + i, j].canBeUsedAsFloor)
                    {
                        return true;
                    }
                }
            }
        }
        if (x+size+2<RoomWidth)
        {
            for (int j = x + size + 1; j < x + size + 2; j++)
            {
                int i = -2;
                if (y + i > 0)
                {
                    if (IsThereABlock(j, y + i) && blocks[y + i, j].canBeUsedAsFloor)
                    {
                        return true;
                    }
                }

                i = 2;
                if (y + i < RoomHeight)
                {
                    if (IsThereABlock(j, y + i) && blocks[y + i, j].canBeUsedAsFloor)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    void CreateSolidPlatformAt(int x,int y,int size)
    {
        Debug.Log("Attempting to create platform at: x= " + x + " ,y= " + y);
        //pick a random aviable floor
        int randomFloor = UnityEngine.Random.Range(0, floors.Count);
        for (int i = 0; i < size; i++)
        {
            blocks[y, x+i] = floors[randomFloor];
        }
    }
    void CreateNonSolidPlatforms()
    {
        for (int x = 0; x < RoomWidth-1; x++)
        {
            for (int y = 2; y < RoomHeight; y++)
            {
                if (IsBlockSolid(x,y) && x+1<RoomWidth && !IsThereABlock(x+1, y) && !IsThereABlock(x+1,y+1) && !IsThereABlock(x + 1, y - 1))
                {
                    for (int i = 2 ; i < MaxNonSolidPlatformDistance+2 && x+i<RoomWidth; i++)
                    {
                        if (IsBlockSolid(x+i,y))
                        {
                            int randomPlatform = UnityEngine.Random.Range(0, platforms.Count);
                            for (int j = x+1; j < x+i; j++)
                            {
                                if(IsBlockSolid(j,y))Debug.Log(blocks[y, j].name);
                                blocks[y, j] = platforms[randomPlatform];
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
    void CreateEnemies()
    {
        enemies = new List<Enemy>();
        List<Enemy> possibleEnemies = manager.PossibleEnemies;
        int numberOfEnemies = UnityEngine.Random.Range(0, MaxEnemies);
        for (int i = 1; i < numberOfEnemies; i++)
        {
            enemies.Add(possibleEnemies[UnityEngine.Random.Range(0, possibleEnemies.Count)]);
        }
    }
    void DrawRoom()
    {
        for (int y = 0; y < RoomHeight; y++)
        {
            for (int x = 0; x < RoomWidth; x++)
            {
                if(blocks[y,x]!=null) blockObjects[y, x] = CreateSpriteGO(ref blocks[y, x], x, y);
            }
        }
        foreach (Enemy item in enemies)
        {
            Vector2 location = FindEmptySpace();
            if (location.x == -1)
            {
                continue;
            }
            else
            {
                CreateEnemy(item,Mathf.FloorToInt(location.x), Mathf.FloorToInt(location.y));
            }
            
        }
    }
    void CreateInteractables()
    {
        if (interactables.Count==0)
        {
            return;
        }
        int numberOfInteractables = UnityEngine.Random.Range(0, MaxInteractable);
        for (int i = 0; i < numberOfInteractables; i++)
        {
            Vector2 location = FindEmptySpace();
            if (location.x == -1)
            {
                continue;
            }
            else
            {
                blocks[Mathf.FloorToInt(location.y), Mathf.FloorToInt(location.x)]
                    = interactables[UnityEngine.Random.Range(0, interactables.Count)];
            }
        }
    }
    GameObject CreateDoorGO(int x,int y)
    {
        GameObject doorObject = new GameObject("Door");
        doorObject.transform.parent = roomObject.transform;
        doorObject.transform.localPosition = new Vector2(x - RoomWidth / 2, y - RoomHeight / 2);
        doorObject.layer = 9;
        BoxCollider2D collider = doorObject.AddComponent<BoxCollider2D>();
        doorObject.AddComponent<doorScript>();
        collider.size = new Vector2(1f, 1f);
        collider.isTrigger = true;
        return doorObject;
    }
    GameObject CreateSpriteGO(ref Block block,int x,int y)
    {
        GameObject blockObject = new GameObject(block.name);
        blockObject.transform.parent = roomObject.transform;
        blockObject.transform.localPosition = new Vector2(x-RoomWidth/2, y - RoomHeight/2);
        SpriteRenderer sr = blockObject.AddComponent<SpriteRenderer>();
        sr.sprite = block.sprite;
        BoxCollider2D collider = blockObject.AddComponent<BoxCollider2D>();
        if (block.allowVerticalMovement && !block.allowHorizontalMovement)
        {
            collider.size = new Vector2(1f,0.25f);
            collider.offset = new Vector2(0f, 0.375f);
            blockObject.tag = "platform";
            blockObject.layer = 7;
        }
        else if (block.interactable)
        {
            blockObject.layer = 10;
            blockObject.transform.localScale = new Vector2(0.6f, 0.6f);
            if (block.utility.Contains("heal"))
            {
                blockObject.AddComponent<HealScript>();
            }
            if (block.utility.Contains("maxHP"))
            {
                blockObject.AddComponent<MaxHPScript>();
            }
            if (block.utility.Contains("attackSpeedIncrease"))
            {
                blockObject.AddComponent<attackSpeedIncreasescript>();
            }
            if (block.utility.Contains("damageIncrease"))
            {
                blockObject.AddComponent<damageIncreaseScript>();
            }
        }
        else
        {
            blockObject.layer = 6;
        }
        block.collider = collider;
        return blockObject;
    }

    GameObject CreateEnemy(Enemy enemy,int x,int y)
    {
        GameObject enemyObject = new GameObject(enemy.name);
        enemyObject.transform.parent = roomObject.transform;
        enemyObject.transform.localPosition = new Vector2(x - RoomWidth / 2, y - RoomHeight / 2);
        SpriteRenderer sr = enemyObject.AddComponent<SpriteRenderer>();
        sr.sprite = enemy.sprite;
        BoxCollider2D collider = enemyObject.AddComponent<BoxCollider2D>();
        if(enemy.name == "slime")
        {
            Enemy newSlime = (Enemy)enemy.Clone();
            newSlime.scoreValue = newSlime.scoreValue * manager.totalRoomMultiplier;
            newSlime.health = newSlime.health * manager.totalRoomMultiplier;
            newSlime.contactDamage = newSlime.contactDamage + manager.totalRoomMultiplier;
            slimeAI slimeAI = enemyObject.AddComponent<slimeAI>();
            slimeAI.InitializeSlime(newSlime, manager, this);
        }
        enemyObject.layer = 8;
        enemyObject.tag = "Enemy";

        return enemyObject;
    }

    List<Block> FindPossibleFloors()
    {
        List<Block> blocks = new List<Block>();
        foreach (Block item in aviableBlocks)
        {
            if (item.canBeUsedAsFloor)
            {
                blocks.Add(item);
            }
        }
        if (blocks.Count==0)
        {
            Debug.LogError("No aviable floors found");
        }
        return blocks;
    }
    List<Block> FindPossibleWalls()
    {
        List<Block> blocks = new List<Block>();
        foreach (Block item in aviableBlocks)
        {
            if (item.canBeUsedAsWall)
            {
                blocks.Add(item);
            }
        }
        if (blocks.Count == 0)
        {
            Debug.LogError("No aviable walls found");
        }
        return blocks;
    }
    List<Block> FindPossiblePlatforms()
    {
        List<Block> blocks = new List<Block>();
        foreach (Block item in aviableBlocks)
        {
            if (item.allowVerticalMovement)
            {
                blocks.Add(item);
            }
        }
        if (blocks.Count == 0)
        {
            Debug.LogError("No aviable platforms found");
        }
        return blocks;
    }
    List<Block> FindPossibleInteractables()
    {
        List<Block> blocks = new List<Block>();
        foreach (Block item in aviableBlocks)
        {
            if (item.interactable)
            {
                blocks.Add(item);
            }
        }
        if (blocks.Count == 0)
        {
            Debug.LogError("No aviable interactables found");
        }
        return blocks;
    }
    List<Block> FindPossibleTraps()
    {
        List<Block> blocks = new List<Block>();
        foreach (Block item in aviableBlocks)
        {
            if (item.utility.Contains("'trap'"))
            {
                blocks.Add(item);
            }
        }
        if (blocks.Count == 0)
        {
            Debug.LogError("No aviable traps found");
        }
        return blocks;
    }
    bool IsThereABlock(int x,int y)
    {
        if (blocks[y,x] !=null)return true;
        else return false;
    }
    bool IsBlockSolid(int x,int y)
    {
        if (!IsThereABlock(x, y)) return false;
        else
        {
            if (blocks[y, x].canBeUsedAsFloor || blocks[y, x].canBeUsedAsWall) return true;
            else return false;
        }
    }
    Vector2 FindEmptySpace()
    {
        for (int i = 0; i < 500; i++)
        {
            int x = UnityEngine.Random.Range(2, RoomWidth-1);
            int y = UnityEngine.Random.Range(3, RoomHeight-1);
            if (blocks[y,x]==null)
            {
                while (y-1>0 && blocks[y - 1, x] == null)
                {
                    y--;
                }
                return new Vector2(x, y);
            }
        }
        return new Vector2(-1, -1);
    }
    public Tuple<float,float,float,float> CalculateRoomBoundries()
    {
        return new Tuple<float, float, float, float>(
            0 - RoomWidth / 2 + roomObject.transform.position.x,
            0 - RoomHeight / 2 + roomObject.transform.position.y,
            RoomWidth - 1 - RoomWidth / 2 + roomObject.transform.position.x,
            RoomHeight - 1 - RoomHeight / 2 + roomObject.transform.position.y
            );
        
    }
}

