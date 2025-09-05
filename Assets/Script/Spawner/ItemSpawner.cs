using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    #region Public
    public GameObject[] CoinSpawnGround;
    public GameObject Coin;
    public float DelayCoinSpawn;

    public GameObject[] HealSpawnGround;
    public GameObject Heal;
    public float DelayHealSpawn;

    public GameObject[] UpgradeSpawnGround;
    public GameObject Upgrade;
    public float DelayUpgradeSpawn;

    public GameObject Chest;
    #endregion
    #region Private
    private float CoinSpawnTimer;
    private float HealSpawnTimer;
    private float UpgradeSpawnTimer;

    private int ItemSpawnedCount;
    #endregion
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CoinSpawnTimer -= Time.deltaTime;
        HealSpawnTimer -= Time.deltaTime;
        UpgradeSpawnTimer -= Time.deltaTime;
        if (ItemSpawnedCount <= 20)
        {
            SpawnItemByDelay();
        }
    }

    private void SpawnItemByDelay()
    {
        if (CoinSpawnTimer <= 0f)
        {
            CoinSpawnTimer = DelayCoinSpawn;
            SpawnCoin();
        }
        if (HealSpawnTimer <= 0f)
        {
            HealSpawnTimer = DelayHealSpawn;
            SpawnHeal();
        }
        if (UpgradeSpawnTimer <= 0f)
        {
            UpgradeSpawnTimer = DelayUpgradeSpawn;
            SpawnUpgrade();
        }
    }

    private void SpawnCoin()
    {
        // Spawn coin on random ground by a offset y value
        GameObject GroundSpawn = CoinSpawnGround[Random.Range(0, CoinSpawnGround.Length)];
        float xRandom = Random.Range(GroundSpawn.GetComponent<Collider2D>().bounds.min.x, GroundSpawn.GetComponent<Collider2D>().bounds.max.x);
        float yRandom = GroundSpawn.transform.position.y + 0.75f;
        GameObject CoinIns = Instantiate(Coin, new Vector2(xRandom, yRandom), Quaternion.identity);
        CoinIns.SetActive(true);
        ItemSpawnedCount++;
    }

    public void SpawnCoinByMonster(Vector2 SpawnPosition, Vector2 FinalPosition)
    {
        GameObject CoinIns = Instantiate(Coin, SpawnPosition, Quaternion.identity);
        CoinIns.GetComponent<Collider2D>().enabled = false;
        CoinIns.GetComponent<Item>().CoinSpawnedByMonster = true;
        CoinIns.GetComponent<Item>().FinalPos = FinalPosition;
        CoinIns.SetActive(true);
        ItemSpawnedCount++;
    }


    private void SpawnHeal()
    {
        GameObject GroundSpawn = HealSpawnGround[Random.Range(0, HealSpawnGround.Length)];
        float xRandom = Random.Range(GroundSpawn.GetComponent<Collider2D>().bounds.min.x, GroundSpawn.GetComponent<Collider2D>().bounds.max.x);
        float yRandom = GroundSpawn.transform.position.y + 1f;
        GameObject HealIns = Instantiate(Heal, new Vector2(xRandom, yRandom), Quaternion.identity);
        HealIns.SetActive(true);
        ItemSpawnedCount++;
    }

    private void SpawnUpgrade()
    {
        GameObject GroundSpawn = UpgradeSpawnGround[Random.Range(0, UpgradeSpawnGround.Length)];
        float xRandom = Random.Range(GroundSpawn.GetComponent<Collider2D>().bounds.min.x, GroundSpawn.GetComponent<Collider2D>().bounds.max.x);
        float yRandom = GroundSpawn.transform.position.y + 1.5f;
        GameObject UpgradeIns = Instantiate(Upgrade, new Vector2(xRandom, yRandom), Quaternion.Euler(0,0,90));
        UpgradeIns.SetActive(true);
        ItemSpawnedCount++;
    }

    public void RemoveItem()
    {
        ItemSpawnedCount--;
    }

    public void SpawnChest()
    {
        GameObject GroundSpawn = CoinSpawnGround[Random.Range(0, CoinSpawnGround.Length)];
        float xRandom = Random.Range(GroundSpawn.GetComponent<Collider2D>().bounds.min.x, GroundSpawn.GetComponent<Collider2D>().bounds.max.x);
        float yRandom = GroundSpawn.transform.position.y + 1f;
        GameObject ChestSpawn = Instantiate(Chest, new Vector2(xRandom, yRandom), Quaternion.identity);
        ChestSpawn.SetActive(true);
    }
}
