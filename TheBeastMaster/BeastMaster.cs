﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.POI;
using Styx.CommonBot.Routines;
using Styx.Helpers;
using Styx.Pathing;
using Styx.Plugins;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace TheBeastMaster
{
    internal class BeastMaster : CombatRoutine
    {
        public override sealed string Name { get { return "The Beast Master PvE CC 1.1"; } }

        public override WoWClass Class { get { return WoWClass.Hunter; } }

        private static LocalPlayer Me { get { return ObjectManager.Me; } }


        #region Log
        private static void slog(string format, params object[] args) //use for slogging
        {
            Logging.Write(format, args);
        }

        #endregion


        #region Initialize
        public override void Initialize()
        {
            Logging.Write(Colors.Crimson, "The Beast Master 1.1");
            Logging.Write(Colors.Crimson, "A Beast Mastery Hunter Routine");
            Logging.Write(Colors.Crimson, "Made By FallDown");
            Logging.Write(Colors.Crimson, "For LazyRaider Only!");
        }
        #endregion



        #region Settings

        public override bool WantButton { get { return true; } }

        #endregion

        public override void OnButtonPress()
        {
            slog("Config opened!");
            new BeastForm1().ShowDialog();
        }

        #region Halt on Trap Launcher
        public bool HaltTrap()
        {
            if (!Me.HasAura("Trap Launcher"))
                return true;
            else return false;
        }
        #endregion

        #region Halt on Feign Death
        public bool HaltFeign()
        {
            {
                if (!Me.ActiveAuras.ContainsKey("Feign Death"))
                    return true;
            }
            return false;
        }
        #endregion

        #region SelfControl
        public bool SelfControl(WoWUnit unit)
        {
            if (Me.GotTarget && (unit.HasAura("Freezing Trap") || unit.HasAura("Wyvern Sting") || unit.HasAura("Scatter Shot") || unit.HasAura("Bad Manner")))
                return true;

            else return false;
        }
        #endregion

        #region Dragon Soul

        public bool DebuffByID(int spellId)
        {
            if (Me.HasAura(spellId) && StyxWoW.Me.GetAuraById(spellId).TimeLeft.TotalMilliseconds <= 2000)
                return true;
            else return false;
        }

        public bool Ultra()
        {
            if (BeastMasterSettings.Instance.DSNOR || BeastMasterSettings.Instance.DSLFR)
            {
                if (!Me.ActiveAuras.ContainsKey("Deterrence"))
                {
                    foreach (WoWUnit u in ObjectManager.GetObjectsOfType<WoWUnit>(true, true))
                    {
                        if (u.IsAlive
                            && u.Guid != Me.Guid
                            && u.IsHostile
                            && u.IsCasting
                            && (u.CastingSpell.Id == 109417
                                || u.CastingSpell.Id == 109416
                                || u.CastingSpell.Id == 109415
                                || u.CastingSpell.Id == 106371)
                            && u.CurrentCastTimeLeft.TotalMilliseconds <= 800)
                            return true;
                    }
                }
            }
            return false;
        }

        public bool UltraFL()
        {
            if (DebuffByID(110079)
                || DebuffByID(110080)
                || DebuffByID(110070)
                || DebuffByID(110069)
                || DebuffByID(109075)
                || DebuffByID(110068)
                || DebuffByID(105925)
                || DebuffByID(110078))
                return true;

            else return false;
        }

        public bool DW()
        {
            if (DebuffByID(110139)
                || DebuffByID(110140)
                || DebuffByID(110141)
                || DebuffByID(106791)
                || DebuffByID(109599)
                || DebuffByID(106794)
                || DebuffByID(109597)
                || DebuffByID(109598))
                return true;

            else return false;
        }
        #endregion

        #region Boss Check

        private bool IsTargetBoss()
        {
            if (Me.CurrentTarget.CreatureRank == WoWUnitClassificationType.WorldBoss ||
               (Me.CurrentTarget.Level >= 85 && Me.CurrentTarget.Elite && Me.CurrentTarget.MaxHealth > 3500000))
                return true;

            else return false;
        }
        private bool IsTargetEasyBoss()
        {
            if (Me.CurrentTarget.CreatureRank == WoWUnitClassificationType.WorldBoss ||
               (Me.CurrentTarget.Level >= 85 && Me.CurrentTarget.Elite && Me.CurrentTarget.MaxHealth > 300000))
                return true;

            else return false;
        }
        #endregion

        #region CastSpell Method

        public static bool CastSpell(int spellID, string spellName)
        {
            if (SpellManager.CanCast(spellID))
            {
                if (!SpellManager.HasSpell(spellID))
                    return false;

                if (SpellManager.Spells[spellName].CooldownTimeLeft.TotalMilliseconds > 500)
                    return false;


                bool inRange = false;
                WoWUnit target = StyxWoW.Me;
                if (StyxWoW.Me.CurrentTarget != null)
                    target = StyxWoW.Me.CurrentTarget;


                if (target == StyxWoW.Me)
                    inRange = true;
                else
                {
                    WoWSpell spell;
                    if (SpellManager.Spells.TryGetValue(spellName, out spell))
                    {
                        float minRange = spell.MinRange;
                        float maxRange = spell.MaxRange;
                        double targetDistance = target.Distance;
                        // RangeId 1 is "Self Only". This should make life easier for people to use self-buffs, or stuff like Starfall where you cast it as a pseudo-buff.
                        if (spell.IsSelfOnlySpell)
                            inRange = true;
                        // RangeId 2 is melee range. Huzzah :)
                        else if (spell.IsMeleeSpell)
                            inRange = targetDistance < 5;
                        else
                            inRange = targetDistance < maxRange &&
                                      targetDistance > minRange;
                    }
                }
                if (inRange)
                {
                    SpellManager.Cast(spellID);
                    return true;
                    // We managed to cast the spell, so return true, saying we were able to cast it.
                }
            }
            // Can't cast the spell right now, so return false.
            return false;
        }

        public static bool CastSpell(string spellName, WoWUnit target)
        {
            if (SpellManager.CanCast(spellName, target))
            {
                if (!SpellManager.HasSpell(spellName))
                    return false;

                if (SpellManager.Spells[spellName].Cooldown)
                    return false;

                bool inRange = false;
                if (target == StyxWoW.Me)
                    inRange = true;

                else
                {
                    WoWSpell spell;
                    if (SpellManager.Spells.TryGetValue(spellName, out spell))
                    {
                        float minRange = spell.MinRange;
                        float maxRange = spell.MaxRange;
                        double targetDistance = target.Distance;
                        // RangeId 1 is "Self Only". This should make life easier for people to use self-buffs, or stuff like Starfall where you cast it as a pseudo-buff.
                        if (spell.IsSelfOnlySpell)
                            inRange = true;
                        // RangeId 2 is melee range. Huzzah :)
                        else if (spell.IsMeleeSpell)
                            inRange = targetDistance < 5;
                        else
                            inRange = targetDistance < maxRange &&
                                      targetDistance > minRange;
                    }
                }

                if (inRange)
                {
                    SpellManager.Cast(spellName, target);
                    return true;
                }
            }
            return false;
        }

        public static bool CastSpell(string spellName)
        {
            if (SpellManager.CanCast(spellName))
            {
                if (!SpellManager.HasSpell(spellName))
                    return false;

                if (SpellManager.Spells[spellName].CooldownTimeLeft.TotalMilliseconds > 500)
                    return false;


                bool inRange = false;
                WoWUnit target = StyxWoW.Me;
                if (StyxWoW.Me.CurrentTarget != null)
                    target = StyxWoW.Me.CurrentTarget;


                if (target == StyxWoW.Me)
                    inRange = true;
                else
                {
                    WoWSpell spell;
                    if (SpellManager.Spells.TryGetValue(spellName, out spell))
                    {
                        float minRange = spell.MinRange;
                        float maxRange = spell.MaxRange;
                        double targetDistance = target.Distance;
                        // RangeId 1 is "Self Only". This should make life easier for people to use self-buffs, or stuff like Starfall where you cast it as a pseudo-buff.
                        if (spell.IsSelfOnlySpell)
                            inRange = true;
                        // RangeId 2 is melee range. Huzzah :)
                        else if (spell.IsMeleeSpell)
                            inRange = targetDistance < 5;
                        else
                            inRange = targetDistance < maxRange &&
                                      targetDistance > minRange;
                    }
                }
                if (inRange)
                {
                    SpellManager.Cast(spellName);
                    return true;
                    // We managed to cast the spell, so return true, saying we were able to cast it.
                }
            }
            // Can't cast the spell right now, so return false.
            return false;
        }

        #endregion


        #region Controlled

         private static List<WoWSpellMechanic> controlMechanic = new List<WoWSpellMechanic>()
        {
            WoWSpellMechanic.Charmed,
            WoWSpellMechanic.Disoriented,
            WoWSpellMechanic.Fleeing,
            WoWSpellMechanic.Frozen,
            WoWSpellMechanic.Incapacitated,
            WoWSpellMechanic.Polymorphed,
            WoWSpellMechanic.Sapped
        };

        public bool isControlled(WoWUnit u)
        {
            foreach (WoWAura aura in u.Auras.Values)
            {
                if (controlMechanic.Contains(WoWSpell.FromId(aura.SpellId).Mechanic))
                    return true;
                else
                    return false;
            }
            return true;
        }
        #endregion

        #region Add Detection

        private int addCount()
        {
            int count = 0;
            foreach (WoWUnit u in ObjectManager.GetObjectsOfType<WoWUnit>(true, true))
            {
                if (Me.GotTarget
                    && u.IsAlive
                    && u.Guid != Me.Guid
                    && !u.IsFriendly
                    && u.IsHostile
                    && !u.IsTotem
                    && !u.IsCritter
                    && !u.IsNonCombatPet
                    && (u.Location.Distance(Me.CurrentTarget.Location) <= 10 || u.Location.Distance2D(Me.CurrentTarget.Location) <= 10))
                {
                    count++;
                }
            }
            return count;
        }
        #endregion

        #region MyDebuffTime
        //Used for checking how the time left on "my" debuff
        private int MyDebuffTime(String spellName, WoWUnit unit)
        {
            {
                if (unit.HasAura(spellName))
                {
                    var auras = unit.GetAllAuras();
                    foreach (var a in auras)
                    {
                        if (a.Name == spellName && a.CreatorGuid == Me.Guid)
                        {
                            return a.TimeLeft.Seconds;
                        }
                    }
                }
            }
            return 0;
        }
        #endregion

        #region DebuffTime
        //Used for checking debuff timers
        private int DebuffTime(String spellName, WoWUnit unit)
        {
            {
                if (unit.HasAura(spellName))
                {
                    var auras = unit.GetAllAuras();
                    foreach (var b in auras)
                    {
                        if (b.Name == spellName)
                        {
                            return b.TimeLeft.Seconds;
                        }
                    }
                }
            }
            return 0;
        }
        #endregion

        #region IsMyAuraActive
        //Used for checking auras that has no time
        private bool IsMyAuraActive(WoWUnit Who, String What)
        {
            {
                return Who.GetAllAuras().Where(p => p.CreatorGuid == Me.Guid && p.Name == What).FirstOrDefault() != null;
            }
        }
        #endregion

        #region rest

        public override bool NeedRest
        {
            get
            {
                if (HaltFeign() && StyxWoW.IsInWorld && !Me.IsGhost && Me.IsAlive && !Me.Mounted && !Me.IsFlying && !Me.IsOnTransport)
                {
                    if (BeastMasterSettings.Instance.RP && !Me.GotAlivePet && SpellManager.HasSpell("Revive Pet"))
                    {
                        if (CastSpell("Revive Pet"))
                        {
                            Logging.Write(Colors.Aqua, ">> Reviving Pet <<");
                        }
                        StyxWoW.SleepForLagDuration();
                    }
                    if (BeastMasterSettings.Instance.CP && Me.Pet == null && !Me.IsCasting)
                    {
                        if (BeastMasterSettings.Instance.PET == 1 && SpellManager.HasSpell("Call Pet 1"))
                        {
                            SpellManager.Cast("Call Pet 1");
                            StyxWoW.SleepForLagDuration();
                        }
                        if (BeastMasterSettings.Instance.PET == 2 && SpellManager.HasSpell("Call Pet 2"))
                        {
                            SpellManager.Cast("Call Pet 2");
                            StyxWoW.SleepForLagDuration();
                        }
                        if (BeastMasterSettings.Instance.PET == 3 && SpellManager.HasSpell("Call Pet 3"))
                        {
                            SpellManager.Cast("Call Pet 3");
                            StyxWoW.SleepForLagDuration();
                        }
                        if (BeastMasterSettings.Instance.PET == 4 && SpellManager.HasSpell("Call Pet 4"))
                        {
                            SpellManager.Cast("Call Pet 4");
                            StyxWoW.SleepForLagDuration();
                        }
                        if (BeastMasterSettings.Instance.PET == 5 && SpellManager.HasSpell("Call Pet 5"))
                        {
                            SpellManager.Cast("Call Pet 5");
                            StyxWoW.SleepForLagDuration();
                        }
                        StyxWoW.SleepForLagDuration();
                    }
                }
                return true;
            }
        }
        #endregion

        #region CombatStart

        private void AutoAttack()
        {
            if (!Me.IsAutoAttacking && Me.GotTarget && HaltFeign() && !SelfControl(Me.CurrentTarget))
            {
                Lua.DoString("StartAttack()");  
            }

        }
        #endregion

        #region Combat

        public override void Combat()
        {
/*
            if (SelfControl(Me.CurrentTarget))
            {
                Lua.DoString("StopAttack()");
                {
                    Logging.Write(Colors.Aqua, ">> Stop Everything! <<");
                }
                SpellManager.StopCasting();
                {
                    Logging.Write(Colors.Aqua, ">> Stop Everything! <<");
                }
            }
 */
            if (Me.GotTarget && Me.CurrentTarget.IsAlive && !Me.Mounted && HaltFeign())
            {
                if (Ultra())
                {
                    Lua.DoString("RunMacroText('/click ExtraActionButton1');");
                    SpellManager.StopCasting();
                    {
                        Logging.Write(Colors.Aqua, ">> Heroic Will! <<");
                    }
                } 
                if (UltraFL())
                {
                    Lua.DoString("RunMacroText('/click ExtraActionButton1');");
                    SpellManager.StopCasting();
                    {
                        Logging.Write(Colors.Aqua, ">> Heroic Will! <<");
                    }
                }
                if (DW())
                {
                    Lua.DoString("RunMacroText('/click ExtraActionButton1');");
                    SpellManager.StopCasting();
                    {
                        Logging.Write(Colors.Aqua, ">> Enter the dream! <<");
                    }
                }
                if (BeastMasterSettings.Instance.MP && Me.GotAlivePet && Me.Pet.HealthPercent <= BeastMasterSettings.Instance.MendHealth && !Me.Pet.ActiveAuras.ContainsKey("Mend Pet"))
                {
                    if (CastSpell("Mend Pet"))
                    {
                        Logging.Write(Colors.Aqua, ">> Mend Pet <<");
                    }
                }
                if (BeastMasterSettings.Instance.MDPet && Me.GotAlivePet && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && !Me.ActiveAuras.ContainsKey("Misdirection") 
                    && !WoWSpell.FromId(34477).Cooldown && !SpellManager.Spells["Misdirection"].Cooldown)
                {
                    Lua.DoString("RunMacroText('/cast [@pet,exists] Misdirection');");
                    {
                        Logging.Write(Colors.Crimson, ">> Misdirection on Pet <<");
                    }
                }
                ///////////////////////////////////////////Close Combat and Defense Mechanisms//////////////////////////////////////////////////////////////////////////////////////
                if (BeastMasterSettings.Instance.SMend && Me.CurrentHealth < Me.MaxHealth - 20000 && !WoWSpell.FromId(90361).Cooldown)
                {
                    Lua.DoString("RunMacroText(\"/cast [@" + Me.Name + "] Spirit Mend\")");
                    {
                        Logging.Write(Colors.Crimson, ">> Pet: Spirit Mend <<");
                    }
                }
                if (BeastMasterSettings.Instance.FDCBox == "1. High Threat" && Me.CurrentTarget.ThreatInfo.RawPercent > 90)
                {
                    if (CastSpell("Feign Death"))
                    {
                        Logging.Write(Colors.Aqua, ">> High Aggro, Feign Death <<");
                    }
                }
                if (BeastMasterSettings.Instance.FDCBox == "2. On Aggro" && Me.CurrentTarget.ThreatInfo.RawPercent > 90 
                    && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && (Me.CurrentTarget.IsCasting || Me.CurrentTarget.Distance < 10))
                {
                    if (CastSpell("Feign Death"))
                    {
                        Logging.Write(Colors.Aqua, ">> Aggro'ed, Feign Death <<");
                    }
                }
                if (BeastMasterSettings.Instance.FDCBox == "3. Low Health" && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.HealthPercent < 15)
                {
                    if (CastSpell("Feign Death"))
                    {
                        Logging.Write(Colors.Aqua, ">> Low Health, Feign Death <<");
                    }
                }


                if (BeastMasterSettings.Instance.FDCBox == "1 + 3" && Me.CurrentTarget.ThreatInfo.RawPercent > 90 && Me.HealthPercent < 15)
                {
                    if (CastSpell("Feign Death"))
                    {
                        Logging.Write(Colors.Aqua, ">> Thread + Low HP, Feign Death <<");
                    }
                }
                if (BeastMasterSettings.Instance.FDCBox == "2 + 3" && Me.CurrentTarget.ThreatInfo.RawPercent > 90 && Me.HealthPercent < 15 
                    && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && (Me.CurrentTarget.IsCasting || Me.CurrentTarget.Distance < 10))
                {
                    if (CastSpell("Feign Death"))
                    {
                        Logging.Write(Colors.Aqua, ">> Aggro + Low HP, Feign Death <<");
                    }
                }
             /*
                if (BeastMasterSettings.Instance.INT && SpellManager.HasSpell("Wyvern Sting") && Me.CurrentTarget.IsCasting && Me.CanInterruptCurrentSpellCast && Me.CurrentTarget.Distance > 5)
                {
                    if (CastSpell("Wyvern"))
                    {
                        Logging.Write(Colors.Aqua, ">> Wyvern Sting, Interrupt <<");
                    }
                }
             */
                if (BeastMasterSettings.Instance.ScatterBox == "1. Interrupt" && Me.CurrentTarget.Distance <= 20 && Me.CurrentTarget.IsCasting && Me.CanInterruptCurrentSpellCast)
                {
                    if (CastSpell("Scatter Shot"))
                    {
                        Logging.Write(Colors.Aqua, ">> Scatter Shot, Interrupt <<");
                    }
                }
                if (BeastMasterSettings.Instance.ScatterBox == "2. Defense" && Me.CurrentTarget.Distance <= 20 && Me.CurrentTarget.CurrentTargetGuid == Me.Guid)
                {
                    if (CastSpell("Scatter Shot"))
                    {
                        Logging.Write(Colors.Aqua, ">> Scatter Shot, Evade <<");
                    }
                }
                if (BeastMasterSettings.Instance.SLS && Me.CurrentTarget.IsCasting && Me.CanInterruptCurrentSpellCast && !SpellManager.Spells["Silencing Shot"].Cooldown)
                {
                    if (CastSpell("Silencing Shot"))
                    {
                        Logging.Write(Colors.Aqua, ">> Silencing Shot <<");
                    }
                }

                if (BeastMasterSettings.Instance.ScatterBox == "1 + 2" && ((Me.CurrentTarget.Distance <= 20 && Me.CurrentTarget.CurrentTargetGuid == Me.Guid) 
                    || (Me.CurrentTarget.IsCasting && Me.CanInterruptCurrentSpellCast)))
                {
                    if (CastSpell("Scatter Shot"))
                    {
                        Logging.Write(Colors.Aqua, ">> Scatter Shot <<");
                    }
                }
                if (BeastMasterSettings.Instance.CONC && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && !Me.CurrentTarget.HasAura("Concussive Shot") && Me.CurrentTarget.Distance < 25)
                {
                    if (CastSpell("Concussive Shot"))
                    {
                        Logging.Write(Colors.Aqua, ">> Concussive Shot <<");
                    }
                }
                if (BeastMasterSettings.Instance.IntimidateBox == "1. Interrupt" && Me.GotAlivePet && Me.Pet.Location.Distance(Me.CurrentTarget.Location) < 9 
                    && Me.CurrentTarget.IsCasting && Me.CanInterruptCurrentSpellCast && !SpellManager.Spells["Intimidation"].Cooldown)
                {
                    if (CastSpell("Intimidation"))
                    {
                        Logging.Write(Colors.Aqua, ">> Intimidation, Interrupt <<");
                    }
                }
                if (BeastMasterSettings.Instance.IntimidateBox == "2. Defense" && Me.GotAlivePet && Me.Pet.Location.Distance(Me.CurrentTarget.Location) < 9 
                    && Me.CurrentTarget.Distance <= 20 && Me.CurrentTarget.CurrentTargetGuid == Me.Guid)
                {
                    if (CastSpell("Intimidation"))
                    {
                        Logging.Write(Colors.Aqua, ">> Intimidation, Defense <<");
                    }
                }
                if (BeastMasterSettings.Instance.IntimidateBox == "1 + 2" && ((Me.CurrentTarget.Distance <= 20 && Me.CurrentTarget.CurrentTargetGuid == Me.Guid) 
                    || (Me.CurrentTarget.IsCasting && Me.CanInterruptCurrentSpellCast && !SpellManager.Spells["Intimidation"].Cooldown)))
                {
                    if (CastSpell("Intimidation"))
                    {
                        Logging.Write(Colors.Aqua, ">> Intimidation Stranger Danger! <<");
                    }
                }
                if (BeastMasterSettings.Instance.DETR && Me.HealthPercent < 20 && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && (Me.CurrentTarget.Distance < 10 || Me.CurrentTarget.IsCasting))
                {
                    if (CastSpell("Deterrence"))
                    {
                        Logging.Write(Colors.Aqua, ">> Deterrence <<");
                    }
                }
                /////////////////////////////////////////////////////Cooldowns/////////////////////////////////////////////////////////////////////////////////////////////////           
                if (BeastMasterSettings.Instance.RF && !Me.ActiveAuras.ContainsKey("Rapid Fire") && !Me.ActiveAuras.ContainsKey("The Beast Within") 
                    && !Me.ActiveAuras.ContainsKey("Bloodlust") && !Me.ActiveAuras.ContainsKey("Heroism") && !Me.ActiveAuras.ContainsKey("Ancient Hysteria") 
                    && !Me.ActiveAuras.ContainsKey("Time Warp") && (IsTargetBoss() || Me.CurrentTarget.Name == "Training Dummy"))
                {
                    if (CastSpell("Rapid Fire"))
                    {
                        Logging.Write(Colors.Aqua, ">> Rapid Fire <<");
                    }
                }
                if (Me.GotAlivePet && BeastMasterSettings.Instance.BWR && Me.Pet.Location.Distance(Me.CurrentTarget.Location) <= 25 && !Me.ActiveAuras.ContainsKey("Rapid Fire") 
                    && !Me.ActiveAuras.ContainsKey("The Beast Within") && !Me.ActiveAuras.ContainsKey("Bloodlust") && !Me.ActiveAuras.ContainsKey("Heroism") 
                    && !Me.ActiveAuras.ContainsKey("Ancient Hysteria") && !Me.ActiveAuras.ContainsKey("Time Warp") && (Me.CurrentTarget.MaxHealth > 200000 || Me.CurrentTarget.Name == "Training Dummy"))
                {
                    if (CastSpell("Bestial Wrath"))
                    {
                        Logging.Write(Colors.Aqua, ">> Bestial Wrath <<");
                    }
                }
                if (BeastMasterSettings.Instance.GLV 
                    && SpellManager.Spells["Rapid Fire"].CooldownTimeLeft.TotalSeconds > 1 
                    && SpellManager.Spells["Bestial Wrath"].CooldownTimeLeft.TotalSeconds > 1
                    && SpellManager.Spells["Lynx Rush"].CooldownTimeLeft.TotalSeconds > 1 
                    && SpellManager.Spells["Kill Command"].CooldownTimeLeft.TotalSeconds > 1 
                    && SpellManager.Spells["Dire Beast"].CooldownTimeLeft.TotalSeconds > 1)
                {
                    if (CastSpell("Readiness"))
                    {
                        Logging.Write(Colors.Aqua, ">> Readiness <<");
                    }
                }
                if (BeastMasterSettings.Instance.LB && IsTargetBoss())
                {
                    if (CastSpell("Lifeblood"))
                    {
                        Logging.Write(Colors.Aqua, ">> Lifeblood <<");
                    }
                }
                if (BeastMasterSettings.Instance.CW && Me.GotAlivePet && !WoWSpell.FromId(53434).Cooldown && !Me.ActiveAuras.ContainsKey("Rapid Fire") && IsTargetBoss())
                {
                    Lua.DoString("RunMacroText('/cast Call of the Wild');");
                    {
                        Logging.Write(Colors.Crimson, ">> Pet: Call of the Wild <<");
                    }
                }
                if (BeastMasterSettings.Instance.GE && IsTargetBoss() && StyxWoW.Me.Inventory.Equipped.Hands != null && StyxWoW.Me.Inventory.Equipped.Hands.Cooldown <= 0)
                {
                    Lua.DoString("RunMacroText('/use 10');");
                }
                if (BeastMasterSettings.Instance.T1 && IsTargetBoss() && StyxWoW.Me.Inventory.Equipped.Trinket1 != null && StyxWoW.Me.Inventory.Equipped.Trinket1.Cooldown <= 0)
                {
                    Lua.DoString("RunMacroText('/use 13');");
                }
                if (BeastMasterSettings.Instance.T2 && IsTargetBoss() && StyxWoW.Me.Inventory.Equipped.Trinket2 != null && StyxWoW.Me.Inventory.Equipped.Trinket2.Cooldown <= 0)
                {
                    Lua.DoString("RunMacroText('/use 14');");
                }
                //////////////////////////////////////////////////Racial Skills/////////////////////////////////////////////////////////////////////////////////////////
                if (BeastMasterSettings.Instance.RS && Me.Race == WoWRace.Troll && IsTargetBoss() && !SpellManager.Spells["Berserking"].Cooldown)
                {
                    Lua.DoString("RunMacroText('/Cast Berserking');");
                }
                if (BeastMasterSettings.Instance.RS && Me.Race == WoWRace.Orc && IsTargetBoss() && !SpellManager.Spells["Blood Fury"].Cooldown)
                {
                    Lua.DoString("RunMacroText('/Cast Blood Fury');");
                }
            }
            ///////////////////////////////////////////////Aspect Switching////////////////////////////////////////////////////////////////////////////////////////////
            if (BeastMasterSettings.Instance.AspectSwitching && HaltFeign() && Me.CurrentTarget != null && Me.CurrentTarget.IsAlive && !Me.Mounted)
            {
                if (!Me.IsMoving && !Me.Auras.ContainsKey("Aspect of the Iron Hawk"))
                {
                    if (CastSpell("Aspect of the Hawk"))
                    {
                        Logging.Write(Colors.Aqua, ">> Not moving - Aspect of the Iron Hawk <<");
                    }
                }
                if (Me.IsMoving && Me.Auras.ContainsKey("Aspect of the Iron Hawk") && Me.CurrentFocus < 60)
                {
                    if (CastSpell("Aspect of the Fox"))
                    {
                        Logging.Write(Colors.Aqua, ">> Moving - Aspect of the Fox <<");
                    }
                }
            }
            /////////////////////////////////////////////Beastmastery Rotation///////////////////////////////////////////////////////////////////////////////////////////
            if (Me.GotTarget && (addCount() < BeastMasterSettings.Instance.Mobs || (!BeastMasterSettings.Instance.MS && !BeastMasterSettings.Instance.TL)) && HaltFeign() && Me.CurrentTarget.IsAlive && !Me.Mounted)
            {
                if (BeastMasterSettings.Instance.HM && Me.CurrentTarget.HealthPercent > 25 && !Me.CurrentTarget.HasAura("Hunter's Mark") && IsTargetEasyBoss())
                {
                    if (CastSpell("Hunter's Mark"))
                    {
                        Logging.Write(Colors.Aqua, ">> Hunter's Mark <<");
                    }
                }
                if (Me.CurrentTarget.HealthPercent <= 20)
                {
                    if (CastSpell("Kill Shot"))
                    {
                        Logging.Write(Colors.Aqua, ">> Kill Shot <<");
                    }
                }
                if (BeastMasterSettings.Instance.SerpentBox == "Always" && (!IsMyAuraActive(Me.CurrentTarget, "Serpent Sting") || MyDebuffTime("Serpent Sting", Me.CurrentTarget) < 1))
                {
                    if (CastSpell("Serpent Sting"))
                    {
                        Logging.Write(Colors.Aqua, ">> Serpent Sting <<");
                    }
                }
                if (BeastMasterSettings.Instance.SerpentBox == "Sometimes" && !IsMyAuraActive(Me.CurrentTarget, "Serpent Sting") && Me.CurrentTarget.MaxHealth > Me.MaxHealth * 2 && Me.CurrentTarget.HealthPercent > 10)
                {
                    if (CastSpell("Serpent Sting"))
                    {
                        Logging.Write(Colors.Aqua, ">> Serpent Sting <<");
                    }
                }
                if (BeastMasterSettings.Instance.DB && ((Me.CurrentTarget.Level >= Me.Level && Me.CurrentTarget.CurrentHealth > Me.MaxHealth * 0.3) || Me.CurrentFocus < 20) || Me.CurrentTarget.Name == "Training Dummy")
                {
                    if (CastSpell("Dire Beast"))
                    {
                        Logging.Write(Colors.Aqua, ">> Dire Beast <<");
                    }
                }
                if (Me.GotAlivePet && Me.CurrentFocus >= 39 && Me.Pet.Location.Distance(Me.CurrentTarget.Location) <= 25)
                {
                    if (CastSpell("Kill Command"))
                    {
                        Logging.Write(Colors.Aqua, ">> Kill Command <<");
                    }
                }
                if (BeastMasterSettings.Instance.LXR && ((Me.CurrentTarget.MaxHealth > 250000 && Me.CurrentTarget.CurrentHealth > 75000) || Me.CurrentTarget.Name == "Training Dummy"))
                {
                    if (CastSpell("Lynx Rush"))
                    {
                        Logging.Write(Colors.Aqua, ">> Lynx Rush <<");
                    }
                }
                if (BeastMasterSettings.Instance.FF && Me.GotAlivePet && Me.Pet.Auras.ContainsKey("Frenzy") && !Me.ActiveAuras.ContainsKey("The Beast Within") 
                    && ((SpellManager.Spells["Bestial Wrath"].Cooldown && SpellManager.Spells["Bestial Wrath"].CooldownTimeLeft.TotalSeconds > 9) 
                    || (!SpellManager.Spells["Bestial Wrath"].Cooldown && (Me.CurrentTarget.MaxHealth <= 200000 || Me.ActiveAuras.ContainsKey("Rapid Fire")))))
                {
                    if (BeastMasterSettings.Instance.FFS == 5 && Me.Pet.Auras["Frenzy"].StackCount >= 5)
                    {
                        if (CastSpell("Focus Fire"))
                        {
                            Logging.Write(Colors.Aqua, ">> Focus Fire: 5 Stacks <<");
                        }
                    }
                    if (BeastMasterSettings.Instance.FFS == 4 && Me.Pet.Auras["Frenzy"].StackCount >= 4)
                    {
                        if (CastSpell("Focus Fire"))
                        {
                            Logging.Write(Colors.Aqua, ">> Focus Fire: 4 Stacks <<");
                        }
                    }
                    if (BeastMasterSettings.Instance.FFS == 3 && Me.Pet.Auras["Frenzy"].StackCount >= 3)
                    {
                        if (CastSpell("Focus Fire"))
                        {
                            Logging.Write(Colors.Aqua, ">> Focus Fire: 3 Stacks <<");
                        }
                    }
                    if (BeastMasterSettings.Instance.FFS == 2 && Me.Pet.Auras["Frenzy"].StackCount >= 2)
                    {
                        if (CastSpell("Focus Fire"))
                        {
                            Logging.Write(Colors.Aqua, ">> Focus Fire: 2 Stacks <<");
                        }
                    }
                    if (BeastMasterSettings.Instance.FFS == 1 && Me.Pet.Auras["Frenzy"].StackCount >= 1)
                    {
                        if (CastSpell("Focus Fire"))
                        {
                            Logging.Write(Colors.Aqua, ">> Focus Fire: 1 Stack <<");
                        }
                    }
                }
                if (BeastMasterSettings.Instance.FF && Me.Pet.Auras.ContainsKey("Frenzy") && Me.Pet.Auras["Frenzy"].StackCount >= 1 && DebuffTime("Frenzy", Me.Pet) < 2)
                {
                    if (CastSpell("Focus Fire"))
                    {
                        Logging.Write(Colors.Aqua, ">> Focus Fire: Running out of time <<");
                    }
                } 
                if (SpellManager.Spells["Kill Command"].Cooldown && SpellManager.Spells["Kill Command"].CooldownTimeLeft.TotalMilliseconds > 700 
                    && ((Me.CurrentFocus > 40 && SpellManager.Spells["Kill Command"].CooldownTimeLeft.TotalSeconds > 2) 
                    || (Me.CurrentFocus > 60 && SpellManager.Spells["Kill Command"].CooldownTimeLeft.TotalSeconds <= 2) 
                    || (Me.CurrentFocus > 20 && (!Me.GotAlivePet || Me.ActiveAuras.ContainsKey("The Beast Within")))))
                {
                    if (CastSpell("Arcane Shot"))
                    {
                        Logging.Write(Colors.Aqua, ">> Arcane Shot <<");
                    }
                }
                if (Me.CurrentFocus > 100 && Me.IsCasting && Me.CastingSpell.Name == "Cobra Shot" && Me.CurrentCastTimeLeft.TotalMilliseconds > 700)
                {
                    SpellManager.StopCasting();
                    Logging.Write(Colors.Red, ">> Stop Cobra Shot <<");
                }
                if (!Me.IsCasting && ((!SpellManager.CanCast("Kill Shot") && SpellManager.Spells["Kill Shot"].CooldownTimeLeft.TotalSeconds > 1) 
                    || Me.CurrentTarget.HealthPercent > 20) && ((Me.CurrentFocus >= 40 && SpellManager.Spells["Kill Command"].CooldownTimeLeft.TotalSeconds > 1) || Me.CurrentFocus < 39))
                {
                    if ((Me.CurrentFocus < BeastMasterSettings.Instance.FocusShots || (Me.ActiveAuras.ContainsKey("The Beast Within") && Me.CurrentFocus < BeastMasterSettings.Instance.FocusShots * 0.5)) 
                        || (MyDebuffTime("Serpent Sting", Me.CurrentTarget) < 9 && Me.CurrentFocus < 90))
                    {
                        Lua.DoString("RunMacroText('/cast Cobra Shot');");
                        {
                            Logging.Write(Colors.Crimson, ">> Cobra Shot <<");
                        }
                    }
                }
            }
            //////////////////////////////////////////////AoE Rotation here/////////////////////////////////////////////////////////////////////////////////////////////////
            if (addCount() >= BeastMasterSettings.Instance.Mobs && HaltFeign() && Me.GotTarget && Me.CurrentTarget.IsAlive && !Me.Mounted && (BeastMasterSettings.Instance.MS || BeastMasterSettings.Instance.TL))
            {
                if (Me.CurrentTarget.Distance >= 5)
                {
                    if (BeastMasterSettings.Instance.TL && SpellManager.Spells["Explosive Trap"].CooldownTimeLeft.TotalSeconds < 1 && Me.CurrentTarget.InLineOfSight)
                    {
                        if (!Me.HasAura("Trap Launcher"))
                        {
                            if (CastSpell("Trap Launcher"))
                            {
                            Logging.Write(Colors.Red, ">> Trap Launcher Activated! <<");                    
                            }
                        }
                        else if (Me.HasAura("Trap Launcher"))
                        {
                            Lua.DoString("CastSpellByName('Explosive Trap');");
                            {
                                SpellManager.ClickRemoteLocation(StyxWoW.Me.CurrentTarget.Location);
                                Logging.Write(Colors.Red, ">> Explosive Trap Launched! <<");
                            }
                        }
                    }
                }
                else if (Me.CurrentTarget.Distance < 5)
                {
                    if (BeastMasterSettings.Instance.TL && SpellManager.Spells["Explosive Trap"].CooldownTimeLeft.TotalSeconds < 1)
                    {
                        if (Me.HasAura("Trap Launcher"))
                        {
                            if (CastSpell("Trap Launcher"))
                            {
                                Logging.Write(Colors.Red, ">> Trap Launcher Deactivated! <<");
                            }
                        }

                        else if (!Me.HasAura("Trap Launcher"))
                        {
                            if (CastSpell("Explosive Trap"))
                            {
                                Logging.Write(Colors.Red, ">> Dropping Explosive Trap <<");
                            }
                        }
                    }              
                }
                if (BeastMasterSettings.Instance.FF && Me.GotAlivePet && Me.Pet.Auras.ContainsKey("Frenzy") && !Me.ActiveAuras.ContainsKey("The Beast Within")
                    && ((SpellManager.Spells["Bestial Wrath"].Cooldown && SpellManager.Spells["Bestial Wrath"].CooldownTimeLeft.TotalSeconds > 9)
                    || (!SpellManager.Spells["Bestial Wrath"].Cooldown && (Me.CurrentTarget.MaxHealth <= 200000 || Me.ActiveAuras.ContainsKey("Rapid Fire")))))
                {
                    if (BeastMasterSettings.Instance.FFS == 5 && Me.Pet.Auras["Frenzy"].StackCount >= 5)
                    {
                        if (CastSpell("Focus Fire"))
                        {
                            Logging.Write(Colors.Aqua, ">> Focus Fire: 5 Stacks <<");
                        }
                    }
                    if (BeastMasterSettings.Instance.FFS == 4 && Me.Pet.Auras["Frenzy"].StackCount >= 4)
                    {
                        if (CastSpell("Focus Fire"))
                        {
                            Logging.Write(Colors.Aqua, ">> Focus Fire: 4 Stacks <<");
                        }
                    }
                    if (BeastMasterSettings.Instance.FFS == 3 && Me.Pet.Auras["Frenzy"].StackCount >= 3)
                    {
                        if (CastSpell("Focus Fire"))
                        {
                            Logging.Write(Colors.Aqua, ">> Focus Fire: 3 Stacks <<");
                        }
                    }
                    if (BeastMasterSettings.Instance.FFS == 2 && Me.Pet.Auras["Frenzy"].StackCount >= 2)
                    {
                        if (CastSpell("Focus Fire"))
                        {
                            Logging.Write(Colors.Aqua, ">> Focus Fire: 2 Stacks <<");
                        }
                    }
                    if (BeastMasterSettings.Instance.FFS == 1 && Me.Pet.Auras["Frenzy"].StackCount >= 1)
                    {
                        if (CastSpell("Focus Fire"))
                        {
                            Logging.Write(Colors.Aqua, ">> Focus Fire: 1 Stack <<");
                        }
                    }
                }
                if (BeastMasterSettings.Instance.FF && Me.Pet.Auras.ContainsKey("Frenzy") && Me.Pet.Auras["Frenzy"].StackCount >= 1 && DebuffTime("Frenzy", Me.Pet) < 2)
                {
                    if (CastSpell("Focus Fire"))
                    {
                        Logging.Write(Colors.Aqua, ">> Focus Fire: Running out of time <<");
                    }
                }
                if (Me.GotAlivePet && BeastMasterSettings.Instance.BWR && Me.Pet.Location.Distance(Me.CurrentTarget.Location) <= 25 && !Me.ActiveAuras.ContainsKey("The Beast Within"))
                {
                    if (CastSpell("Bestial Wrath"))
                    {
                        Logging.Write(Colors.Aqua, ">> Bestial Wrath, AoE <<");
                    }
                }
                if (Me.CurrentTarget.HealthPercent < 20 && (Me.CurrentFocus < 40 || (Me.ActiveAuras.ContainsKey("The Beast Within") && Me.CurrentFocus < 20)))
                {
                    if (CastSpell("Kill Shot"))
                    {
                        Logging.Write(Colors.Aqua, ">> Kill Shot <<");
                    }
                }
                if (Me.CurrentPendingCursorSpell == null && BeastMasterSettings.Instance.MS && (!Me.Pet.HasAura("Beast Cleave") || DebuffTime("Beast Cleave", Me.Pet) < 1 
                    || Me.CurrentFocus > 70 || Me.ActiveAuras.ContainsKey("The Beast Within")))
                {
                    if (CastSpell("Multi-Shot"))
                    {
                        Logging.Write(Colors.Aqua, ">> Multi-Shot <<");
                    }
                }
                if (BeastMasterSettings.Instance.AOEDB && Me.CurrentFocus < 40 && !Me.ActiveAuras.ContainsKey("The Beast Within"))
                {
                    if (CastSpell("Dire Beast"))
                    {
                        Logging.Write(Colors.Aqua, ">> Dire Beast, AoE <<");
                    }
                }
                if (BeastMasterSettings.Instance.AOELR && Me.CurrentFocus < 40 && !Me.ActiveAuras.ContainsKey("The Beast Within") && ((Me.CurrentTarget.MaxHealth > 250000 && Me.CurrentTarget.CurrentHealth > 75000) || Me.CurrentTarget.Name == "Training Dummy"))
                {
                    if (CastSpell("Lynx Rush"))
                    {
                        Logging.Write(Colors.Aqua, ">> Lynx Rush, AoE <<");
                    }
                }
                if (BeastMasterSettings.Instance.BRA && !WoWSpell.FromId(93433).Cooldown && Me.Pet.Location.Distance(Me.CurrentTarget.Location) < 5)
                {
                    Lua.DoString("RunMacroText('/cast Burrow Attack');");
                    {
                        Logging.Write(Colors.Crimson, ">> Pet AoE: Burrow Attack <<");
                    }
                }
                if (!SpellManager.CanCast("Kill Shot") && (Me.CurrentFocus < 40 || (Me.ActiveAuras.ContainsKey("The Beast Within") && Me.CurrentFocus < 20)))
                {
                    Lua.DoString("RunMacroText('/cast Cobra Shot');");
                    {
                        Logging.Write(Colors.Crimson, ">> AoE Cobra Shot <<");
                    }
                }
            }
        }
        #endregion
    }
}