using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    public float snakeSpeed = 1;

    public BodyPart bodyPrefab = null;
    public GameObject rockPrefab = null;
    public GameObject eggPrefab = null;
    public GameObject goldEggPrefab = null;
    public GameObject spikePrefab = null;

    public Sprite tailSprite = null;
    public Sprite bodySprite = null;

    public SnakeHead snakeHead = null;

    const float width = 4f;
    const float height = 7f;

    public bool alive = true;

    public bool waitingToPlay = true;

    List<Egg> eggs = new List<Egg>();
    List<Spike> spikes = new List<Spike>();

    int noOfEggsForNextLevel = 0;
    int noOfSpikesForNextLevel = 0;

    public int level = 0;
    public int score = 0;
    public int hiScore = 0;

    public Text scoreText = null;
    public Text hiScoreText = null;
    public Text levelText = null;

    public Text tapToPlayText = null;
    public Text gameOverText = null;

    void Start()
    {
        instance = this;
        CreateWalls();
        alive = false;
    }

    void Update()
    {
        if (waitingToPlay)
        {
            foreach(Touch touch in Input.touches)
            {
                if(touch.phase == TouchPhase.Ended)
                {
                    StartGamePlay();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                StartGamePlay();
            }
        }
    }

    public void GameOver()
    {
        alive = false;
        waitingToPlay = true;

        gameOverText.gameObject.SetActive(true);
        tapToPlayText.gameObject.SetActive(true);
    }

    void StartGamePlay()
    {
        score = 0;
        level = 0;

        scoreText.text = "Score = " + score;
        hiScoreText.text = "HiScore = " + hiScore;
        levelText.text = "Level = " + level;

        gameOverText.gameObject.SetActive(false);
        tapToPlayText.gameObject.SetActive(false);

        waitingToPlay = false;
        alive = true;

        KillOldEggs();
        KillOldSpikes();

        LevelUp();
    }

    void LevelUp()
    {
        levelText.text = "Level = " + level;
        level++;

        noOfEggsForNextLevel = 4 + (level * 2); //6 8 10
        noOfSpikesForNextLevel = 1 + (level * 2);

        snakeSpeed = 1f + (level / 4);
        if(snakeSpeed > 6)
        {
            snakeSpeed = 6;
        }

        snakeHead.ResetSnake();
        CreateEgg();

        KillOldSpikes();
        CreateSpikes();
    }

    public void EggEaten(Egg egg)
    {
        score++;

        noOfEggsForNextLevel--;
        if(noOfEggsForNextLevel == 0)
        {
            score += 10;
            LevelUp();
        }
        else if(noOfEggsForNextLevel == 1)
        {
            CreateEgg(true);
        }
        else
        {
            CreateEgg(false);
        }
        if (score > hiScore)    
        {
            hiScore = score;
            scoreText.text = "Score = " + score;
        }
        hiScoreText.text = "HiScore = " + hiScore;

        eggs.Remove(egg);
        Destroy(egg.gameObject);
    }

    public void SpikeEaten(Spike spike)
    {
        noOfSpikesForNextLevel++;
        if(noOfSpikesForNextLevel == 0)
        {
            CreateSpikes();
        }
        spikes.Remove(spike);
        Destroy(spike.gameObject);

    }

    void CreateWalls()
    {
        float z = 0;

        Vector3 start = new Vector3(-width, -height, z);
        Vector3 finish = new Vector3(-width, +height, z);
        CreateWall(start, finish);

        start = new Vector3(+width, -height, z);
        finish = new Vector3(+width, +height, z);
        CreateWall(start, finish);

        start = new Vector3(-width, -height, z);
        finish = new Vector3(+width, -height, z);
        CreateWall(start, finish);

        start = new Vector3(-width, +height, z);
        finish = new Vector3(+width, +height, z);
        CreateWall(start, finish);

    }

    void CreateWall(Vector3 start, Vector3 finish)
    {
        float distance = Vector3.Distance(start, finish);
        int noOfRocks = (int)(distance * 3f);
        Vector3 delta = (finish - start) / noOfRocks;

        Vector3 position = start;
        for (int rock = 0; rock <= noOfRocks; rock++)
        {
            float rotation = Random.Range(0, 360f);
            float scale = Random.Range(1f, 2f);
            CreateRock(position, scale, rotation);
            position = position + delta;
        }
    }

    void CreateRock(Vector3 position, float scale, float rotation)
    {
        GameObject rock = Instantiate(rockPrefab, position, Quaternion.Euler(0, 0, rotation));
        rock.transform.localScale = new Vector3(scale, scale, 1);
    }

    void CreateEgg(bool golden = false)
    {
        Vector3 position;
        position.x = -width + Random.Range(1f, (width * 2) - 2f);
        position.y = -height + Random.Range(1f, (height * 2) - 2f);
        position.z = 0;
        Egg egg = null;

        if (golden)
        {
           egg = Instantiate(goldEggPrefab, position, Quaternion.identity).GetComponent<Egg>();
        }
        else
        {
           egg = Instantiate(eggPrefab, position, Quaternion.identity).GetComponent<Egg>();
        }
        eggs.Add(egg);
    }

    void KillOldEggs()
    {
        foreach(Egg egg in eggs)
        {
            Destroy(egg.gameObject);
        }
        eggs.Clear();
    }

    void CreateSpikes()
    {
        Vector3 position;
        position.x = -width + Random.Range(1f, (width * 2) - 2f);
        position.y = -height + Random.Range(1f, (height * 2) - 2f);
        position.z = 0;
        Spike spike = null;

        spike = Instantiate(spikePrefab, position, Quaternion.identity).GetComponent<Spike>();
        spikes.Add(spike);
    }

    void KillOldSpikes()
    {
        foreach(Spike spike in spikes)
        {
            if(spike.gameObject != null)
                Destroy(spike.gameObject);
        }
        spikes.Clear();
    }
}
