using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStatus
{
    next, play, gameOver, win
}
public class GameManager : Singleton<GameManager> {
    [SerializeField]
    private GameObject spawnPoint;
    [SerializeField]
    private GameObject [] enemies;
   
    [SerializeField]
    private int totalEnemies = 6;
    [SerializeField]
    private int enemiesPerSpawn;
    [SerializeField]
    private int totalWaves = 10;
    [SerializeField]
    private Text totalMoneyLbl;
    [SerializeField]
    private Text currentWaveLbl;
    [SerializeField]
    private Text totalEscapedLbl;
    [SerializeField]
    private Text playButtonLbl;
    [SerializeField]
    private Button playButton;

    private int waveNumber = 0;
    private int totalMoney = 10;
    private int totalEscaped = 0;
    private int roundEscaped = 0;
    private int totalKilled = 0;
    private int witchEnemiesToSpawn = 0;
    private GameStatus currentState = GameStatus.play;
    public bool pause = false;

    private AudioSource audioSource;

    public List<Enemy> enemyList = new List<Enemy>();

    const float spawnDelay = 0.5f;

    public AudioSource AudioSource
    {
        get
        {
            return audioSource;
        }
    }
    public int TotalMoney
    {
        get
        {
            return totalMoney;
        }
        set
        {
            totalMoney = value;
            totalMoneyLbl.text = totalMoney.ToString();
        }
    }

    public int TotalEscaped
    {
        get
        {
            return totalEscaped;
        }
        set
        {
            totalEscaped = value;
        }
    }

    public int RoundEscaped
    {
        get
        {
            return roundEscaped;
        }
        set
        {
            roundEscaped = value;
        }
    }

    public int TotalKilled
    {
        get
        {
            return totalKilled;
        }
        set
        {
            totalKilled = value;
        }
    }
	// Use this for initialization
	void Start () {
        if (pause == false)
        {
            playButton.gameObject.SetActive(false);
            showMenu();
            audioSource = GetComponent<AudioSource>();
        }
        //StartCoroutine(spawn());
	}
	
	// Update is called once per frame
	void Update () {
        if(pause == false)
            handleEscape();
	}

    void Pause() {
        pause = !pause;
    }
    IEnumerator spawn(int enemyId)
    {
        int firstEnemyes = 0;
        int secondEnemyes = 0;
        int thirdEnemyes = 0;

        if (waveNumber <= 2) {
            firstEnemyes = enemiesPerSpawn;
            secondEnemyes = 0;
            thirdEnemyes = 0;
        }

        else if (waveNumber > 2 && waveNumber <= 4)
        {
            firstEnemyes = enemiesPerSpawn - 2;
            secondEnemyes = 2;
            thirdEnemyes = 0;
        }

        else if (waveNumber > 4 && waveNumber <= 5)
        {
            firstEnemyes = enemiesPerSpawn - 6;
            secondEnemyes = 6;
            thirdEnemyes = 0;
        }

        else if (waveNumber > 5 && waveNumber <= 7)
        {
            firstEnemyes = enemiesPerSpawn - 7;
            secondEnemyes = 6;
            thirdEnemyes =  1;
        }

        else if (waveNumber > 7 && waveNumber <= 9)
        {
            firstEnemyes = enemiesPerSpawn - 10;
            secondEnemyes = 7;
            thirdEnemyes =  3;
        }

        else if (waveNumber > 9 && waveNumber <= 11)
        {
            firstEnemyes = 0;
            secondEnemyes = enemiesPerSpawn - 7;
            thirdEnemyes = 7;
        }

        else if (waveNumber > 11 && waveNumber <= 13)
        {
            firstEnemyes = 0;
            secondEnemyes = 0;
            thirdEnemyes = enemiesPerSpawn;
        }
        if (enemiesPerSpawn > 0 && enemyList.Count < totalEnemies)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                if (enemyList.Count < totalEnemies)
                {
                    GameObject newEnemy = Instantiate(enemies[enemyId]) as GameObject;
                    newEnemy.transform.position = spawnPoint.transform.position;
                }
            }
            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(spawn(enemyId));
        }
    }

    public void RegisterEnemy(Enemy newEnemy)
    {
        enemyList.Add(newEnemy);
    }

    public void UnregisterEnemy(Enemy killedEnemy)
    {
        enemyList.Remove(killedEnemy);
        DestroyObject(killedEnemy.gameObject);
    }

    public void DestroyAllEnemies()
    {
        foreach(Enemy e in enemyList)
        {
            DestroyObject(e.gameObject);
        }

        enemyList.Clear();
    }

    public void addMoney(int amount) {
        TotalMoney += amount;
    }
 
    public void subtractMoney(int amount)
    {
        TotalMoney -= amount;
    }

    public void isWaveOver() {
        totalEscapedLbl.text = "Escaped: " + totalEscaped + "/10";
        if (roundEscaped + totalKilled == totalEnemies)
        {
            setCurrentGameState();
            showMenu();
        }
    }

    public void setCurrentGameState() {
        if (totalEscaped >= 10) {
            currentState = GameStatus.gameOver;
        } else if (waveNumber == 0 && (totalKilled + roundEscaped) == 0)
        {
            currentState = GameStatus.play;
        } else if (waveNumber >= totalWaves)
        {
            currentState = GameStatus.win;
        } else {
            currentState = GameStatus.next;
        }
    }
    void showMenu()
    {
        switch(currentState)
        {
            case GameStatus.gameOver:
                playButtonLbl.text = "Play Again";
                GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.GameOver);
                break;
            case GameStatus.next:
                playButtonLbl.text = "Next Wave";
                break;
            case GameStatus.play:
                playButtonLbl.text = "Play Game";
                break;
            case GameStatus.win:
                playButtonLbl.text = "Play Game";
                break;
        }
        playButton.gameObject.SetActive(true);
    }

    public void pauseBtnPressed()
    {
        Pause();
    }
    public void playBtnPressed()
    {
        if (pause == false)
        {
            int enemyId = 0;
            switch (currentState)
            {
                case GameStatus.next:
                    waveNumber++;
                    if (waveNumber >= 0 && waveNumber < 4)
                    {
                        totalEnemies += 2;
                        enemyId = 0;
                    }
                    else if (waveNumber >= 4 && waveNumber < 7)
                    {
                        totalEnemies += 1;
                        enemyId = 1;
                    }
                    else if (waveNumber >= 7 && waveNumber < 9)
                    {
                        totalEnemies += 2;
                        enemyId = 1;
                    }
                    else if (waveNumber >= 9 && waveNumber < 11)
                    {
                        totalEnemies += 1;
                        enemyId = 2;
                    }
                    else if (waveNumber >= 11)
                    {
                        totalEnemies += 2;
                        enemyId = 2;
                    }
                    break;
                default:
                    totalEnemies = 6;
                    totalEscaped = 0;
                    TotalMoney = 10;
                    totalMoneyLbl.text = TotalMoney.ToString();
                    totalEscapedLbl.text = "Escaped: " + totalEscaped.ToString() + "/10";
                    TowerManager.Instance.destroyAllTowers();
                    TowerManager.Instance.renameTagsBuildSite();
                    audioSource.PlayOneShot(SoundManager.Instance.NewGame);
                    break;
            }
            DestroyAllEnemies();
            totalKilled = 0;
            roundEscaped = 0;
            currentWaveLbl.text = "Wave " + (waveNumber + 1);
            StartCoroutine(spawn(enemyId));
            playButton.gameObject.SetActive(false);
            TowerManager.Instance.destroyAllProjectiles();
        }
    }
    private void handleEscape()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TowerManager.Instance.DisableDragSprite();
            TowerManager.Instance.TowerBtnPressed = null;
        }
    }
}
