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

    float LastTakenDamage;

    public GameObject ScorePointDisplayGO;
    TextMeshPro scoreTextMesh;

    public GameObject HealthSliderGO;

    public GameObject Curtain;
    public Animator CurtainAnimator;

    Vector2 PlayerStartPos;
    int currentRoom = 0;

    public List<AudioClip> audioClips;

    AudioSource audioSo;
    [SerializeField]
    Slider healthSlider;
    void Start()
    {
        playerMovement = Player.GetComponent<PlayerMovement>();
        scoreTextMesh = ScorePointDisplayGO.GetComponent<TextMeshPro>();
        healthSlider = HealthSliderGO.GetComponent<Slider>();
        CurtainAnimator = Curtain.GetComponent<Animator>();
        audioSo = this.GetComponent<AudioSource>();
        PlayerStartPos = Player.transform.position;

        SortOutThemesOfBlocks();
        rooms = new List<Room>();

        //rooms.Add(new Room(this.gameObject, ThemedBlockLists[UnityEngine.Random.Range(0, ThemedBlockLists.Count)], 0, 0));
        rooms.Add(new Room(this.gameObject, ThemedBlockLists[0], 0, 0,currentRoom + 1));

        CurtainAnimator.Play("LiftCurtain");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerData.Health<=0)
        {
            SceneManager.LoadScene("MainMenu");
        }
        if (playerData.invinclible && Time.time - LastTakenDamage > playerData.invincibilityTime)
        {
            playerData.invinclible = false;
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
        rooms.Add(new Room(this.gameObject, ThemedBlockLists[0], 0, 0,currentRoom+1));
        Player.transform.position = PlayerStartPos;
        AudioSource.PlayClipAtPoint(audioClips[UnityEngine.Random.Range(0, 2)], Vector3.zero);
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
        if (!playerData.invinclible)
        {
            LastTakenDamage = Time.time;
            playerData.invinclible = true;
            playerData.Health = playerData.Health - damage;
            healthSlider.value = playerData.Health / playerData.MaxHealth;
            playerMovement.TakeDamage(fromLeft);
            AudioSource.PlayClipAtPoint(audioClips[UnityEngine.Random.Range(3, 6)], Vector3.zero);
        }
        
    }
    public void UpdateHpBar()
    {
        healthSlider.value = playerData.Health / playerData.MaxHealth;
    }
    public void PlayerIncreaseScore(float score)
    {
        playerData.score = playerData.score + score;
        scoreTextMesh.text = playerData.score.ToString();
    }
}
