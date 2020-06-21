using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    #region Manager References
    public JobManager _jobManager; //Reference to the JobManager
    public GameManager _gameManager;//Reference to the GameManager
    #endregion

    public int _age = 0; // The age of this worker
    public float _happiness = 1; // happiness of the worker measured as float, between 0 and 1
    public bool _employed = false; // status of employment

    public HousingBuilding _house; // house building
    public Job _job; // references to particular a job
    public List<GameManager.ResourceTypes> _resoucesToConsume 
    {
        get { return new List<GameManager.ResourceTypes> {
            GameManager.ResourceTypes.Fish,
            GameManager.ResourceTypes.Schnapps,
            GameManager.ResourceTypes.Clothes
        }; }

    } // resources which are consumed by every worker

    public Worker(HousingBuilding b)
    {
        this._house = b;
    }
    private void Awake()
    {
        _jobManager = JobManager.Instance;
        _gameManager = GameManager.Instance;
    }
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("IncrementAge", 15f, 15f);
    }

    // Update is called once per frame
    void Update()
    {
        Age();
        Debug.Log("Current Age: " + this._age);
    }
    // increment age by 1
    private void IncrementAge()
    {
        EventualDeath();
        _age++;
        ConsumeResources();
    }
    private void Age()
    {
        //method that defines in which phase of life a human being is

        if (_age > 100)
        {
            Die();
            return;
        }
        if (_age > 64)
        {
            Retire();
            return;
        }
        if (_age > 14)
        {
            BecomeOfAge();
            return;
        }
    }

    private void workerDies()
    {
        //cycle that describes the process of dying
        float prob = Random.Range(0f, 1f);
        if (0.015f * _age - _happiness > prob)
            Die();
    }

    public void BecomeOfAge()
    {
        _jobManager.RegisterWorker(this);
    }

    private void Retire()
    {
        _jobManager.ReleaseJob(this);
    }

    private void Die()
    {
        Retire();
        // reset the worker
        this._age = 0;
        this._happiness = 1f;
        this._employed = false;
        // remove this worker from the house where he or she lives
        this._house.WorkerRemovedFromBuilding(this);
        // remove this worker from the house where he or she works
        if (this._job != null)
            this._job._prodBuilding.WorkerRemovedFromBuilding(this);

        this.gameObject.SetActive(false);
    } 
    float ComputeHappiness()
    {

        float _hasJob = 0.25f;
        float _happinessGrade = 0f;
        // is the worker employed
        if (this._job != null)
            _happinessGrade += _hasJob;
        // is the worker provided with resources
        foreach (var res in _resoucesToConsume)
        {
            if (_gameManager.HasResourceInWarehouse(res))
                _happinessGrade += 0.25f;
        }

        return _happinessGrade;
    }
    private void UpdateHappiness()
    {
        this._happiness = ComputeHappiness();
    }
    private void ConsumeResources()
    {
        foreach(var res in _resoucesToConsume)
        {
            if (_gameManager.HasResourceInWarehouse(res))
                _gameManager._resourcesInWarehouse[res]--;
        }
    }
}
