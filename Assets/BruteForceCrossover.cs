using GeneticSharp.Domain.Chromosomes;
using System;
using System.Linq;
using GeneticSharp.Domain.Randomizations;
using System.Collections.Generic;
using GeneticSharp.Domain.Crossovers;

public class BruteForceCrossover : CrossoverBase
{

    public BruteForceCrossover() : base(1,1)
    {
    }

    protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
    {
        return new List<IChromosome> { parents[0].CreateNew() };
    }
}