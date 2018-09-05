using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

class Chromosome : ChromosomeBase
{
    public Chromosome(int length) : base(length)
    {
        for (int i = 0; i < length; i++)
        {
            ReplaceGene(i, GenerateGene(i));
        }
    }

    public override IChromosome CreateNew()
    {
        return new Chromosome(Length);
    }

    public override Gene GenerateGene(int geneIndex)
    {
        return new Gene(RandomizationProvider.Current.GetFloat());
    }

    public Vector3[] GetPoints(float min, float max)
    {
        var genes = GetGenes();

        var points = new Vector3[genes.Length / 2];

        for (int i = 0; i < genes.Length; i += 2)
        {
            float x = (float)genes[i].Value;
            float y = (float)genes[i + 1].Value;
            // float z = (float)genes[i * 2 + 2].Value;

            x = Mathf.Lerp(min, max, x);
            y = Mathf.Lerp(min, max, y);
            //  z = Mathf.Lerp(min, max, z);

            points[i / 2] = new Vector3(x, y, 0);
        }

        return points;
    }
}