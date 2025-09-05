using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    #region Public
    public GameObject[] BeeSpawnGround;
    public GameObject Bee;

    public GameObject[] BoarSpawnGround;
    public GameObject Boar;

    public GameObject[] SnailSpawnGround;
    public GameObject Snail;
    public float DelaySpawnMonster;
    #endregion
    #region Private
    private float SpawnMonsterTimer;
    private int MonsterCount;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnMonsterTimer > 0f)
        {
            SpawnMonsterTimer -= Time.deltaTime;
        } else
        {
            if (MonsterCount < 20)
            {
                SpawnMonsterTimer = DelaySpawnMonster;
                SpawnMonster();
            }
        }
    }

    private void SpawnMonster()
    {
        int n = Random.Range(0, 3);
        if (n==0)
        {
            GameObject GroundSpawn = BeeSpawnGround[Random.Range(0, BeeSpawnGround.Length)];
            float xRandom = Random.Range(GroundSpawn.GetComponent<Collider2D>().bounds.min.x, GroundSpawn.GetComponent<Collider2D>().bounds.max.x);
            float yRandom = GroundSpawn.transform.position.y + 0.75f;
            GameObject BeeIns = Instantiate(Bee, new Vector2(xRandom, yRandom), Quaternion.identity);
            BeeIns.GetComponent<Monster>().GroundStanding = GroundSpawn;
            BeeIns.SetActive(true);
            MonsterCount++;
        } else if (n==1)
        {
            GameObject GroundSpawn = BoarSpawnGround[Random.Range(0, BoarSpawnGround.Length)];
            float xRandom = Random.Range(GroundSpawn.GetComponent<Collider2D>().bounds.min.x, GroundSpawn.GetComponent<Collider2D>().bounds.max.x);
            float yRandom = GroundSpawn.transform.position.y + 1f;
            GameObject BoarIns = Instantiate(Boar, new Vector2(xRandom, yRandom), Quaternion.identity);
            BoarIns.GetComponent<Monster>().GroundStanding = GroundSpawn;
            BoarIns.SetActive(true);
            MonsterCount++;
        } else
        {
            GameObject GroundSpawn = SnailSpawnGround[Random.Range(0, SnailSpawnGround.Length)];
            float xRandom = Random.Range(GroundSpawn.GetComponent<Collider2D>().bounds.min.x, GroundSpawn.GetComponent<Collider2D>().bounds.max.x);
            float yRandom = GroundSpawn.transform.position.y + 1f;
            GameObject SnailIns = Instantiate(Snail, new Vector2(xRandom, yRandom), Quaternion.identity);
            SnailIns.GetComponent<Monster>().GroundStanding = GroundSpawn;
            SnailIns.SetActive(true);
            MonsterCount++;
        }
    }

    public void MonsterDead()
    {
        MonsterCount--;
    }
}
