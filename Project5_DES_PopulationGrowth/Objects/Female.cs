using MathNet.Numerics.Distributions;
using Project5_DES_PopulationGrowth.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project5_DES_PopulationGrowth.Objects
{
    public class Female : Individual
    {
        public bool IsPregnant { get; set; }
        public double PregnancyAge { get; set; }
        public double ChildrenCount { get; set; }
        public Female(int age) : base(age)
        {
        }

        public bool SuitablePregnancy(int currentTime)
        {
            return Age >= PregnancyAge && currentTime <= TimeChildren && ChildrenCount > 0;
        }

        public Individual GiveBirth(Dictionary<Event, IDistribution> distributions, int currentTime)
        {
            var sample =
            ((ContinuousUniform)distributions[Event.BirthEngageDisengage]).Sample();
            var child = sample > 0.5 ? (Individual)new Male(0) : new Female(0);
            // One less child to give birth to
            ChildrenCount--;
            child.LifeTime = ((Poisson)distributions[Event.Die]).Sample();
            child.RelationAge = ((Poisson)distributions[Event.CapableEngaging]).Sample();
            if (child is Female)
            {
                (child as Female).PregnancyAge =
                    ((Normal)distributions[Event.GetPregnant]).Sample();
                (child as Female).ChildrenCount =
                    ((Normal)distributions[Event.ChildrenCount]).Sample();
            }
            if (Engaged && ChildrenCount > 0)
            {
                TimeChildren = currentTime + ((Exponential)distributions[Event.TimeChildren]).Sample();
                Couple.TimeChildren = TimeChildren;
            }
            else
                TimeChildren = 0;
                IsPregnant = false;
            return child;
        }

        public override string ToString()
        {
            return base.ToString() + " Female";
        }
    }
}
