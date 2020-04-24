using MathNet.Numerics.Distributions;
using Project5_DES_PopulationGrowth.Events;
using Project5_DES_PopulationGrowth.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project5_DES_PopulationGrowth
{
    class Simulation
    {
        public List<Individual> Population { get; set; }
        public int Time { get; set; }
        public int _currentTime;
        public readonly Dictionary<Event, IDistribution> _distributions;

        public Simulation(IEnumerable<Individual> population, int time)
        {
            Population = new List<Individual>(population);
            Time = time;
            _distributions = new Dictionary<Event, IDistribution>
            {
                {Event.CapableEngaging, new Poisson(18) },
                {Event.BirthEngageDisengage, new ContinuousUniform() },
                {Event.GetPregnant, new Normal(28, 8) },
                { Event.ChildrenCount, new Normal(2, 6) },
                { Event.TimeChildren, new Exponential(8) },
                { Event.Die, new Poisson(70) },
            };
            foreach (var individual in Population)
            {
                // LifeTime
                individual.LifeTime = ((Poisson)_distributions[Event.Die]).Sample();
                //Ready to start having relations
                individual.RelationAge = ((Poisson)_distributions[Event.CapableEngaging]).Sample();
                //Pregnancy (only women)
                if (individual is Female)
                {
                    (individual as Female).PregnancyAge =
                        ((Normal)_distributions[Event.GetPregnant]).Sample();
                    (individual as Female).ChildrenCount =
                        ((Normal)_distributions[Event.ChildrenCount]).Sample();
                }
            }
        }

        // Execute() -> where all simulation logi occurs 
        public void Execute()
        {
            // Iterations in the while repr years that go by in the simulation
            while (_currentTime < Time)
            {
                // Iterations here goes through events that might occur those
                // individuals in that particular year
                // Check what happens to every individual this year
                for (var i = 0; i < Population.Count; i++)
                {
                    var individual = Population[i];
                    // Event -> Birth
                    if (individual is Female && (individual as Female).IsPregnant)
                        Population.Add((individual as Female).GiveBirth(_distributions, _currentTime));
                    // Event -> check whether someone starts a relationship this year
                    if (individual.SuitableRelation())
                        individual.FindPartner(Population, _currentTime, _distributions);
                    // Events where having an engaged individual represents a prerequisite
                    if (individual.Engaged)
                    {
                        // Event -> check if a rel ends this year
                        if (individual.EndRelation(_distributions))
                            individual.Disengage();
                        // Event -> check if a couple can have child now
                        if (individual is Female && (individual as Female).SuitablePregnancy(_currentTime))
                            (individual as Female).IsPregnant = true;
                    }
                    //Event -> check if someone dies this year
                    if (individual.Age.Equals(individual.LifeTime))
                    {
                        // Case: Individual in relationship (break relation)
                        if (individual.Engaged)
                            individual.Disengage();
                        Population.RemoveAt(i);
                    }
                    individual.Age++;
                    _currentTime++;
                }
            }
        }
    }
}
