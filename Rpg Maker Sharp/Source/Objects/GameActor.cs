#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using RpgSharp.Data;
using RpgSharp.Management;

namespace RpgSharp.Objects;

public class GameActor : GameBattler
{
    public int ActorId { get; protected set; }
    public DataActor Actor => DataManager.DataActors[ActorId];
    public string Name { get; protected set; }
    public string Nickname { get; protected set; }
    public string Profile { get; protected set; }
    public string CharacterName { get; protected set; }
    public int CharacterIndex { get; protected set; }
    public int ClassId { get; protected set; }
    public string FaceName { get; protected set; }
    public int FaceIndex { get; protected set; }
    public string BattlerName { get; protected set; }
    public int Level { get; protected set; }

    protected Dictionary<int, int> Exp;
    protected List<int> SkillsContainer;
    protected List<GameItem> EquipsContainer;
    protected int ActionInputIndex;
    protected GameItem LastMenuSkill;
    protected GameItem LastBattleSkill;
    protected string LastCommandSymbol;

    protected Dictionary<int, int> StateSteps;


    public GameActor(int id) : base()
    {
        Setup(id);
    }

    public void Setup(int id)
    {
        var actor = DataManager.DataActors[id];
        ActorId = id;
        Name = actor.Name;
        Nickname = actor.Nickname;
        Profile = actor.Profile;
        ClassId = actor.ClassId;
        Level = actor.InitialLevel;
        InitImages();
        InitExp();
        InitSkills();
        InitEquips(actor.Equips.ToList());
        ClearParamPlus();
        RecoverAll();
    }


    protected override void ClearStates()
    {
        base.ClearStates();
        StateSteps.Clear();
    }

    public override void EraseState(int stateId)
    {
        base.EraseState(stateId);
        StateSteps.Remove(stateId);
    }

    public override void ResetStateCounts(int stateId)
    {
        base.ResetStateCounts(stateId);
        var steps = DataManager.DataStates[stateId].StepsToRemove;
        StateSteps.Add(stateId, steps);
    }

    protected void InitImages()
    {
        CharacterName = Actor.CharacterName;
        CharacterIndex = Actor.CharacterIndex;
        FaceName = Actor.FaceName;
        FaceIndex = Actor.FaceIndex;
        BattlerName = Actor.BattlerName;
    }

    public int ExpForLevel(int level)
    {
        var c = CurrentClass();
        var basis = c.ExpParams[0];
        var extra = c.ExpParams[1];
        var accA = c.ExpParams[2];
        var accB = c.ExpParams[3];
        return (int)Math.Round(
            (basis * Math.Pow(level - 1, 0.9 + accA / 250.0) * level * (level + 1)) /
            (6 + Math.Pow(level, 2) / 50.0 / accB) +
            (level - 1) * extra
        );
    }

    public void InitExp()
    {
        Exp[ClassId] = CurrentLevelExp();
    }

    public int CurrentExp()
    {
        return Exp[ClassId];
    }

    public int CurrentLevelExp()
    {
        return ExpForLevel(Level);
    }

    public int NextLevelExp()
    {
        return ExpForLevel(Level + 1);
    }

    public int NextRequiredExp()
    {
        return NextLevelExp() - CurrentExp();
    }

    // since its a very common method that get patched by plugin devs we making it virtual 
    // TODO : implement notetag Parsing
    public virtual int MaxLevel()
    {
        return Actor.MaxLevel;
    }

    public bool IsMaxLevel()
    {
        return Level >= MaxLevel();
    }
    
    public void InitSkills()
    {
        SkillsContainer = [];
        foreach (var learning in CurrentClass().Learnings)
        {
            if (learning.Level <= Level)
            {
                LearnSkill(learning.SkillId);
            }
        }
    }

    public void InitEquips(List<int> equipments)
    {
        var slots = EquipSlots();
        var maxSlots = slots.Count;
        EquipsContainer = [];
        for (var i = 0; i < maxSlots; i++)
        {
            EquipsContainer[i] = new GameItem();
        }

        for (var i = 0; i < EquipsContainer.Count; i++)
        {
            if (i < maxSlots)
            {
                EquipsContainer[i].SetEquip(equipments[i] == 1, equipments[i]);
            }

            ReleaseUnequippableItems(true);
            Refresh();
        }
    }

    public List<int> EquipSlots()
    {
        var slots = new List<int>();
        for (var i = 1; i < DataManager.DataSystem.EquipTypes.Length; i++)
        {
            slots.Add(i);
        }

        if (slots.Count >= 2 && IsDualWield())
        {
            slots[1] = 1;
        }

        return slots;
    }

    public virtual List<DataEquipable?> Equips()
    {
        return EquipsContainer.Select(item => item.Object() as DataEquipable).ToList();
    }

    public virtual List<DataWeapon> Weapons()
    {
        return Equips()
            .OfType<DataWeapon>()
            .ToList();
    }

    public virtual List<DataArmor> Armors()
    {
        return Equips()
            .OfType<DataArmor>()
            .ToList();
    }

    public bool HasWeapon(DataWeapon weapon)
    {
        return Weapons().Contains(weapon);
    }

    public bool HasArmor(DataArmor armor)
    {
        return Armors().Contains(armor);
    }

    public bool IsEquipChangeOk(int slotId)
    {
        return (
            !IsEquipTypeLocked(EquipSlots()[slotId]) &&
            IsEquipTypeSealed(EquipSlots()[slotId])
        );
    }

    public virtual void ChangeEquip(int slotId, DataEquipable? item)
    {
        if (TradeItemWithParty(item, Equips()[slotId]) &&
            (item == null || EquipSlots()[slotId] == item.EtypeId))
        {
            EquipsContainer[slotId].SetObject(item);
            Refresh();
        }
    }

    public virtual void ForceChangeEquip(int slotId, DataEquipable? item)
    {
        EquipsContainer[slotId].SetObject(item);
        ReleaseUnequippableItems(true);
        Refresh();
    }

    public virtual bool TradeItemWithParty(DataEquipable? newItem, DataEquipable? oldItem)
    {
        if (newItem != null && !DataManager.GameParty.HasItem(newItem))
            return false;

        DataManager.GameParty.GainItem(oldItem, 1);
        DataManager.GameParty.LoseItem(newItem, 1);
        return true;
    }

    public virtual void ChangeEquipById(int etypeId, int itemId)
    {
        var slotId = etypeId - 1;
        if (EquipSlots()[slotId] == 1)
            ChangeEquip(slotId, DataManager.DataWeapons[itemId]);
        else
            ChangeEquip(slotId, DataManager.DataArmors[itemId]);
    }

    public virtual bool IsEquipped(DataEquipable? item)
    {
        return Equips().Contains(item);
    }

    public virtual void DiscardEquip(DataEquipable? item)
    {
        var slotId = Equips().IndexOf(item);
        if (slotId >= 0)
            EquipsContainer[slotId].SetObject(null);
    }

    public virtual void ReleaseUnequippableItems(bool forcing)
    {
        while (true)
        {
            var slots = EquipSlots();
            var equips = Equips();
            var changed = false;

            for (int i = 0; i < equips.Count; i++)
            {
                var item = equips[i];
                if (item != null && (!CanEquip(item) || item.EtypeId != slots[i]))
                {
                    if (!forcing)
                        TradeItemWithParty(null, item);

                    EquipsContainer[i].SetObject(null);
                    changed = true;
                }
            }

            if (!changed) break;
        }
    }

    public virtual void ClearEquipments()
    {
        var maxSlots = EquipSlots().Count;
        for (var i = 0; i < maxSlots; i++)
        {
            if (IsEquipChangeOk(i))
                ChangeEquip(i, null);
        }
    }

    public virtual void OptimizeEquipments()
    {
        var maxSlots = EquipSlots().Count;
        ClearEquipments();
        for (int i = 0; i < maxSlots; i++)
        {
            if (IsEquipChangeOk(i))
                ChangeEquip(i, BestEquipItem(i));
        }
    }

    public virtual DataEquipable? BestEquipItem(int slotId)
    {
        var etypeId = EquipSlots()[slotId];
        var items = DataManager.GameParty.EquipItems()
            .Where(item => item.EtypeId == etypeId && CanEquip(item))
            .ToList();

        DataEquipable? bestItem = null;
        var bestPerformance = -1000;

        foreach (var item in items)
        {
            var performance = CalcEquipItemPerformance(item);
            if (performance > bestPerformance)
            {
                bestPerformance = performance;
                bestItem = item;
            }
        }

        return bestItem;
    }

    public virtual int CalcEquipItemPerformance(DataEquipable item)
    {
        return item.Params.Sum();
    }

    public override bool IsSkillWtypeOk(DataSkill skill)
    {
        var wtypeId1 = skill.RequiredWtypeId1;
        var wtypeId2 = skill.RequiredWtypeId2;
        return (wtypeId1 == 0 && wtypeId2 == 0) ||
               (wtypeId1 > 0 && IsWtypeEquipped(wtypeId1)) ||
               (wtypeId2 > 0 && IsWtypeEquipped(wtypeId2));
    }

    public virtual bool IsWtypeEquipped(int wtypeId)
    {
        return Weapons().Any(weapon => weapon.WtypeId == wtypeId);
    }

    public override void Refresh()
    {
        ReleaseUnequippableItems(false);
        base.Refresh();
    }

    public override void Hide()
    {
        base.Hide();
        DataManager.GameTemp.RequestBattleRefresh();
    }

    public override GameParty FriendsUnit()
    {
        return DataManager.GameParty;
    }

    public override GameTroop OpponentsUnit()
    {
        return DataManager.GameTroop;
    }

    public virtual int Index()
    {
        return DataManager.GameParty.Members().IndexOf(this);
    }

    public bool IsBattleMember()
    {
        return DataManager.GameParty.BattleMembers().Contains(this);
    }

    public bool IsFormationChangeOk()
    {
        return true;
    }

    public DataClass CurrentClass()
    {
        return DataManager.DataClasses[ClassId];
    }

    public virtual List<int> SkillTypes()
    {
        return AddedSkillTypes()
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    public virtual List<DataSkill> Skills()
    {
        var list = new List<DataSkill>();
        foreach (var id in SkillsContainer.Concat(AddedSkills()))
        {
            var skill = DataManager.DataSkills[id];
            if (!list.Contains(skill))
                list.Add(skill);
        }

        return list;
    }

    public virtual List<DataSkill> UsableSkills()
    {
        return Skills().Where(skill => CanUse(skill)).ToList();
    }

    public override List<IWithTraits> TraitObjects()
    {
        var objects = base.TraitObjects().ToList();
        objects.Add(Actor);
        objects.Add(CurrentClass());

        var equips = Equips();
        foreach (var item in equips)
        {
            if (item != null)
                objects.Add(item);
        }

        return objects;
    }

    public override List<int> AttackElements()
    {
        var set = base.AttackElements();
        if (HasNoWeapons() && !set.Contains(BareHandsElementId()))
            set.Add(BareHandsElementId());
        return set;
    }

    public virtual bool HasNoWeapons()
    {
        return Weapons().Count == 0;
    }

    public virtual int BareHandsElementId()
    {
        return 1;
    }

    public override int ParamBase(int paramId)
    {
        return base.ParamBase(paramId);
    }

    public override int ParamPlus(int paramId)
    {
        var value = base.ParamPlus(paramId);
        foreach (var item in Equips())
        {
            if (item is not null)
            {
                value += item.Params[paramId];
            }
        }

        return value;
    }

    public virtual int AttackAnimationId1()
    {
        if (HasNoWeapons())
            return BareHandsAnimationId();

        var weapons = Weapons();
        return weapons.Count > 0 ? weapons[0].AnimationId : 0;
    }

    public virtual int AttackAnimationId2()
    {
        var weapons = Weapons();
        return weapons.Count > 1 ? weapons[1].AnimationId : 0;
    }

    public virtual int BareHandsAnimationId()
    {
        return 1;
    }

    public virtual void ChangeExp(int exp, bool show)
    {
        this.Exp[ClassId] = Math.Max(exp, 0);
        var lastLevel = Level;
        var lastSkills = Skills();

        while (!IsMaxLevel() && CurrentExp() >= NextLevelExp())
            LevelUp();

        while (CurrentExp() < CurrentLevelExp())
            LevelDown();

        if (show && Level > lastLevel)
            DisplayLevelUp(FindNewSkills(lastSkills));

        Refresh();
    }

    public virtual void LevelUp()
    {
        Level++;
        foreach (var learning in CurrentClass().Learnings)
        {
            if (learning.Level == Level)
                LearnSkill(learning.SkillId);
        }
    }

    public virtual void LevelDown()
    {
        Level--;
    }

    public virtual List<DataSkill> FindNewSkills(List<DataSkill> lastSkills)
    {
        var newSkills = Skills();
        foreach (var skill in lastSkills)
            newSkills.Remove(skill);
        return newSkills;
    }

    public virtual void DisplayLevelUp(List<DataSkill> newSkills)
    {
        var text = string.Format(TextManager.LevelUp, Name, TextManager.Level, Level);
        DataManager.GameMessage.NewPage();
        DataManager.GameMessage.Add(text);
        foreach (var skill in newSkills)
            DataManager.GameMessage.Add(string.Format(TextManager.ObtainSkill, skill.Name));
    }
    
    public virtual void GainExp(int exp)
    {
        var newExp = CurrentExp() + (int)Math.Round(exp * FinalExpRate());
        ChangeExp(newExp, ShouldDisplayLevelUp());
    }

    public virtual double FinalExpRate()
    {
        return Exr * (IsBattleMember() ? 1 : BenchMembersExpRate());
    }

    public virtual double BenchMembersExpRate()
    {
        return DataManager.DataSystem.OptExtraExp ? 1 : 0;
    }

    public virtual bool ShouldDisplayLevelUp()
    {
        return true;
    }

    public virtual void ChangeLevel(int level, bool show)
    {
        level = Math.Clamp(level, 1, MaxLevel());
        ChangeExp(ExpForLevel(level), show);
    }

    public virtual void LearnSkill(int skillId)
    {
        if (!IsLearnedSkill(skillId))
        {
            SkillsContainer.Add(skillId);
            SkillsContainer.Sort();
        }
    }

    public virtual void ForgetSkill(int skillId)
    {
        SkillsContainer.Remove(skillId);
    }

    public virtual bool IsLearnedSkill(int skillId)
    {
        return SkillsContainer.Contains(skillId);
    }

    public virtual bool HasSkill(int skillId)
    {
        return Skills().Contains(DataManager.DataSkills[skillId]);
    }

    public virtual void ChangeClass(int classId, bool keepExp)
    {
        if (keepExp)
            Exp[classId] = CurrentExp();
    
        ClassId = classId;
        Level = 0;
        ChangeExp(Exp.GetValueOrDefault(ClassId, 0), false);
        Refresh();
    }
}