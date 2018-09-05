using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
using System;


public class GeneticExample01 : MonoBehaviour
{
    [SerializeField] GameObject _sphere;

    int _pointCount = 200;
    float size = 100;

    string _text = "";
    GeneticAlgorithm _ga;
    Chromosome _best;
    List<Transform> _spheres = new List<Transform>();


    void Start()
    {
        Application.targetFrameRate = 10;
        AddSpheres();
        SetupGA();
    }

    void Update()
    {
        if (_ga == null) return;

        _text = $"[<b>{_ga.State}</b>] Fitness: <i>{_best?.Fitness:0.00}</i>  Generation: <i>{_ga.GenerationsNumber}</i>";

        var currentBest = _ga.BestChromosome as Chromosome;

        if (currentBest != _best)
        {
            _best = currentBest;
            SetSpherePositions();
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(25, 25, 400, 40), _text);
    }

    void SetSpherePositions()
    {
        var points = _best.GetPoints(-size / 2, size / 2);

        for (int i = 0; i < _pointCount; i++)
        {
            _spheres[i].transform.position = points[i];
        }
    }

    void AddSpheres()
    {
        var parent = new GameObject("Spheres").transform;

        for (int i = 0; i < _pointCount; i++)
        {
            _spheres.Add(Instantiate(_sphere, parent).transform);
        }
    }

    void SetupGA()
    {
        if (_ga != null) _ga.Stop();

        _ga = new GeneticAlgorithm
            (
            new Population(50, 100, AdamChromosome()),
            new FuncFitness(FitnessFunction),
            new EliteSelection(),
            //new BruteForceCrossover(),
            new UniformCrossover(),
            new TworsMutation()
            );

        _ga.Termination = new FitnessStagnationTermination(1000);

        //_ga.TaskExecutor = new LinearTaskExecutor();
        int threads = Environment.ProcessorCount;

        _ga.TaskExecutor = new ParallelTaskExecutor
        {
            MinThreads = threads,
            MaxThreads = threads
        };

        Task.Run(() => _ga.Start());
    }


    Chromosome AdamChromosome()
    {
        return new Chromosome(_pointCount * 2);
    }

    double FitnessFunction(IChromosome chromosome)
    {
        var points = (chromosome as Chromosome).GetPoints(-size / 2, size / 2);
        float fitness = 0;

        Vector3 center = Vector3.zero;

        foreach (var point in points)
            center += point;

        center /= points.Length;

        float radius = 0;

        foreach (var point in points)
        {
            radius += (point - center).magnitude;
        }

        radius /= points.Length;

        foreach (var point in points)
        {
            float error = (point - center).magnitude - radius;
            fitness -= error * error;
        }

        return fitness;
    }


    double FitnessFunction2(IChromosome chromosome)
    {
        var points = (chromosome as Chromosome).GetPoints(-size / 2, size / 2);
        float fitness = 0;

        foreach (var point in points)
        {
            fitness -= Mathf.Abs(point.x);
        }

        return fitness;
    }
}
