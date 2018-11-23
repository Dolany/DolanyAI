using System;
using System.Collections.Generic;
using System.Text;

namespace PlanDescripProj.CardGame
{
    public class Skill
    {
        public string Name { get; set; }
        public object Action { get; set; }
    }

    public class Carreer
    {
        public string Name { get; set; }
        public Skill Passive { get; set; }
        public Skill Positive { get; set; }
    }
}
