using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WorldManager : MonoBehaviour
{
    public Camera cam;
    public GameObject Player;
    public Player playerData;
    //List of all added blocks
    public List<Block> PossibleBlocks;
    //List of all loaded block types
    List<string> BlockTypes;
    List<ThemedBlockList> ThemedBlockLists;
    List<Room> rooms;
    public List<Enemy> PossibleEnemies;

    public PlayerMovement playerMovement;

    public GameObject ScorePointDisplayGO;
    TextMeshPro scoreTextMesh;

    public GameObject HealthSliderGO;

    public GameObject Curtain;
    public Animator CurtainAnimator;

    int currentRoom = 0;

    public List<AudioClip> audioClips;

    public GameObject HPCounterObject;

    TextMeshProUGUI HPCounter;

    public GameObject ASCounterObject;
    public GameObject ADCounterObject;
    public GameObject LifeCounterObject;

    TextMeshProUGUI ASCounter;
    TextMeshProUGUI ADCounter;
    TextMeshProUGUI LifeCounter;

    AudioSource audioSo;
    [SerializeField]
    Slider healthSlider;

    //Szybkie dodanie paskow zycia dla przeciwnikow
    public GameObject enemyHpBarPre;
    public GameObject enemyLifeTextPre;
    public GameObject enemyBarSlider;

    public Camera MainCamera;

    public int maxRoomSizeX;
    public int maxRoomSizeY;

    public float totalRoomMultiplier = 1f;
    void Start()
    {
        playerMovement = Player.GetComponent<PlayerMovement>();
        scoreTextMesh = ScorePointDisplayGO.GetComponent<TextMeshPro>();

        ASCounter = ASCounterObject.GetComponent<TextMeshProUGUI>();
        ADCounter = ADCounterObject.GetComponent<TextMeshProUGUI>();
        LifeCounter = LifeCounterObject.GetComponent<TextMeshProUGUI>();
        HPCounter = HPCounterObject.GetComponent<TextMeshProUGUI>();

        healthSlider = HealthSliderGO.GetComponent<Slider>();
        CurtainAnimator = Curtain.GetComponent<Animator>();
        audioSo = this.GetComponent<AudioSource>();

        SortOutThemesOfBlocks();
        rooms = new List<Room>();

        //rooms.Add(new Room(this.gameObject, ThemedBlockLists[UnityEngine.Random.Range(0, ThemedBlockLists.Count)], 0, 0));
        rooms.Add(new Room(this.gameObject, ThemedBlockLists[0], 0, 0,currentRoom + 1,
            UnityEngine.Random.Range(18,maxRoomSizeX), UnityEngine.Random.Range(10, maxRoomSizeY)));

        Tuple<float, float, float, float> boundries = rooms[currentRoom].CalculateRoomBoundries();
        Player.transform.position = new Vector2(boundries.Item1 + 1f, boundries.Item2 + 1.5f);

        totalRoomMultiplier = totalRoomMultiplier + rooms[currentRoom].RoomMultiplier;
        UpdateHpBar();
        UpdateUIs();
        CurtainAnimator.Play("LiftCurtain");
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        if (playerData.Health<=0)
        {
            playerMovement.InputsCleanup();
            SceneManager.LoadScene("MainMenu");
        }
        if (playerData.invincibilityTimer>0)
        {
            playerData.invincibilityTimer = playerData.invincibilityTimer - Time.deltaTime;
        }
        if (playerData.attackCooldownTimer>0)
        {
            playerData.attackCooldownTimer = playerData.attackCooldownTimer - Time.deltaTime;
        }
    }
    public void MoveToNextRoom()
    {
        CurtainAnimator.Play("DropCurtain");
        
        PlayerIncreaseScore(currentRoom + 1);
        rooms[currentRoom].roomObject.SetActive(false);
        currentRoom++;
        rooms.Add(new Room(this.gameObject, ThemedBlockLists[0], 0, 0,currentRoom+1,
            UnityEngine.Random.Range(18, maxRoomSizeX), UnityEngine.Random.Range(10, maxRoomSizeY)));
        totalRoomMultiplier = totalRoomMultiplier + rooms[currentRoom].RoomMultiplier;
        Tuple<float, float, float, float> boundries = rooms[currentRoom].CalculateRoomBoundries();
        Player.transform.position = new Vector2(boundries.Item1 + 1f, boundries.Item2 + 1.5f);
        AudioSource.PlayClipAtPoint(audioClips[UnityEngine.Random.Range(0, 2)], Player.transform.position);
        CurtainAnimator.Play("LiftCurtain");
    }
    void CreateListOfBlockTypes()
    {
        BlockTypes = new List<string>();
        foreach (Block item in PossibleBlocks)
        {
            if (!BlockTypes.Contains(item.type))
            {
                BlockTypes.Add(item.type);
            }
        }
    }
    void SortOutThemesOfBlocks()
    {
        CreateListOfBlockTypes();
        ThemedBlockLists = new List<ThemedBlockList>();
        foreach (string item in BlockTypes)
        {
            ThemedBlockLists.Add(new ThemedBlockList(item, new List<Block>()));
        }
        foreach (Block item in PossibleBlocks)
        {
            for (int i = 0; i < ThemedBlockLists.Count; i++)
            {
                if (item.type == ThemedBlockLists[i].type) ThemedBlockLists[i].blocks.Add(item);
            }
        }
    }
    public void PlayerTakeDamage(float damage,bool fromLeft)
    {
        if (playerData.invincibilityTimer<=0)
        {
            playerData.invincibilityTimer = playerData.invincibilityTime;
            playerData.Health = playerData.Health - damage;
            UpdateHpBar();
            playerMovement.TakeDamage(fromLeft);
            AudioSource.PlayClipAtPoint(audioClips[UnityEngine.Random.Range(3, 6)], Player.transform.position);
        }
        
    }
    public void UpdateUIs()
    {
        ADCounter.text = playerData.ADincreasedCounter.ToString();
        ASCounter.text = playerData.ASincreasedCounter.ToString();
        LifeCounter.text = playerData.MaxHPincreasedCounter.ToString();
    }
    public void UpdateHpBar()
    {
        healthSlider.value = playerData.Health / playerData.MaxHealth;
        HPCounter.text = Mathf.CeilToInt(playerData.Health).ToString() + "/" + Mathf.CeilToInt(playerData.MaxHealth).ToString();
    }
    public void PlayerIncreaseScore(float score)
    {
        playerData.score = playerData.score + score;
        scoreTextMesh.text = playerData.score.ToString();
    }
    void MoveCamera()
    {
        Tuple<float, float, float, float> boundries = rooms[currentRoom].CalculateRoomBoundries();
        float CameraX = Player.transform.position.x;
        float CameraY = Player.transform.position.y;
        if (CameraX<boundries.Item1 + 8.5f)
        {
            CameraX = boundries.Item1 + 8.5f;
        }
        if (CameraX > boundries.Item3 - 8.5f)
        {
            CameraX = boundries.Item3 - 8.5f;
        }
        if (CameraY < boundries.Item2 + 4.5f)
        {
            CameraY = boundries.Item2 + 4.5f;
        }
        if (CameraY > boundries.Item4 - 4.5f)
        {
            CameraY = boundries.Item4 - 4.5f;
        }
        Vector3 newCamPos = new Vector3(CameraX, CameraY, -10f);
        MainCamera.transform.position = newCamPos;
    }
}
