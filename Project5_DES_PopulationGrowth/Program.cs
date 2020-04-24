using Project5_DES_PopulationGrowth.Objects;
using System;
using System.Collections.Generic;

namespace Project5_DES_PopulationGrowth
{
    class Program
    {
        static void Main(string[] args)
        {
            // Viewing the Population over Time
            var population = new List<Individual>
        {
            new Male(2),
            new Female(2),
            new Male(3),
            new Female(4),
            new Male(5),
            new Male(2),
            new Female(2),
            new Male(3),
            new Female(4),
            new Male(5),
            new Male(2),
            new Female(2),
            new Male(3),
            new Female(4),
            new Male(5),
            new Male(2),
            new Female(2),
            new Male(3),
            new Female(4),
            new Male(5),
            //new Female(3)
        };
            var sim = new Simulation(population, 1000);
            sim.Execute();
            // Print population after simulation
            foreach (var individual in sim.Population)
                Console.WriteLine("Individual {0}", individual);
            //Console.ReadLine();
        }
    }
}
