using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;


public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public Brick BrickPrefab;
    public int LineCount = 6;
    [SerializeField]
    private Rigidbody Ball;
    [SerializeField]
    private Text ScoreText;    
    [SerializeField]
    private Text BestScoreText;
    [SerializeField]
    private GameObject GameOverText;
    [SerializeField]
    private bool m_Started = false;
    private int m_Points;
    private int bestScore;
    [SerializeField]
    private bool m_GameOver = false;

    public string playerName;
    private bool loaded = false;

    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
           
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1 && !loaded)
        {
            Ball = GameObject.Find("Ball").GetComponent<Rigidbody>();
            ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
            BestScoreText = GameObject.Find("BestScoreText").GetComponent<Text>();
            GameOverText = GameObject.Find("GameoverText");
            GameOverText.SetActive(false);
            LoadBestScore();
            BestScoreText.text = "Best Score: " + playerName + $" :{bestScore}";


            const float step = 0.6f;
            int perLine = Mathf.FloorToInt(4.0f / step);

            int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
            for (int i = 0; i < LineCount; ++i)
            {
                for (int x = 0; x < perLine; ++x)
                {
                    Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                    var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                    brick.PointValue = pointCountArray[i];
                    brick.onDestroyed.AddListener(AddPoint);
                }
            }
            loaded = true;
        }

        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                loaded = false;

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                m_GameOver = false;
                m_Started = false;
                m_Points = 0;

            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        if (m_Points>bestScore)
        {
            bestScore = m_Points;
            BestScoreText.text = "Best Score: " + playerName + $" :{bestScore}";
            SaveBestScore();
        }
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    [System.Serializable]
    class SaveData
    {
        public string playerName;
        public int bestScore;
    }

    public void SaveBestScore()
    {
        SaveData data = new SaveData();
        data.playerName = playerName;
        data.bestScore = bestScore;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadBestScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            playerName = data.playerName;
            bestScore = data.bestScore;
        }
    }
}
