﻿using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Ai.Game.Pet.PetAgainst;

namespace Dolany.Ai.Core.Ai.Game.Pet
{
    public static class PetExtent
    {
        public static readonly string[] AllAttributes = {"钢铁", "海洋", "深渊", "自然", "神秘"};

        public static string ExtGain(this PetRecord pet, MsgInformationEx MsgDTO, int exp)
        {
            var remainExp = pet.Exp + exp;
            var lvl = pet.Level;
            var levelMode = PetLevelMgr.Instance[lvl];
            while (levelMode.Exp <= remainExp)
            {
                remainExp -= levelMode.Exp;
                lvl++;
                levelMode = PetLevelMgr.Instance[lvl];
            }

            var msg = $"{pet.Name}获得了 {exp} 点经验值！";
            if (lvl > pet.Level)
            {
                var points = (lvl - pet.Level) * 2;
                msg += $"\r恭喜{pet.Name}升到了 {lvl} 级！\r" +
                       $"{pet.Name}获得了 {points} 点技能点！";

                pet.RemainSkillPoints += points;

                var skills = PetSkillMgr.Instance.AllSkills.Where(p => p.LearnLevel > pet.Level && p.LearnLevel <= lvl).ToList();

                if (!skills.IsNullOrEmpty())
                {
                    msg += $"\r恭喜{pet.Name}学会了新技能 {string.Join(",", skills.Select(p => p.Name))}!";
                    if (pet.Skills == null)
                    {
                        pet.Skills = new Dictionary<string, int>();
                    }
                    foreach (var skill in skills)
                    {
                        pet.Skills.Add(skill.Name, 1);
                    }
                }
            }

            pet.Level = lvl;
            pet.Exp = remainExp;
            pet.Update();

            return msg;
        }
    }
}
