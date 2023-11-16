using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private bool canSpawnEnemy1 = true;
    private bool canSpawnEnemy2 = true;
    private bool canSpawnEnemy3 = true;
    [SerializeField] int spawnRate;
  //  [SerializeField] GameObject enemyPrefab;
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
    private bool isBossSpawned = false;
    public float timePassed = 0;
    private float timeAtStart;

    private float spawnDelaySeconds1 = 3; // delay between each grouping for Enemy_1
    private float spawnDelaySeconds2 = 5; // delay between each grouping for Enemy_2
    private float spawnDelaySeconds3 = 10; // delay between each grouping for Enemy_3

    [SerializeField] int enemiesPerGroup1; // amount of enemies per spawn for Enemy_1
    [SerializeField] private int enemiesPerGroup2; // amount of enemies per spawn for Enemy_2
    [SerializeField] private int enemiesPerGroup3; // amount of enemies per spawn for Enemy_3


    private float counter1 = 0; // how many times coroutine has gone for Enemy_1
    private float counter2 = 0; // how many times coroutine has gone for Enemy_2
    private float counter3 = 0; // how many times coroutine has gone for Enemy_3
    

    // Start is called before the first frame update
    void Start()
    {
        timeAtStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawnEnemy1 && !isBossSpawned)
        {
            StartCoroutine(SpawnEnemy1());
        }

        if (canSpawnEnemy2 && !isBossSpawned && Time.time - timeAtStart > 60)
        {
            StartCoroutine (SpawnEnemy2());
        }

        if (canSpawnEnemy3 && !isBossSpawned && Time.time - timeAtStart > 120)
        {
            StartCoroutine(SpawnEnemy3());
        }
    }

    IEnumerator SpawnEnemy1()
    {
        canSpawnEnemy1 = false;
        for (int i = 0; i < enemiesPerGroup1; i++)
        {
           // int index = 0;
          //  float randomEnemyType = Random.Range(0, 99f); // For determining what type of enemy
            float randomOffset = Random.Range(-0.2f, 4f); // For determining how to adjust enemy position
            float randomSecondaryOffset = Random.Range(0.5f, 0.8f); // For slight adjustment on x for up/down spawning and on y for left/right
                                                                    // random val for sides 0-180  <30 on top 30-90 left 90-150 right 150-180 bottom 
            int randomNumber = Random.Range(0, 180); // used for determining whether left/right up/down spawning


            /* if (Time.time - GameManager.elapsedTime < 60) // 60
             {
                 randomEnemyType = 55f; 
             }*/
            // never used enemyTypes
            // 4 enemy_1 spawn at once

           
           
            if (Time.time - timeAtStart >= 300 && isBossSpawned == false) // 300
            {
                isBossSpawned = true;
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Kill all enemies and their effects  (add dying animation [in enemy die()] if possible)
                foreach (GameObject enemyz in enemies)
                {
                    Destroy(enemyz);
                }
                GameManager.instance.InitiateBossFight();
                GameObject enemy = Instantiate(enemyPrefabs[3], transform.position + new Vector3(5,0, 0), Quaternion.identity);
                StopAllCoroutines();
            }

            // Positioning logic -------------------------------------------------------------------

            if (0 < randomNumber && randomNumber <= 30) // on top
            {
                GameObject enemy = Instantiate(enemyPrefabs[0], transform.position, Quaternion.identity);
                enemy.transform.position = new Vector3(enemy.transform.position.x + randomOffset, enemy.transform.position.y + randomSecondaryOffset, 0);
            }

            else if (30 < randomNumber && randomNumber <= 90) // on left
            {
                GameObject enemy = Instantiate(enemyPrefabs[0], transform.position, Quaternion.identity);
                enemy.transform.position = new Vector3(enemy.transform.position.x + -randomSecondaryOffset, enemy.transform.position.y + randomOffset, 0);
            }

            else if (90 < randomNumber && randomNumber <= 150) // on right
            {
                GameObject enemy = Instantiate(enemyPrefabs[0], transform.position, Quaternion.identity);
                enemy.transform.position = new Vector3(enemy.transform.position.x + randomSecondaryOffset, enemy.transform.position.y + randomOffset, 0);
            }

            else if (150 < randomNumber && randomNumber <= 180) // on bottom 
            {
                GameObject enemy = Instantiate(enemyPrefabs[0], transform.position, Quaternion.identity);
                enemy.transform.position = new Vector3(enemy.transform.position.x + randomOffset, enemy.transform.position.y + -randomSecondaryOffset, 0);
            }
            yield return new WaitForSeconds(spawnDelaySeconds1);
        }
        // END Positioning logic --------------------------------------------------------------

        counter1++;
        yield return new WaitForSeconds(2);
        canSpawnEnemy1 = true;

        if (counter1 % 1 == 0) // if Coroutine has been iterated over 1 times... enemies spawn more frequently, enemiesPerGroup decrease
        {
            float enemiesPerGroupNew = enemiesPerGroup1 * 1.5f;
            spawnDelaySeconds1 /= (enemiesPerGroupNew / enemiesPerGroup1);

            enemiesPerGroup1 = Mathf.RoundToInt(enemiesPerGroupNew);
        }

        
    }

    IEnumerator SpawnEnemy2() // Uncommon enemy spawner logic
    {
        
        canSpawnEnemy2 = false;
        for (int i = 0; i < enemiesPerGroup2; i++)
        {
         //   int index = 0;
        //   float randomEnemyType = Random.Range(0, 99f); // For determining what type of enemy
            float randomOffset = Random.Range(-0.2f, 4f); // For determining how to adjust enemy position
            float randomSecondaryOffset = Random.Range(0.5f, 0.8f); // For slight adjustment on x for up/down spawning and on y for left/right
                                                                    // random val for sides 0-180  <30 on top 30-90 left 90-150 right 150-180 bottom 
            int randomNumber = Random.Range(0, 180); // used for determining whether left/right up/down spawning

            // Positioning logic -------------------------------------------------------------------

            if (0 < randomNumber && randomNumber <= 30) // on top
            {
                GameObject enemy = Instantiate(enemyPrefabs[1], transform.position, Quaternion.identity);
                enemy.transform.position = new Vector3(enemy.transform.position.x + randomOffset, enemy.transform.position.y + randomSecondaryOffset, 0);
            }

            else if (30 < randomNumber && randomNumber <= 90) // on left
            {
                GameObject enemy = Instantiate(enemyPrefabs[1], transform.position, Quaternion.identity);
                enemy.transform.position = new Vector3(enemy.transform.position.x + -randomSecondaryOffset, enemy.transform.position.y + randomOffset, 0);
            }

            else if (90 < randomNumber && randomNumber <= 150) // on right
            {
                GameObject enemy = Instantiate(enemyPrefabs[1], transform.position, Quaternion.identity);
                enemy.transform.position = new Vector3(enemy.transform.position.x + randomSecondaryOffset, enemy.transform.position.y + randomOffset, 0);
            }

            else if (150 < randomNumber && randomNumber <= 180) // on bottom 
            {
                GameObject enemy = Instantiate(enemyPrefabs[1], transform.position, Quaternion.identity);
                enemy.transform.position = new Vector3(enemy.transform.position.x + randomOffset, enemy.transform.position.y + -randomSecondaryOffset, 0);
            }
            yield return new WaitForSeconds(spawnDelaySeconds2);
        }
        // END Positioning logic --------------------------------------------------------------

        counter2++;
        yield return new WaitForSeconds(2);
        canSpawnEnemy2 = true;

        if (counter2 % 3 == 0) // if Coroutine has been iterated over 4 times... enemies spawn more frequently, enemiesPerGroup decrease
        {
            float enemiesPerGroupNew = enemiesPerGroup2 * 1.25f;
            spawnDelaySeconds2 /= (enemiesPerGroupNew / enemiesPerGroup2);

            enemiesPerGroup2 = Mathf.RoundToInt(enemiesPerGroupNew);

        }

        

    }



    IEnumerator SpawnEnemy3() // Uncommon enemy spawner logic
    {

        canSpawnEnemy3 = false;
        for (int i = 0; i < enemiesPerGroup3; i++)
        {
            //   int index = 0;
            //   float randomEnemyType = Random.Range(0, 99f); // For determining what type of enemy
            float randomOffset = Random.Range(-0.2f, 4f); // For determining how to adjust enemy position
            float randomSecondaryOffset = Random.Range(0.5f, 0.8f); // For slight adjustment on x for up/down spawning and on y for left/right
                                                                    // random val for sides 0-180  <30 on top 30-90 left 90-150 right 150-180 bottom 
            int randomNumber = Random.Range(0, 180); // used for determining whether left/right up/down spawning

            // Positioning logic -------------------------------------------------------------------

            if (0 < randomNumber && randomNumber <= 30) // on top
            {
                GameObject enemy = Instantiate(enemyPrefabs[2], transform.position, Quaternion.identity);
                enemy.transform.position = new Vector3(enemy.transform.position.x + randomOffset, enemy.transform.position.y + randomSecondaryOffset, 0);
            }

            else if (30 < randomNumber && randomNumber <= 90) // on left
            {
                GameObject enemy = Instantiate(enemyPrefabs[2], transform.position, Quaternion.identity);
                enemy.transform.position = new Vector3(enemy.transform.position.x + -randomSecondaryOffset, enemy.transform.position.y + randomOffset, 0);
            }

            else if (90 < randomNumber && randomNumber <= 150) // on right
            {
                GameObject enemy = Instantiate(enemyPrefabs[2], transform.position, Quaternion.identity);
                enemy.transform.position = new Vector3(enemy.transform.position.x + randomSecondaryOffset, enemy.transform.position.y + randomOffset, 0);
            }

            else if (150 < randomNumber && randomNumber <= 180) // on bottom 
            {
                GameObject enemy = Instantiate(enemyPrefabs[2], transform.position, Quaternion.identity);
                enemy.transform.position = new Vector3(enemy.transform.position.x + randomOffset, enemy.transform.position.y + -randomSecondaryOffset, 0);
            }
            yield return new WaitForSeconds(spawnDelaySeconds3);
        }
        // END Positioning logic --------------------------------------------------------------

        counter3++;
        yield return new WaitForSeconds(2);
        canSpawnEnemy3 = true;

        if (counter3 % 3 == 0) // if Coroutine has been iterated over 4 times... enemies spawn more frequently, enemiesPerGroup decrease
        {
            float enemiesPerGroupNew = enemiesPerGroup3 * 1.25f;
            spawnDelaySeconds3 /= (enemiesPerGroupNew / enemiesPerGroup3);

            enemiesPerGroup3 = Mathf.RoundToInt(enemiesPerGroupNew);

        }



    }
}
