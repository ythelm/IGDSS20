using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    GameManager _gameManager;//Reference to the GameManager
    #endregion

    public float _age; // The age of this worker
    public float _happiness; // The happiness of this worker

    public Job _job; //The job this worker is assigned to
    public HousingBuilding _home; //The house this worker is assigned to

    public float _agingInterval = 15; //The time in seconds it takes for a worker to age by one year
    private float _agingProgress; //The counter that needs to be incrementally increased during a production cycle
    private bool _becameOfAge = false;
    private bool _retired = false;

    public Building _Workplace; // js
    // Adjust the speed for the movement.
    public float speed = 1.0f;
    public GameObject _worker;
    public Tile _nowOnTile;


    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.Instance;
        _worker = GameObject.Find("Worker");
        _nowOnTile = _home._tile;
    }

    // Update is called once per frame
    void Update()
    {
       Age();
//       if (_job != null) {MoveToJob();}
    }

    private void Age()
    {
        _agingProgress += Time.deltaTime;
        if (_agingProgress > _agingInterval)
        {
            _agingProgress = 0;
            _age++;
            ConsumeResourcesAndCalculateHappiness();
            ChanceOfDeath();
        }

        if (_age > 14 && !_becameOfAge)
        {
            BecomeOfAge();
        }

        if (_age > 64 && !_retired)
        {
            Retire();
        }

        if (_age > 100)
        {
            Die();
        }
    }

    private void ConsumeResourcesAndCalculateHappiness()
    {
        bool fish = _gameManager.RemoveResourceFromWarehouse(GameManager.ResourceTypes.Fish, 2);
        bool clothes = _gameManager.RemoveResourceFromWarehouse(GameManager.ResourceTypes.Clothes, 2);
        bool schnapps = _gameManager.RemoveResourceFromWarehouse(GameManager.ResourceTypes.Schnapps, 2);
        bool job = _job != null;

        float happinessTarget = (fish ? 25 : 0) + (clothes ? 25 : 0) + (schnapps ? 25 : 0) + (job ? 25 : 10);
        _happiness = (happinessTarget + _happiness) / 2;
    }

    private void ChanceOfDeath()
    {
        float chanceOfDeath = _age * 0.1f * (100f / _happiness);

        float rng = Random.Range(0f, 100f);

        if (rng < chanceOfDeath)
        {
            Die();
        }
    }

    public void AssignToJob(Job job)
    {
        _job = job;
        _Workplace=job._building;
        print("I assigned to job" + job._building._type);
        StartCoroutine("MoveToJob");
    }

    public void AssignToHome(HousingBuilding home)
    {
        _home = home;
    }

    public void BeBorn()
    {
        _age = 0;
        gameObject.name = "Child";
    }

    public void BecomeOfAge()
    {
        _becameOfAge = true;

        _jobManager = JobManager.Instance;
        _jobManager.RegisterWorker(this);
        gameObject.name = "Worker";
    }

    private void Retire()
    {
        _retired = true;
        _jobManager.RemoveWorker(this);
        gameObject.name = "Retiree";
    }

    private void Die()
    {
        if (_job != null ) {_jobManager.RemoveWorker(this);}
        _home.RemoveWorker(this);
        GameManager.Instance.RemoveWorker(this);
        print("A " + gameObject.name + " has died");

        Destroy(this.gameObject, 1f);
    }

    public IEnumerator MoveToJob(){
         int[,] pfm =_Workplace._pathFindingMap;
        while (_Workplace._tile != _nowOnTile){
        MoveTo(getNextTile(pfm));
        yield return null;
        }
    }

    public Tile getNextTile (int[,] pfm)
    {
        List<Tile> neighbors = _nowOnTile._neighborTiles;
        
        int nextTileWeight = 2147483647;
        Tile nextTile = _home._tile;
        
        foreach (Tile tl in neighbors)
        {
           int h = tl._coordinateHeight;
           int w = tl._coordinateWidth;
            if (pfm[h,w] < nextTileWeight){
                nextTileWeight = pfm[h,w];
                nextTile = _gameManager._tileMap[h,w];
            }
        }
        print("NowOnTile " + _nowOnTile + " nextTile" + nextTile);
        return nextTile;
    }

    public void MoveTo(Tile t)
    {
        Transform tf = t.transform;
        if (_worker != null) 
        {
        float step =  speed * Time.deltaTime; // calculate distance to move
        // Move our position a step closer to the target:
        transform.position = Vector3.MoveTowards(_worker.transform.position, tf.position, step);
        _nowOnTile = t;
        }
    
    }
}
