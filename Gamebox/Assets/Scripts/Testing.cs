using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] GameObject whiteSquare;
    [SerializeField] GameObject boostTilePrefab;
    [SerializeField] GameObject doubleCashTilePrefab;
    [SerializeField] GameObject autoHarvestTilePrefab;

    public static Testing singleton;

    private void Awake()
    {
        if (singleton != null)
        {
            Destroy(this);
        }
        singleton = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        GridClass grid = new GridClass(12, 12, 1, new Vector3(-6, -6, 0));

        //GridClass newGrid = new GridClass(12, 5, 1, new Vector3(-6, 1, 0));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnTile(Vector3 spawnPosition)
    {
        GameObject newTile = Instantiate(whiteSquare, spawnPosition, Quaternion.identity, this.transform);
    }
    public GameObject SpawnBoostTile(Vector3 spawnPosition)
    {
        GameObject boostTile = Instantiate(boostTilePrefab, spawnPosition, Quaternion.identity, this.transform);
        return boostTile;
    }
    public GameObject SpawnDoubleCashTile(Vector3 spawnPosition)
    {
        GameObject doubleCashTile = Instantiate(doubleCashTilePrefab, spawnPosition, Quaternion.identity, this.transform);
        return doubleCashTile;
    }
    public GameObject SpawnAutoHarvestTile(Vector3 spawnPosition)
    {
        GameObject doubleCashTile = Instantiate(autoHarvestTilePrefab, spawnPosition, Quaternion.identity, this.transform);
        return doubleCashTile;
    }
}
