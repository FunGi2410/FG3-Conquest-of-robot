using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public event System.Action OnFireAllEnemy;

    public List<Wave> waves = new List<Wave>();
    public List<Enemy> objPools= new List<Enemy>();

    public Transform enemySpawnPos;

    int currentWaveIndex;

    public float timeBetweenSpawns;
    float nextSpawnTime;

    [SerializeField]
    int enemyAlive;

    [SerializeField]
    bool isAllEnemyDie = false;

    private void Awake()
    {
        foreach(Wave wave in waves)
        {
            for(int i = 0; i < wave.enemyCount; i++)
            {
                Enemy enemyInstance = Instantiate(wave.enemy,
                this.enemySpawnPos.position,
                Quaternion.identity);
                enemyInstance.gameObject.SetActive(false);
                this.objPools.Add(enemyInstance);
            }
        }
    }

    private void Start()
    {
        Enemy enemy = FindObjectOfType<Enemy>();
        if(enemy != null)
        {
            enemy.OnDeath += OneEnemyDeath;
        }
    }

    private void Update()
    {
        if (this.waves.Count <= 0 && this.enemyAlive <= 0)
        {
            if (this.isAllEnemyDie) return;
            this.AllEnemyDie();
            this.isAllEnemyDie = true;
            return;
        }

        if (this.waves.Count <= 0) return;
        if(Time.time > this.nextSpawnTime)
        {
            this.currentWaveIndex = Random.Range(0, this.waves.Count);
            this.nextSpawnTime = Time.time + this.timeBetweenSpawns;

            /*Enemy spawnedEnemy = Instantiate(this.waves[this.currentWaveIndex].enemy, 
                this.enemySpawnPos.position, 
                Quaternion.identity) as Enemy;*/
            for(int i = 0; i < objPools.Count; i++)
            {
                if(!objPools[i].gameObject.activeSelf && objPools[i].Id == waves[currentWaveIndex].enemy.Id)
                {
                    objPools[i].gameObject.SetActive(true);
                    break;
                }
            }
            this.enemyAlive++;
            this.waves[this.currentWaveIndex].enemyCount--;
            if (this.waves[this.currentWaveIndex].enemyCount <= 0)
            {
                this.waves.RemoveAt(this.currentWaveIndex);
            }
        }
    }

    public void OneEnemyDeath()
    {
        this.enemyAlive--;
    }

    protected void AllEnemyDie()
    {
        if (this.OnFireAllEnemy != null)
        {
            this.OnFireAllEnemy();
        }
    }

    /*void OnEnemyDeath()
    {
        this.enemiesRemainingAlive--;
        if(this.enemiesRemainingAlive <= 0)
        {
            this.NextWave();
        }
    }*/

    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public Enemy enemy;
    }
}
