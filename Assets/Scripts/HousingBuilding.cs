using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBuilding : Building
{
    List<GameObject> workers;

    List<GameObject> possibleWorkers;
    public GameObject worker_female;
    public GameObject worker_male;

    private Dictionary<int, Vector3> workerPositions;

    private int inhabitants;

    public int start_position;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        possibleWorkers.Add(worker_female);
        possibleWorkers.Add(worker_male);
        // spawns 2 grown workers when built
        GameObject initialWorker = createWorker();
        initialWorker.GetComponent<Worker>().SetAge(20);

        GameObject initialWorker2 = createWorker();
        initialWorker2.GetComponent<Worker>().SetAge(20);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject createWorker()
    {
        if (inhabitants < 10)
        {
            GameObject worker = GameObject.Instantiate(possibleWorkers[Random.Range(0, 2)], _tile.transform.position, Quaternion.identity);
            workers.Add(worker);
            rearrangeWorkers();
            return worker;
        }

        return null;
    }

    // arranges the workers so they don't stick in each other in front of the building
    private void rearrangeWorkers()
    {
        for (int i = 0; i < workers.Count; i++)
        {
            double x = workers[i].transform.position.x - 4 + 0.6 * i;
            workers[i].transform.position = new Vector3((float) x, workers[i].transform.position.y, workers[i].transform.position.z);
        }
    }
}
