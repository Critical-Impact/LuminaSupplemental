using System;
using System.Collections.Generic;
using System.Linq;

using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Extensions;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Extensions;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public class RelicWeaponStep : GeneratorStep
{
    private readonly ExcelSheet<RelicItem> relicItemSheet;
    private readonly ExcelSheet<ClassJob> classJobSheet;
    private readonly ExcelSheet<Quest> questSheet;
    private readonly ExcelSheet<CustomTalk> customTalkSheet;
    private readonly ExcelSheet<AnimaWeaponItem> animaWeaponItemSheet;
    private readonly ExcelSheet<Item> itemSheet;
    private readonly ClassJob GLA;
    private readonly ClassJob PGL;
    private readonly ClassJob MRD;
    private readonly ClassJob LNC;
    private readonly ClassJob ARC;
    private readonly ClassJob CNJ;
    private readonly ClassJob THM;
    private readonly ClassJob SMN;
    private readonly ClassJob SCH;
    private readonly ClassJob ROG;
    private readonly ClassJob MCH;
    private readonly ClassJob RDM;
    private readonly ClassJob DRK;
    private readonly ClassJob SAM;
    private readonly ClassJob AST;
    private readonly ClassJob GNB;
    private readonly ClassJob DNC;
    private readonly ClassJob RPR;
    private readonly ClassJob SGE;
    private readonly ClassJob PCT;
    private readonly ClassJob VPR;
    private readonly Dictionary<string, uint> itemsByName;

    public override Type OutputType => typeof(RelicWeapon);

    public override string FileName => "RelicWeapon.csv";

    public override string Name => "Relic Weapons";

    public RelicWeaponStep(ExcelSheet<RelicItem> relicItemSheet, ExcelSheet<ClassJob> classJobSheet, ExcelSheet<Quest> questSheet, ExcelSheet<CustomTalk> customTalkSheet, ExcelSheet<AnimaWeaponItem> animaWeaponItemSheet, DataCacher dataCacher, ExcelSheet<Item> itemSheet)
    {
        this.relicItemSheet = relicItemSheet;
        this.classJobSheet = classJobSheet;
        this.questSheet = questSheet;
        this.customTalkSheet = customTalkSheet;
        this.animaWeaponItemSheet = animaWeaponItemSheet;
        this.itemSheet = itemSheet;
        var bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.itemsByName = dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
        var classes = classJobSheet.Where(c => !c.Abbreviation.IsEmpty).ToDictionary(c => c.Abbreviation.ToImGuiString());
        GLA = classes["GLA"];
        PGL = classes["PGL"];
        MRD = classes["MRD"];
        LNC = classes["LNC"];
        ARC = classes["ARC"];
        CNJ = classes["CNJ"];
        THM = classes["THM"];
        SMN = classes["SMN"];
        SCH = classes["SCH"];
        ROG = classes["ROG"];
        DRK = classes["DRK"];
        RDM = classes["RDM"];
        MCH = classes["MCH"];
        SAM = classes["SAM"];
        AST = classes["AST"];
        GNB = classes["GNB"];
        DNC = classes["DNC"];
        RPR = classes["RPR"];
        SGE = classes["SGE"];
        PCT = classes["PCT"];
        VPR = classes["VPR"];
    }

    public List<RelicWeapon> ProcessWeapons()
    {
        uint? gladiatorParent = null;
        uint? pugilistParent = null;
        uint? maraurderParent = null;
        uint? lancerParent = null;
        uint? archerParent = null;
        uint? conjurerParent = null;
        uint? thaumaturgeParent = null;
        uint? summonerParent = null;
        uint? scholarParent = null;
        uint? rogueParent = null;

        //Zodiac Weapons
        Dictionary<uint, uint> itemToQuest = new Dictionary<uint, uint>();
        //Base Zodiac Weapon
        foreach (var quest in this.questSheet.Where(c => c.QuestParams.Any(c => c.ScriptInstruction.ToImGuiString() == "LOC_ITEM1")))
        {
            foreach (var questParam in quest.QuestParams)
            {
                if (questParam.ScriptInstruction == "LOC_ITEM1")
                {
                    itemToQuest.TryAdd(questParam.ScriptArg, quest.RowId);
                }
            }
        }

        //No Quest for Zenith

        //Atma Zodiac Weapon

        foreach (var customTalk in this.customTalkSheet.Where(c => c.Script.Any(d => d.ScriptInstruction.ToImGuiString().StartsWith("ITEM_EPIC_"))))
        {
            var questId = customTalk.Script.First(c => c.ScriptInstruction.ToImGuiString().Contains("JOB_REL")).ScriptArg;
            var mainItemId = customTalk.Script.First(c => c.ScriptInstruction.ToImGuiString().StartsWith("ITEM_EPIC_")).ScriptArg;
            var offhandItemId = customTalk.Script.FirstOrNull(c => c.ScriptInstruction.ToImGuiString().StartsWith("ITEM_EPIC_") && c.ScriptInstruction.ToImGuiString().Contains("SUB"))?.ScriptArg ?? 0;
            itemToQuest.TryAdd(mainItemId, questId);
            if (offhandItemId != 0)
            {
                itemToQuest.TryAdd(offhandItemId, questId);
            }
        }

        //Hardcode quest for Animus

        uint getQuestId(RelicWeaponType weaponType, uint itemId)
        {
            var questId = itemToQuest.TryGetValue(itemId, out var value) ? value : 0;
            if (weaponType == RelicWeaponType.ZodiacAnimus)
            {
                questId = 66972;
            }

            if (weaponType == RelicWeaponType.ZodiacNovus)
            {
                questId = 66998;
            }

            if (weaponType == RelicWeaponType.ZodiacNexus)
            {
                questId = 65742;
            }

            if (weaponType == RelicWeaponType.ZodiacZodiac)
            {
                questId = 65892;
            }

            if (weaponType == RelicWeaponType.ZodiacZeta)
            {
                questId = 66096;
            }

            return questId;
        }


        var relicWeapons = new List<RelicWeapon>();

        var rowId = 1u;

        var relicWeaponType = 1;
        var relicWeaponCategory = 1;

        var emptyItem = new RowRef<Item>(this.classJobSheet.Module, 0);

        foreach (var relicItem in this.relicItemSheet)
        {
            var stage = relicItem.RowId + 1;

            var weaponCategory = (RelicWeaponCategory)relicWeaponCategory;
            var weaponType = (RelicWeaponType)relicWeaponType;

            var item = relicItem.GladiatorItem;
            var offhand = relicItem.ShieldItem;
            var questId = getQuestId(weaponType, item.RowId);
            relicWeapons.Add(new RelicWeapon(rowId, item.RowId, offhand.RowId, stage, this.GLA.RowId, weaponCategory, weaponType, questId, gladiatorParent ?? 0));
            gladiatorParent = rowId;
            rowId++;

            item = relicItem.PugilistItem;
            offhand = emptyItem;
            questId = getQuestId(weaponType, item.RowId);
            relicWeapons.Add(new RelicWeapon(rowId, item.RowId, offhand.RowId, stage, this.PGL.RowId, weaponCategory, weaponType, questId, pugilistParent ?? 0));
            pugilistParent = rowId;
            rowId++;

            item = relicItem.MarauderItem;
            offhand = emptyItem;
            questId = getQuestId(weaponType, item.RowId);
            relicWeapons.Add(new RelicWeapon(rowId, item.RowId, offhand.RowId, stage, this.MRD.RowId, weaponCategory, weaponType, questId, maraurderParent ?? 0));
            maraurderParent = rowId;
            rowId++;

            item = relicItem.LancerItem;
            offhand = emptyItem;
            questId = getQuestId(weaponType, item.RowId);
            relicWeapons.Add(new RelicWeapon(rowId, item.RowId, offhand.RowId, stage, this.LNC.RowId, weaponCategory, weaponType, questId, lancerParent ?? 0));
            lancerParent = rowId;
            rowId++;

            item = relicItem.ArcherItem;
            offhand = emptyItem;
            questId = getQuestId(weaponType, item.RowId);
            relicWeapons.Add(new RelicWeapon(rowId, item.RowId, offhand.RowId, stage, this.ARC.RowId, weaponCategory, weaponType, questId, archerParent ?? 0));
            archerParent = rowId;
            rowId++;

            item = relicItem.ConjurerItem;
            offhand = emptyItem;
            questId = getQuestId(weaponType, item.RowId);
            relicWeapons.Add(new RelicWeapon(rowId, item.RowId, offhand.RowId, stage, this.CNJ.RowId, weaponCategory, weaponType, questId, conjurerParent ?? 0));
            conjurerParent = rowId;
            rowId++;

            item = relicItem.ThaumaturgeItem;
            offhand = emptyItem;
            questId = getQuestId(weaponType, item.RowId);
            relicWeapons.Add(new RelicWeapon(rowId, item.RowId, offhand.RowId, stage, this.THM.RowId, weaponCategory, weaponType, questId, thaumaturgeParent ?? 0));
            thaumaturgeParent = rowId;
            rowId++;

            item = relicItem.ArcanistSMNItem;
            offhand = emptyItem;
            questId = getQuestId(weaponType, item.RowId);
            relicWeapons.Add(new RelicWeapon(rowId, item.RowId, offhand.RowId, stage, this.SMN.RowId, weaponCategory, weaponType, questId, summonerParent ?? 0));
            summonerParent = rowId;
            rowId++;

            item = relicItem.ArcanistSCHItem;
            offhand = emptyItem;
            questId = getQuestId(weaponType, item.RowId);
            relicWeapons.Add(new RelicWeapon(rowId, item.RowId, offhand.RowId, stage, this.SCH.RowId, weaponCategory, weaponType, questId, scholarParent ?? 0));
            scholarParent = rowId;
            rowId++;

            item = relicItem.RogueItem;
            offhand = emptyItem;
            questId = getQuestId(weaponType, item.RowId);
            relicWeapons.Add(new RelicWeapon(rowId, item.RowId, offhand.RowId, stage, this.ROG.RowId, weaponCategory, weaponType, questId, rogueParent ?? 0));
            rogueParent = rowId;
            rowId++;

            relicWeaponType++;
        }

        //Anima Weapons
        relicWeaponCategory++;
        var groupedAnimaWeapons = this.animaWeaponItemSheet.Where(c => c.RowId != 0).SelectMany(c => c.Item).GroupBy(c => c.Value.ClassJobCategory.Value.Name).ToList();
        foreach (var animaWeaponGroup in groupedAnimaWeapons)
        {
            var classJobName = animaWeaponGroup.Key;
            var classJob = this.classJobSheet.First(c => c.Abbreviation == classJobName);
            for (var index = 0; index < animaWeaponGroup.ToList().Count; index++)
            {
                uint itemId;
                uint offhand;
                int stage = index;
                if (classJobName.ToImGuiString() == "PLD")
                {
                    var animaWeapon = animaWeaponGroup.ToList()[index];
                    itemId = animaWeapon.RowId;
                    index++;
                    animaWeapon = animaWeaponGroup.ToList()[index];
                    offhand = animaWeapon.RowId;
                    stage = index / 2;
                }
                else
                {
                    var animaWeapon = animaWeaponGroup.ToList()[index];
                    itemId = animaWeapon.RowId;
                    offhand = 0;
                }

                var weaponCategory = (RelicWeaponCategory)relicWeaponCategory;
                var weaponType = (RelicWeaponType)relicWeaponType + stage;
                uint questId = 0;

                relicWeapons.Add(new RelicWeapon(rowId, itemId, offhand, (uint)stage, classJob.RowId, weaponCategory, weaponType, questId, rogueParent ?? 0));
                rowId++;
            }
        }

        //Eurekan Weapons
        relicWeaponType = (int)RelicWeaponType.EurekanAntiquated;
        relicWeaponCategory = (int)RelicWeaponCategory.Eurekan;

        //SQ don't give us many choices here, the game does not contain enough data for us to work this out.
        var startItemMap = new Dictionary<ClassJob, List<List<string>>>()
        {
            {
                this.GLA, new List<List<string>>
                {
                    new() { "Antiquated Galatyn", "Antiquated Evalach" },
                    new() { "Galatyn", "Evalach" },
                    new() { "Galatyn +1", "Evalach +1" },
                    new() { "Galatyn +2", "Evalach +2" },
                    new() { "Galatyn Anemos", "Evalach Anemos" },
                    new() { "Galatyn Pagos", "Evalach Pagos" },
                    new() { "Galatyn Pagos +1", "Evalach Pagos +1" },
                    new() { "Elemental Sword", "Elemental Shield" },
                    new() { "Elemental Sword +1", "Elemental Shield +1" },
                    new() { "Elemental Sword +2", "Elemental Shield +2" },
                    new() { "Pyros Sword", "Pyros Shield" },
                    new() { "Hydatos Sword", "Hydatos Shield" },
                    new() { "Hydatos Sword +1", "Hydatos Shield +1" },
                    new() { "Antea", "Bellerophon" },
                    new() { "Antea Eureka", "Bellerophon Eureka" },
                    new() { "Antea Physeos", "Bellerophon Physeos" },
                }
            },
            {
                this.MRD, new List<List<string>>
                {
                    new() { "Antiquated Farsha" },
                    new() { "Farsha" },
                    new() { "Farsha +1" },
                    new() { "Farsha +2" },
                    new() { "Farsha Anemos" },
                    new() { "Farsha Pagos" },
                    new() { "Farsha Pagos +1" },
                    new() { "Elemental Battleaxe" },
                    new() { "Elemental Battleaxe +1" },
                    new() { "Elemental Battleaxe +2" },
                    new() { "Pyros Battleaxe" },
                    new() { "Hydatos Battleaxe" },
                    new() { "Hydatos Battleaxe +1" },
                    new() { "Shamash" },
                    new() { "Shamash Eureka" },
                    new() { "Shamash Physeos" },
                }
            },
            {
                this.DRK, new List<List<string>>
                {
                    new() { "Antiquated Caladbolg" },
                    new() { "Caladbolg" },
                    new() { "Caladbolg +1" },
                    new() { "Caladbolg +2" },
                    new() { "Caladbolg Anemos" },
                    new() { "Caladbolg Pagos" },
                    new() { "Caladbolg Pagos +1" },
                    new() { "Elemental Guillotine" },
                    new() { "Elemental Guillotine +1" },
                    new() { "Elemental Guillotine +2" },
                    new() { "Pyros Guillotine" },
                    new() { "Hydatos Guillotine" },
                    new() { "Hydatos Guillotine +1" },
                    new() { "Xiphias" },
                    new() { "Xiphias Eureka" },
                    new() { "Xiphias Physeos" },
                }
            },
            {
                this.LNC, new List<List<string>>
                {
                    new() { "Antiquated Ryunohige" },
                    new() { "Ryunohige" },
                    new() { "Ryunohige +1" },
                    new() { "Ryunohige +2" },
                    new() { "Ryunohige Anemos" },
                    new() { "Ryunohige Pagos" },
                    new() { "Ryunohige Pagos +1" },
                    new() { "Elemental Lance" },
                    new() { "Elemental Lance +1" },
                    new() { "Elemental Lance +2" },
                    new() { "Pyros Lance" },
                    new() { "Hydatos Lance" },
                    new() { "Hydatos Lance +1" },
                    new() { "Daboya" },
                    new() { "Daboya Eureka" },
                    new() { "Daboya Physeos" },
                }
            },
            {
                this.PGL, new List<List<string>>
                {
                    new() { "Antiquated Sudarshana Chakra" },
                    new() { "Sudarshana Chakra" },
                    new() { "Sudarshana Chakra +1" },
                    new() { "Sudarshana Chakra +2" },
                    new() { "Sudarshana Chakra Anemos" },
                    new() { "Sudarshana Chakra Pagos" },
                    new() { "Sudarshana Chakra Pagos +1" },
                    new() { "Elemental Knuckles" },
                    new() { "Elemental Knuckles +1" },
                    new() { "Elemental Knuckles +2" },
                    new() { "Pyros Knuckles" },
                    new() { "Hydatos Knuckles" },
                    new() { "Hydatos Knuckles +1" },
                    new() { "Dumuzis" },
                    new() { "Dumuzis Eureka" },
                    new() { "Dumuzis Physeos" },
                }
            },
            {
                this.SAM, new List<List<string>>
                {
                    new() { "Antiquated Kiku-ichimonji" },
                    new() { "Kiku-ichimonji" },
                    new() { "Kiku-ichimonji +1" },
                    new() { "Kiku-ichimonji +2" },
                    new() { "Kiku-ichimonji Anemos" },
                    new() { "Kiku-ichimonji Pagos" },
                    new() { "Kiku-ichimonji Pagos +1" },
                    new() { "Elemental Blade" },
                    new() { "Elemental Blade +1" },
                    new() { "Elemental Blade +2" },
                    new() { "Pyros Blade" },
                    new() { "Hydatos Blade" },
                    new() { "Hydatos Blade +1" },
                    new() { "Torigashira" },
                    new() { "Torigashira Eureka" },
                    new() { "Torigashira Physeos" },
                }
            },
            {
                this.ROG, new List<List<string>>
                {
                    new() { "Antiquated Nagi" },
                    new() { "Nagi" },
                    new() { "Nagi +1" },
                    new() { "Nagi +2" },
                    new() { "Nagi Anemos" },
                    new() { "Nagi Pagos" },
                    new() { "Nagi Pagos +1" },
                    new() { "Elemental Knives" },
                    new() { "Elemental Knives +1" },
                    new() { "Elemental Knives +2" },
                    new() { "Pyros Knives" },
                    new() { "Hydatos Knives" },
                    new() { "Hydatos Knives +1" },
                    new() { "Kasasagi" },
                    new() { "Kasasagi Eureka" },
                    new() { "Kasasagi Physeos" },
                }
            },
            {
                this.ARC, new List<List<string>>
                {
                    new() { "Antiquated Failnaught" },
                    new() { "Failnaught" },
                    new() { "Failnaught +1" },
                    new() { "Failnaught +2" },
                    new() { "Failnaught Anemos" },
                    new() { "Failnaught Pagos" },
                    new() { "Failnaught Pagos +1" },
                    new() { "Elemental Harp Bow" },
                    new() { "Elemental Harp Bow +1" },
                    new() { "Elemental Harp Bow +2" },
                    new() { "Pyros Harp Bow" },
                    new() { "Hydatos Harp Bow" },
                    new() { "Hydatos Harp Bow +1" },
                    new() { "Circinae" },
                    new() { "Circinae Eureka" },
                    new() { "Circinae Physeos" },
                }
            },
            {
                this.MCH, new List<List<string>>
                {
                    new() { "Antiquated Outsider" },
                    new() { "Outsider" },
                    new() { "Outsider +1" },
                    new() { "Outsider +2" },
                    new() { "Outsider Anemos" },
                    new() { "Outsider Pagos" },
                    new() { "Outsider Pagos +1" },
                    new() { "Elemental Handgonne" },
                    new() { "Elemental Handgonne +1" },
                    new() { "Elemental Handgonne +2" },
                    new() { "Pyros Handgonne" },
                    new() { "Hydatos Handgonne" },
                    new() { "Hydatos Handgonne +1" },
                    new() { "Mollfrith" },
                    new() { "Mollfrith Eureka" },
                    new() { "Mollfrith Physeos" },
                }
            },
            {
                this.THM, new List<List<string>>
                {
                    new() { "Antiquated Vanargand" },
                    new() { "Vanargand" },
                    new() { "Vanargand +1" },
                    new() { "Vanargand +2" },
                    new() { "Vanargand Anemos" },
                    new() { "Vanargand Pagos" },
                    new() { "Vanargand Pagos +1" },
                    new() { "Elemental Rod" },
                    new() { "Elemental Rod +1" },
                    new() { "Elemental Rod +2" },
                    new() { "Pyros Rod" },
                    new() { "Hydatos Rod" },
                    new() { "Hydatos Rod +1" },
                    new() { "Paikea" },
                    new() { "Paikea Eureka" },
                    new() { "Paikea Physeos" },
                }
            },
            {
                this.SMN, new List<List<string>>
                {
                    new() { "Antiquated Lemegeton" },
                    new() { "Lemegeton" },
                    new() { "Lemegeton +1" },
                    new() { "Lemegeton +2" },
                    new() { "Lemegeton Anemos" },
                    new() { "Lemegeton Pagos" },
                    new() { "Lemegeton Pagos +1" },
                    new() { "Elemental Grimoire" },
                    new() { "Elemental Grimoire +1" },
                    new() { "Elemental Grimoire +2" },
                    new() { "Pyros Grimoire" },
                    new() { "Hydatos Grimoire" },
                    new() { "Hydatos Grimoire +1" },
                    new() { "Tuah" },
                    new() { "Tuah Eureka" },
                    new() { "Tuah Physeos" },
                }
            },
            {
                this.RDM, new List<List<string>>
                {
                    new() { "Antiquated Murgleis" },
                    new() { "Murgleis" },
                    new() { "Murgleis +1" },
                    new() { "Murgleis +2" },
                    new() { "Murgleis Anemos" },
                    new() { "Murgleis Pagos" },
                    new() { "Murgleis Pagos +1" },
                    new() { "Elemental Tuck" },
                    new() { "Elemental Tuck +1" },
                    new() { "Elemental Tuck +2" },
                    new() { "Pyros Tuck" },
                    new() { "Hydatos Tuck" },
                    new() { "Hydatos Tuck +1" },
                    new() { "Brunello" },
                    new() { "Brunello Eureka" },
                    new() { "Brunello Physeos" },
                }
            },
            {
                this.CNJ, new List<List<string>>
                {
                    new() { "Antiquated Aymur" },
                    new() { "Aymur" },
                    new() { "Aymur +1" },
                    new() { "Aymur +2" },
                    new() { "Aymur Anemos" },
                    new() { "Aymur Pagos" },
                    new() { "Aymur Pagos +1" },
                    new() { "Elemental Cane" },
                    new() { "Elemental Cane +1" },
                    new() { "Elemental Cane +2" },
                    new() { "Pyros Cane" },
                    new() { "Hydatos Cane" },
                    new() { "Hydatos Cane +1" },
                    new() { "Rose Couverte" },
                    new() { "Rose Couverte Eureka" },
                    new() { "Rose Couverte Physeos" },
                }
            },
            {
                this.SCH, new List<List<string>>
                {
                    new() { "Antiquated Organum" },
                    new() { "Organum" },
                    new() { "Organum +1" },
                    new() { "Organum +2" },
                    new() { "Organum Anemos" },
                    new() { "Organum Pagos" },
                    new() { "Organum Pagos +1" },
                    new() { "Elemental Codex" },
                    new() { "Elemental Codex +1" },
                    new() { "Elemental Codex +2" },
                    new() { "Pyros Codex" },
                    new() { "Hydatos Codex" },
                    new() { "Hydatos Codex +1" },
                    new() { "Jebat" },
                    new() { "Jebat Eureka" },
                    new() { "Jebat Physeos" },
                }
            },
            {
                this.AST, new List<List<string>>
                {
                    new() { "Antiquated Pleiades" },
                    new() { "Pleiades" },
                    new() { "Pleiades +1" },
                    new() { "Pleiades +2" },
                    new() { "Pleiades Anemos" },
                    new() { "Pleiades Pagos" },
                    new() { "Pleiades Pagos +1" },
                    new() { "Elemental Astrometer" },
                    new() { "Elemental Astrometer +1" },
                    new() { "Elemental Astrometer +2" },
                    new() { "Pyros Astrometer" },
                    new() { "Hydatos Astrometer" },
                    new() { "Hydatos Astrometer +1" },
                    new() { "Albireo" },
                    new() { "Albireo Eureka" },
                    new() { "Albireo Physeos" },
                }
            }
        };

        var stageQuests = new Dictionary<RelicWeaponType, uint>()
        {
            { RelicWeaponType.EurekanAnemos, 68614 },
            { RelicWeaponType.EurekanElemental, 68478 },
            { RelicWeaponType.EurekanPyros, 68148 },
            { RelicWeaponType.EurekanEureka, 68149 },
            { RelicWeaponType.EurekanPhyseos, 68149 },
        };

        foreach (var item in startItemMap)
        {
            var stage = 1u;
            var originalRelicWeaponType = relicWeaponType;
            var previousWeapon = 0u;
            foreach(var items in item.Value)
            {
                uint item1;
                uint item2 = 0;
                if (!this.itemsByName.TryGetValue(items[0].ToParseable(), out item1))
                {
                    throw new Exception($"Unknown item: {items[0]}");
                }

                if (items.Count > 1)
                {
                    if (!this.itemsByName.TryGetValue(items[1].ToParseable(), out item2))
                    {
                        throw new Exception($"Unknown item: {items[1]}");
                    }
                }

                var weaponCategory = (RelicWeaponCategory)relicWeaponCategory;
                var weaponType = (RelicWeaponType)relicWeaponType;
                relicWeapons.Add(new RelicWeapon(rowId, item1, item2, stage, item.Key.RowId, weaponCategory, weaponType, stageQuests.TryGetValue(weaponType, out var quest) ? quest : 0, previousWeapon));
                previousWeapon = rowId;
                relicWeaponType++;
                stage++;
                rowId++;
            }

            relicWeaponType = originalRelicWeaponType;
        }

        //Resistance Weapons
        relicWeaponType = (int)RelicWeaponType.ResistanceResistance;
        relicWeaponCategory = (int)RelicWeaponCategory.Resistance;

        startItemMap = new Dictionary<ClassJob, List<List<string>>>()
        {
            {
                this.GLA, new List<List<string>>
                {
                    new() { "Honorbound", "Tenacity" },
                    new() { "Augmented Honorbound", "Augmented Tenacity" },
                    new() { "Honorbound Recollection", "Tenacity Recollection" },
                    new() { "Law's Order Bastard Sword", "Law's Order Kite Shield" },
                    new() { "Augmented Law's Order Bastard Sword", "Augmented Law's Order Kite Shield" },
                    new() { "Blade's Honor", "Blade's Fortitude" },
                }
            },
            {
                this.MRD, new List<List<string>>
                {
                    new() { "Skullrender" },
                    new() { "Augmented Skullrender" },
                    new() { "Skullrender Recollection" },
                    new() { "Law's Order Labrys" },
                    new() { "Augmented Law's Order Labrys" },
                    new() { "Blade's Valor" },
                }
            },
            {
                this.DRK, new List<List<string>>
                {
                    new() { "Woeborn" },
                    new() { "Augmented Woeborn" },
                    new() { "Woeborn Recollection" },
                    new() { "Law's Order Zweihander" },
                    new() { "Augmented Law's Order Zweihander" },
                    new() { "Blade's Justice" },
                }
            },
            {
                this.GNB, new List<List<string>>
                {
                    new() { "Crownsblade" },
                    new() { "Augmented Crownsblade" },
                    new() { "Crownsblade Recollection" },
                    new() { "Law's Order Manatrigger" },
                    new() { "Augmented Law's Order Manatrigger" },
                    new() { "Blade's Resolve" },
                }
            },
            {
                this.LNC, new List<List<string>>
                {
                    new() { "Dreizack" },
                    new() { "Augmented Dreizack" },
                    new() { "Dreizack Recollection" },
                    new() { "Law's Order Spear" },
                    new() { "Augmented Law's Order Spear" },
                    new() { "Blade's Glory" },
                }
            },
            {
                this.PGL, new List<List<string>>
                {
                    new() { "Samsara" },
                    new() { "Augmented Samsara" },
                    new() { "Samsara Recollection" },
                    new() { "Law's Order Knuckles" },
                    new() { "Augmented Law's Order Knuckles" },
                    new() { "Blade's Serenity" },
                }
            },
            {
                this.SAM, new List<List<string>>
                {
                    new() { "Hoshikiri" },
                    new() { "Augmented Hoshikiri" },
                    new() { "Hoshikiri Recollection" },
                    new() { "Law's Order Samurai Blade" },
                    new() { "Augmented Law's Order Samurai Blade" },
                    new() { "Blade's Fealty" },
                }
            },
            {
                this.ROG, new List<List<string>>
                {
                    new() { "Honeshirazu" },
                    new() { "Augmented Honeshirazu" },
                    new() { "Honeshirazu Recollection" },
                    new() { "Law's Order Knives" },
                    new() { "Augmented Law's Order Knives" },
                    new() { "Blade's Subtlety" },
                }
            },
            {
                this.ARC, new List<List<string>>
                {
                    new() { "Brilliance" },
                    new() { "Augmented Brilliance" },
                    new() { "Brilliance Recollection" },
                    new() { "Law's Order Composite Bow" },
                    new() { "Augmented Law's Order Composite Bow" },
                    new() { "Blade's Muse" },
                }
            },
            {
                this.MCH, new List<List<string>>
                {
                    new() { "Lawman" },
                    new() { "Augmented Lawman" },
                    new() { "Lawman Recollection" },
                    new() { "Law's Order Revolver" },
                    new() { "Augmented Law's Order Revolver" },
                    new() { "Blade's Ingenuity" },
                }
            },
            {
                this.DNC, new List<List<string>>
                {
                    new() { "Enchufla" },
                    new() { "Augmented Enchufla" },
                    new() { "Enchufla Recollection" },
                    new() { "Law's Order Chakrams" },
                    new() { "Augmented Law's Order Chakrams" },
                    new() { "Blade's Euphoria" },
                }
            },
            {
                this.THM, new List<List<string>>
                {
                    new() { "Soulscourge" },
                    new() { "Augmented Soulscourge" },
                    new() { "Soulscourge Recollection" },
                    new() { "Law's Order Rod" },
                    new() { "Augmented Law's Order Rod" },
                    new() { "Blade's Fury" },
                }
            },
            {
                this.SMN, new List<List<string>>
                {
                    new() { "Espiritus" },
                    new() { "Augmented Espiritus" },
                    new() { "Espiritus Recollection" },
                    new() { "Law's Order Index" },
                    new() { "Augmented Law's Order Index" },
                    new() { "Blade's Acumen" },
                }
            },
            {
                this.RDM, new List<List<string>>
                {
                    new() { "Talekeeper" },
                    new() { "Augmented Talekeeper" },
                    new() { "Talekeeper Recollection" },
                    new() { "Law's Order Rapier" },
                    new() { "Augmented Law's Order Rapier" },
                    new() { "Blade's Temperance" },
                }
            },
            {
                this.CNJ, new List<List<string>>
                {
                    new() { "Ingrimm" },
                    new() { "Augmented Ingrimm" },
                    new() { "Ingrimm Recollection" },
                    new() { "Law's Order Cane" },
                    new() { "Augmented Law's Order Cane" },
                    new() { "Blade's Mercy" },
                }
            },
            {
                this.SCH, new List<List<string>>
                {
                    new() { "Akademos" },
                    new() { "Augmented Akademos" },
                    new() { "Akademos Recollection" },
                    new() { "Law's Order Codex" },
                    new() { "Augmented Law's Order Codex" },
                    new() { "Blade's Wisdom" },
                }
            },
            {
                this.AST, new List<List<string>>
                {
                    new() { "Solstice" },
                    new() { "Augmented Solstice" },
                    new() { "Solstice Recollection" },
                    new() { "Law's Order Astrometer" },
                    new() { "Augmented Law's Order Astrometer" },
                    new() { "Blade's Providence" },
                }
            }
        };

        stageQuests = new Dictionary<RelicWeaponType, uint>()
        {
            { RelicWeaponType.ResistanceResistance, 69380 },
            { RelicWeaponType.ResistanceAugmentedResistance, 69506 },
            { RelicWeaponType.ResistanceRecollection, 69507 },
            { RelicWeaponType.ResistanceLawsOrder, 69574 },
            { RelicWeaponType.ResistanceAugmentedLawsOrder, 69576 },
            { RelicWeaponType.ResistanceBlades, 69637 },
        };


        foreach (var item in startItemMap)
        {
            var stage = 1u;
            var originalRelicWeaponType = relicWeaponType;
            var previousWeapon = 0u;
            foreach(var items in item.Value)
            {
                uint item1;
                uint item2 = 0;
                if (!this.itemsByName.TryGetValue(items[0].ToParseable(), out item1))
                {
                    throw new Exception($"Unknown item: {items[0]}");
                }

                if (items.Count > 1)
                {
                    if (!this.itemsByName.TryGetValue(items[1].ToParseable(), out item2))
                    {
                        throw new Exception($"Unknown item: {items[1]}");
                    }
                }

                var weaponCategory = (RelicWeaponCategory)relicWeaponCategory;
                var weaponType = (RelicWeaponType)relicWeaponType;
                relicWeapons.Add(new RelicWeapon(rowId, item1, item2, stage, item.Key.RowId, weaponCategory, weaponType, stageQuests.TryGetValue(weaponType, out var quest) ? quest : 0, previousWeapon));
                previousWeapon = rowId;
                relicWeaponType++;
                stage++;
                rowId++;
            }

            relicWeaponType = originalRelicWeaponType;
        }

        //Manderville Weapons
        relicWeaponType = (int)RelicWeaponType.MandervilleManderville;
        relicWeaponCategory = (int)RelicWeaponCategory.Manderville;

        startItemMap = new Dictionary<ClassJob, List<List<string>>>()
        {
            {
                this.GLA, new List<List<string>>
                {
                    new() { "Manderville Sword", "Manderville Kite Shield" },
                    new() { "Amazing Manderville Sword", "Amazing Manderville Kite Shield" },
                    new() { "Majestic Manderville Sword", "Majestic Manderville Shield" },
                    new() { "Mandervillous Falchion", "Mandervillous Kite Shield" },
                }
            },
            {
                this.MRD, new List<List<string>>
                {
                    new() { "Manderville Axe" },
                    new() { "Amazing Manderville Axe" },
                    new() { "Majestic Manderville Bardiche" },
                    new() { "Mandervillous Battleaxe" },
                }
            },
            {
                this.DRK, new List<List<string>>
                {
                    new() { "Manderville Zweihander" },
                    new() { "Amazing Manderville Zweihander" },
                    new() { "Majestic Manderville Greatsword" },
                    new() { "Mandervillous Greatsword" },
                }
            },
            {
                this.GNB, new List<List<string>>
                {
                    new() { "Manderville Gunblade" },
                    new() { "Amazing Manderville Gunblade" },
                    new() { "Majestic Manderville Bayonet" },
                    new() { "Mandervillous Gunblade" },
                }
            },
            {
                this.LNC, new List<List<string>>
                {
                    new() { "Manderville Spear" },
                    new() { "Amazing Manderville Spear" },
                    new() { "Majestic Manderville Spear" },
                    new() { "Mandervillous Trident" },
                }
            },
            {
                this.RPR, new List<List<string>>
                {
                    new() { "Manderville Scythe" },
                    new() { "Amazing Manderville Scythe" },
                    new() { "Majestic Manderville War Scythe" },
                    new() { "Mandervillous Zaghnal" },
                }
            },
            {
                this.PGL, new List<List<string>>
                {
                    new() { "Manderville Knuckles" },
                    new() { "Amazing Manderville Knuckles" },
                    new() { "Majestic Manderville Fists" },
                    new() { "Mandervillous Fists" },
                }
            },
            {
                this.SAM, new List<List<string>>
                {
                    new() { "Manderville Samurai Blade" },
                    new() { "Amazing Manderville Samurai Blade" },
                    new() { "Majestic Manderville Samurai Blade" },
                    new() { "Mandervillous Samurai Blade" },
                }
            },
            {
                this.ROG, new List<List<string>>
                {
                    new() { "Manderville Knives" },
                    new() { "Amazing Manderville Knives" },
                    new() { "Majestic Manderville Knives" },
                    new() { "Mandervillous Knives" },
                }
            },
            {
                this.ARC, new List<List<string>>
                {
                    new() { "Manderville Harp Bow" },
                    new() { "Amazing Manderville Harp Bow" },
                    new() { "Majestic Manderville Harp Bow" },
                    new() { "Mandervillous Compound Bow" },
                }
            },
            {
                this.MCH, new List<List<string>>
                {
                    new() { "Manderville Revolver" },
                    new() { "Amazing Manderville Revolver" },
                    new() { "Majestic Manderville Pistol" },
                    new() { "Mandervillous Revolver" },
                }
            },
            {
                this.DNC, new List<List<string>>
                {
                    new() { "Manderville Chakrams" },
                    new() { "Amazing Manderville Chakrams" },
                    new() { "Majestic Manderville Chakrams" },
                    new() { "Mandervillous Chakrams" },
                }
            },
            {
                this.THM, new List<List<string>>
                {
                    new() { "Manderville Rod" },
                    new() { "Amazing Manderville Rod" },
                    new() { "Majestic Manderville Staff" },
                    new() { "Mandervillous Rod" },
                }
            },
            {
                this.SMN, new List<List<string>>
                {
                    new() { "Manderville Index" },
                    new() { "Amazing Manderville Index" },
                    new() { "Majestic Manderville Index" },
                    new() { "Mandervillous Index" },
                }
            },
            {
                this.RDM, new List<List<string>>
                {
                    new() { "Manderville Rapier" },
                    new() { "Amazing Manderville Rapier" },
                    new() { "Majestic Manderville Degen" },
                    new() { "Mandervillous Rapier" },
                }
            },
            {
                this.CNJ, new List<List<string>>
                {
                    new() { "Manderville Cane" },
                    new() { "Amazing Manderville Cane" },
                    new() { "Majestic Manderville Wand" },
                    new() { "Mandervillous Cane" },
                }
            },
            {
                this.SCH, new List<List<string>>
                {
                    new() { "Manderville Codex" },
                    new() { "Amazing Manderville Codex" },
                    new() { "Majestic Manderville Codex" },
                    new() { "Mandervillous Codex" },
                }
            },
            {
                this.AST, new List<List<string>>
                {
                    new() { "Manderville Torquetum" },
                    new() { "Amazing Manderville Torquetum" },
                    new() { "Majestic Manderville Orrery" },
                    new() { "Mandervillous Torquetum" },
                }
            },
            {
                this.SGE, new List<List<string>>
                {
                    new() { "Manderville Milpreves" },
                    new() { "Amazing Manderville Milpreves" },
                    new() { "Majestic Manderville Wings" },
                    new() { "Mandervillous Wings" },
                }
            }
        };

        stageQuests = new Dictionary<RelicWeaponType, uint>()
        {
            { RelicWeaponType.MandervilleManderville, 70189 },
            { RelicWeaponType.MandervilleAmazing, 70262 },
            { RelicWeaponType.MandervilleMajestic, 70308 },
            { RelicWeaponType.MandervilleMandervillous, 70343 },
        };


        foreach (var item in startItemMap)
        {
            var stage = 1u;
            var originalRelicWeaponType = relicWeaponType;
            var previousWeapon = 0u;
            foreach(var items in item.Value)
            {
                uint item1;
                uint item2 = 0;
                if (!this.itemsByName.TryGetValue(items[0].ToParseable(), out item1))
                {
                    throw new Exception($"Unknown item: {items[0]}");
                }

                if (items.Count > 1)
                {
                    if (!this.itemsByName.TryGetValue(items[1].ToParseable(), out item2))
                    {
                        throw new Exception($"Unknown item: {items[1]}");
                    }
                }

                var weaponCategory = (RelicWeaponCategory)relicWeaponCategory;
                var weaponType = (RelicWeaponType)relicWeaponType;
                relicWeapons.Add(new RelicWeapon(rowId, item1, item2, stage, item.Key.RowId, weaponCategory, weaponType, stageQuests.TryGetValue(weaponType, out var quest) ? quest : 0, previousWeapon));
                previousWeapon = rowId;
                relicWeaponType++;
                stage++;
                rowId++;
            }

            relicWeaponType = originalRelicWeaponType;
        }

        //Phantom Weapons
        relicWeaponType = (int)RelicWeaponType.PhantomPenumbrae;
        relicWeaponCategory = (int)RelicWeaponCategory.Phantom;

        startItemMap = new Dictionary<ClassJob, List<List<string>>>()
        {
            {
                this.GLA, new List<List<string>>
                {
                    new() { "Phantom Sword Penumbrae", "Phantom Kite Shield Penumbrae" },
                    new() { "Phantom Sword Umbrae", "Phantom Kite Shield Umbrae" },
                    new() { "Phantom Sword Obscurum", "Phantom Shield Obscurum" },
                }
            },
            {
                this.MRD, new List<List<string>>
                {
                    new() { "Phantom Bardiche Penumbrae" },
                    new() { "Phantom Bardiche Umbrae" },
                    new() { "Phantom Bardiche Obscurum" },
                }
            },
            {
                this.DRK, new List<List<string>>
                {
                    new() { "Phantom Guillotine Penumbrae" },
                    new() { "Phantom Guillotine Umbrae" },
                    new() { "Phantom Guillotine Obscurum" },
                }
            },
            {
                this.GNB, new List<List<string>>
                {
                    new() { "Phantom Bayonet Penumbrae" },
                    new() { "Phantom Bayonet Umbrae" },
                    new() { "Phantom Gunblade Obscurum" },
                }
            },
            {
                this.LNC, new List<List<string>>
                {
                    new() { "Phantom Spear Penumbrae" },
                    new() { "Phantom Spear Umbrae" },
                    new() { "Phantom Fork Obscurum" },
                }
            },
            {
                this.RPR, new List<List<string>>
                {
                    new() { "Phantom War Scythe Penumbrae" },
                    new() { "Phantom War Scythe Umbrae" },
                    new() { "Phantom War Scythe Obscurum" },
                }
            },
            {
                this.PGL, new List<List<string>>
                {
                    new() { "Phantom Knuckles Penumbrae" },
                    new() { "Phantom Knuckles Umbrae" },
                    new() { "Phantom Jamadhars Obscurum" },
                }
            },
            {
                this.SAM, new List<List<string>>
                {
                    new() { "Phantom Blade Penumbrae" },
                    new() { "Phantom Blade Umbrae" },
                    new() { "Phantom Blade Obscurum" },
                }
            },
            {
                this.ROG, new List<List<string>>
                {
                    new() { "Phantom Cleavers Penumbrae" },
                    new() { "Phantom Cleavers Umbrae" },
                    new() { "Phantom Khukuri Obscurum" },
                }
            },
            {
                this.VPR, new List<List<string>>
                {
                    new() { "Phantom Twinfangs Penumbrae" },
                    new() { "Phantom Twinfangs Umbrae" },
                    new() { "Phantom Twinfangs Obscurum" },
                }
            },
            {
                this.ARC, new List<List<string>>
                {
                    new() { "Phantom Harp Bow Penumbrae" },
                    new() { "Phantom Harp Bow Umbrae" },
                    new() { "Phantom Longbow Obscurum" },
                }
            },
            {
                this.MCH, new List<List<string>>
                {
                    new() { "Phantom Arquebus Penumbrae" },
                    new() { "Phantom Arquebus Umbrae" },
                    new() { "Phantom Musketoon Obscurum" },
                }
            },
            {
                this.DNC, new List<List<string>>
                {
                    new() { "Phantom Terpna Penumbrae" },
                    new() { "Phantom Terpna Umbrae" },
                    new() { "Phantom Tathlums Obscurum" },
                }
            },
            {
                this.THM, new List<List<string>>
                {
                    new() { "Phantom Staff Penumbrae" },
                    new() { "Phantom Staff Umbrae" },
                    new() { "Phantom Longpole Obscurum" },
                }
            },
            {
                this.SMN, new List<List<string>>
                {
                    new() { "Phantom Index Penumbrae" },
                    new() { "Phantom Index Umbrae" },
                    new() { "Phantom Index Obscurum" },
                }
            },
            {
                this.RDM, new List<List<string>>
                {
                    new() { "Phantom Rapier Penumbrae" },
                    new() { "Phantom Rapier Umbrae" },
                    new() { "Phantom Hanger Obscurum" },
                }
            },
            {
                this.PCT, new List<List<string>>
                {
                    new() { "Phantom Flat Brush Penumbrae" },
                    new() { "Phantom Flat Brush Umbrae" },
                    new() { "Phantom Round Brush Obscurum" },
                }
            },
            {
                this.CNJ, new List<List<string>>
                {
                    new() { "Phantom Crook Penumbrae" },
                    new() { "Phantom Crook Umbrae" },
                    new() { "Phantom Cane Obscurum" },
                }
            },
            {
                this.SCH, new List<List<string>>
                {
                    new() { "Phantom Codex Penumbrae" },
                    new() { "Phantom Codex Umbrae" },
                    new() { "Phantom Codex Obscurum" },
                }
            },
            {
                this.AST, new List<List<string>>
                {
                    new() { "Phantom Star Globe Penumbrae" },
                    new() { "Phantom Star Globe Umbrae" },
                    new() { "Phantom Star Pendant Obscurum" },
                }
            },
            {
                this.SGE, new List<List<string>>
                {
                    new() { "Phantom Pendulums Penumbrae" },
                    new() { "Phantom Pendulums Umbrae" },
                    new() { "Phantom Wings Obscurum" },
                }
            }
        };

        stageQuests = new Dictionary<RelicWeaponType, uint>()
        {
            { RelicWeaponType.PhantomPenumbrae, 70847 },
            { RelicWeaponType.PhantomUmbrae, 70916 },
            { RelicWeaponType.PhantomObscurum, 70990 },
        };


        foreach (var item in startItemMap)
        {
            var stage = 1u;
            var originalRelicWeaponType = relicWeaponType;
            var previousWeapon = 0u;
            foreach(var items in item.Value)
            {
                uint item1;
                uint item2 = 0;
                if (!this.itemsByName.TryGetValue(items[0].ToParseable(), out item1))
                {
                    throw new Exception($"Unknown item: {items[0]}");
                }

                if (items.Count > 1)
                {
                    if (!this.itemsByName.TryGetValue(items[1].ToParseable(), out item2))
                    {
                        throw new Exception($"Unknown item: {items[1]}");
                    }
                }

                var weaponCategory = (RelicWeaponCategory)relicWeaponCategory;
                var weaponType = (RelicWeaponType)relicWeaponType;
                relicWeapons.Add(new RelicWeapon(rowId, item1, item2, stage, item.Key.RowId, weaponCategory, weaponType, stageQuests.TryGetValue(weaponType, out var quest) ? quest : 0, previousWeapon));
                previousWeapon = rowId;
                relicWeaponType++;
                stage++;
                rowId++;
            }

            relicWeaponType = originalRelicWeaponType;
        }

        return relicWeapons;
    }

    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<RelicWeapon> items = new List<RelicWeapon>();
        items.AddRange(this.ProcessWeapons());

        return [..items.Select(c => c)];
    }
}
