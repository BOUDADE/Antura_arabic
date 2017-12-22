using System;
using Antura.Core;
using Antura.Database;
using Antura.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Antura.Rewards
{
    public enum RewardUnlockMethod
    {
        Any,
        NewBase,
        NewColor,
        NewBaseAndAllColors
    }

    public class RewardSystemManager
    {
        private const bool VERBOSE = true;

        private const string ANTURA_REWARDS_PARTS_CONFIG_PATH = "Configs/AnturaRewardsPartsConfig";
        private const string ANTURA_REWARDS_UNLOCKS_CONFIG_PATH = "Configs/AnturaRewardsUnlocksConfig";

        private const bool USE_UNLOCK_CONFIG = false;

        /// <summary>
        /// The maximum rewards unlockable for playsession.
        /// </summary>
        // TODO: use this? public const int MaxRewardsUnlockableForPlaysession = 2;

        #region Events

        public delegate void RewardSystemEventHandler(RewardPack rewardPack);
        public static event RewardSystemEventHandler OnRewardChanged;
        public static event RewardSystemEventHandler OnNewRewardUnlocked;

        #endregion

        #region Rewards Configuration

        public void Init()
        {
            LoadConfigs();
        }

        /// <summary>
        /// The configuration of items that can be unlocked
        /// </summary>
        RewardPartsConfig partsConfig;

        /// <summary>
        /// The configuration of items that can be unlocked
        /// </summary>
        RewardsUnlocksConfig unlocksConfig;

        /// <summary>
        /// Loads the reward system configurations
        /// </summary>
        private void LoadConfigs()
        {
            LoadPartsConfig();
            LoadUnlocksConfig();
        }

        private void LoadPartsConfig()
        {
            TextAsset partsConfigData = Resources.Load(ANTURA_REWARDS_PARTS_CONFIG_PATH) as TextAsset;
            partsConfig = JsonUtility.FromJson<RewardPartsConfig>(partsConfigData.text);
            BuildAllPacks(partsConfig);
        }

        void BuildAllPacks(RewardPartsConfig partsConfig)
        {
            rewardPacksDict.Clear();
            rewardPacksDict[RewardBaseType.Prop] = BuildPacks(partsConfig, RewardBaseType.Prop);
            rewardPacksDict[RewardBaseType.Texture] = BuildPacks(partsConfig, RewardBaseType.Texture);
            rewardPacksDict[RewardBaseType.Decal] = BuildPacks(partsConfig, RewardBaseType.Decal);

            if (VERBOSE)
                Debug.Log("Total packs built: " 
                    + "\n " + RewardBaseType.Prop + ": " +  + rewardPacksDict[RewardBaseType.Prop].Count
                    + "\n " + RewardBaseType.Texture + ": " + +rewardPacksDict[RewardBaseType.Texture].Count
                    + "\n " + RewardBaseType.Decal + ": " + +rewardPacksDict[RewardBaseType.Decal].Count
                    );
        }

        private List<RewardPack> BuildPacks(RewardPartsConfig partsConfig, RewardBaseType baseType)
        {
            var bases = partsConfig.GetBasesForType(baseType);
            var colors = partsConfig.GetColorsForType(baseType);

            if (VERBOSE)
                Debug.Log("Building packs for " + baseType
                + "\n Bases: " + bases.Count() + " Colors: " + colors.Count());

            List<RewardPack> rewardPacks = new List<RewardPack>();
            foreach (var b in bases)
            {
                foreach (var c in colors)
                {
                    RewardPack pack = new RewardPack(baseType, b, c);
                    rewardPacks.Add(pack);
                }
            }
            return rewardPacks;
        }

        private void LoadUnlocksConfig()
        {
            TextAsset unlocksConfigData = Resources.Load(ANTURA_REWARDS_UNLOCKS_CONFIG_PATH) as TextAsset;
            unlocksConfig = JsonUtility.FromJson<RewardsUnlocksConfig>(unlocksConfigData.text);

            if (VERBOSE)
            {
                string s = "Unlock data:";
                foreach (var jpUnlock in unlocksConfig.JourneyPositionsUnlocks)
                {
                    s += "\n" + jpUnlock.JourneyPositionID + ": " + jpUnlock.NewPropBase + "," + jpUnlock.NewPropColor +
                         "," + jpUnlock.NewDecal + "," + jpUnlock.NewTexture;
                }
                Debug.Log(s);
            }
        }

        public IEnumerable<RewardBase> GetRewardBasesOfType(RewardBaseType baseType)
        {
            return partsConfig.GetBasesForType(baseType);
        }

        #region Reward Packs

        private Dictionary<RewardBaseType, List<RewardPack>> rewardPacksDict = new Dictionary<RewardBaseType, List<RewardPack>>();

        public RewardPack GetRewardPackByUniqueId(string uniqueId)
        {
            return GetAllRewardPacks().FirstOrDefault(p => p.UniqueId == uniqueId);
        }

        public RewardPack GetRewardPackByPartsIds(string baseId, string colorId)
        {
            return GetAllRewardPacks().FirstOrDefault(p => p.BaseId == baseId && p.ColorId == colorId);
        }

        public List<RewardPack> GetAllRewardPacksOfBase(RewardBaseType baseType)
        {
            if (!rewardPacksDict.ContainsKey(baseType)) throw new ArgumentNullException("Dict not initialised correctly!");
            return rewardPacksDict[baseType];
        }

        public List<RewardPack> GetUnlockedRewardPacksOfBase(RewardBaseType baseType)
        {
            if (!rewardPacksDict.ContainsKey(baseType)) throw new ArgumentNullException("Dict not initialised correctly!");
            return rewardPacksDict[baseType].Where(x => x.IsUnlocked).ToList();
        }

        public IEnumerable<RewardPack> GetAllRewardPacks()
        {
            foreach (var rewardPack in rewardPacksDict[RewardBaseType.Prop])
                yield return rewardPack;
            foreach (var rewardPack in rewardPacksDict[RewardBaseType.Decal])
                yield return rewardPack;
            foreach (var rewardPack in rewardPacksDict[RewardBaseType.Texture])
                yield return rewardPack;
        }

        public List<RewardPack> GetUnlockedRewardPacks()
        {
            var unlockedPacks = GetAllRewardPacks().Where(p => p.IsUnlocked);
            return unlockedPacks.ToList();
        }

        public List<RewardPack> GetLockedRewardPacks()
        {
            var lockedPacks = GetAllRewardPacks().Where(p => p.IsLocked);
            return lockedPacks.ToList();
        }

        public List<RewardPack> GetLockedRewardPacksOfBase(RewardBaseType baseType)
        {
            var packsOfBase = rewardPacksDict[baseType];
            var lockedPacks = packsOfBase.Where(p => p.IsLocked);
            return lockedPacks.ToList();
        }

        List<RewardBase> GetLockedRewardBases(RewardBaseType baseType)
        {
            var unlockedBases = GetUnlockedRewardBases(baseType);
            var allBases = GetRewardBasesOfType(baseType);
            List<RewardBase> lockedBases = new List<RewardBase>();

            foreach (var rewardBase in allBases)
            {
                if (!unlockedBases.Contains(rewardBase))
                    lockedBases.Add(rewardBase);
            }
            return lockedBases;
        }

        List<RewardBase> GetUnlockedRewardBases(RewardBaseType baseType)
        {
            var packsOfBase = rewardPacksDict[baseType];
            var unlockedPacksOfBase = packsOfBase.Where(p => p.IsUnlocked);
            var allBases = GetRewardBasesOfType(baseType);

            HashSet<RewardBase> unlockedBases = new HashSet<RewardBase>();
            foreach (var rewardPack in unlockedPacksOfBase)
            {
                var rewardBase = allBases.First(x => x.ID == rewardPack.BaseId);
                if (rewardBase != null)
                {
                    unlockedBases.Add(rewardBase);
                }
            }
            return unlockedBases.ToList();
        }


        #endregion

        /// <summary>
        /// Gets a PROP by its string identifier.
        /// </summary>
        /// <param name="_baseId">The reward identifier.</param>
        /// <returns></returns>
        // TODO: refactor so that RewardPackData has all it needs
        // TODO: DEPRECATE THIS
        public RewardPack GetPropRewardById(string _baseId)
        {
            return GetAllRewardPacksOfBase(RewardBaseType.Prop).FirstOrDefault(x => x.BaseId == _baseId); 
        }

        #endregion

        #region Rewards Unlocking

        #region Save / Load

        /// <summary>
        /// Called by the Player Profile to load the state of unlocked rewards.
        /// </summary>
        // TODO: let this call the PlayerProfile, and not vice-versa
        public void InjectRewardsUnlockData(List<RewardPackUnlockData> unlockDataList)
        {
            Debug.Log("Loading unlock datas: " + unlockDataList.Count);
            foreach (var unlockData in unlockDataList)
            {
                var id = unlockData.Id;
                var pack = GetRewardPackByUniqueId(id);
                if (pack == null)
                    Debug.LogError("Cannot find pack with id " + id + ": RewardPackUnlockData out of sync?");
                else
                    pack.SetUnlockData(unlockData);
            }
        }

        /// <summary>
        /// Called to save the state of unlocked rewards.
        /// </summary>
        public void SaveRewardsUnlockDataChanges()
        {
            AppManager.I.Player.SaveRewardPackUnlockDataList();
        }

        /// <summary>
        /// Called to reset the state of unlocked rewards.
        /// </summary>
        public void ResetRewardsUnlockData()
        {
            AppManager.I.Player.ResetRewardPackUnlockData();

            foreach (var pack in GetAllRewardPacks())
            {
                pack.SetUnlockData(null);
            }
        }

        #endregion

        #region Checks

        public bool IsThereSomeNewReward()
        {
            return GetUnlockedRewardPacks().Any(r => r.IsNew);
        }

        public bool IsRewardColorNew(string baseId, string colorId)
        {
            return GetUnlockedRewardPacks().Any(r => r.BaseId == baseId && r.ColorId == colorId && r.IsNew);
        }

        public bool IsRewardBaseNew(string baseId)
        {
            return GetUnlockedRewardPacks().Any(r => r.BaseId == baseId && r.IsNew);
        }

        public bool DoesRewardCategoryContainNewElements(RewardBaseType baseType, string _rewardCategory = "")
        {
            return GetUnlockedRewardPacks().Any(r => r.BaseType == baseType && r.Category == _rewardCategory && r.IsNew);
        }

        /// <summary>
        /// Return true if all rewards for this JourneyPosition are already unlocked.
        /// </summary>
        /// <param name="_journeyPosition">The journey position.</param>
        public bool AreAllJourneyPositionRewardsAlreadyUnlocked(JourneyPosition journeyPosition)
        {
            int nAlreadyUnlocked = GetRewardPacksAlreadyUnlockedForJourneyPosition(journeyPosition).Count();
            int nForJourneyPosition = GetRewardPacksForJourneyPosition(journeyPosition).Count();
            //if (nForJourneyPosition == 0) throw new Exception("No rewards were added for JP " + journeyPosition);
            return nForJourneyPosition > 0 && nAlreadyUnlocked == nForJourneyPosition;
        }

        #endregion

        #region Getters

        /// <summary>
        /// Gets the total count of all reward packs. 
        /// Any base with any color variation available in game.
        /// </summary>
        public int GetTotalRewardPacksCount()
        {
            return GetAllRewardPacks().Count();
        }

        /// <summary>
        /// Gets the unlocked reward count for the current player. 0 if current player is null.
        /// </summary>
        /// <returns></returns>
        public int GetUnlockedRewardsCount()
        {
            return AppManager.I.Player != null ? GetUnlockedRewardPacks().Count : 0;
        }

        public IEnumerable<RewardPack> GetRewardPacksAlreadyUnlockedForJourneyPosition(JourneyPosition journeyPosition)
        {
            return GetAllRewardPacks().Where(x => x.IsFoundAtJourneyPosition(journeyPosition) && x.IsUnlocked);
        }

        public IEnumerable<RewardPack> GetRewardPacksForJourneyPosition(JourneyPosition journeyPosition)
        {
            return GetAllRewardPacks().Where(x => x.IsFoundAtJourneyPosition(journeyPosition));
        }

        #endregion

        #region Pack Creation and Unlocking

        private void RegisterLockedPacks(List<RewardPack> packs, JourneyPosition jp, bool save = true)
        {
            // Packs are at first added and registered as Locked
            foreach (var pack in packs)
            {
                RegisterLockedPack(pack, jp);
            }
            if (save) SaveRewardsUnlockDataChanges();
        }

        private void RegisterLockedPack(RewardPack pack, JourneyPosition jp)
        {
            if (pack.HasUnlockData())
            {
                throw new Exception("Pack " + pack + " is already registered! Cannot register again");
            }

            // Add the unlock data and register it
            var unlockData = new RewardPackUnlockData(LogManager.I.AppSession, pack.UniqueId, jp);
            unlockData.IsLocked = true;
            unlockData.IsNew = true;
            AppManager.I.Player.RegisterUnlockData(unlockData);
            pack.SetUnlockData(unlockData);

            if (VERBOSE) Debug.Log("Registered locked pack " + pack);
        }

        public void UnlockPacksSelection(List<RewardPack> packs, int nToUnlock, bool save = true)
        {
            if (packs.Count == 0) Debug.LogError("No packs to unlock!");
            var packsSelection = packs.RandomSelect(nToUnlock, true);
            UnlockPacks(packsSelection, save);
        }

        private void UnlockPacks(List<RewardPack> packs, bool save = true)
        {
            foreach (var pack in packs)
            {
                UnlockPack(pack);
            }
            if (save) SaveRewardsUnlockDataChanges();
        }

        private void UnlockPack(RewardPack pack)
        {
            pack.SetUnlocked();
            pack.SetNew(true);
            if (VERBOSE) Debug.Log("Unlocked pack " + pack);
        }

        /// <summary>
        /// Unlocks ALL reward packs that have not been unlocked yet
        /// </summary>
        /// <param name="save"></param>
        public void UnlockAllMissingExtraPacks(bool save = true)
        {
            JourneyPosition extraRewardJP = new JourneyPosition(100, 100, 100);

            var packs = new List<RewardPack>();
            packs.AddRange(GetLockedRewardPacksOfBase(RewardBaseType.Prop));
            packs.AddRange(GetLockedRewardPacksOfBase(RewardBaseType.Decal));
            packs.AddRange(GetLockedRewardPacksOfBase(RewardBaseType.Texture));

            RegisterLockedPacks(packs, extraRewardJP, false);
            UnlockPacks(packs, save);
        }

        /// <summary>
        /// Unlocks all rewards in the game.
        /// </summary>
        public void UnlockAllPacks()
        {
            var allPlaySessionInfos = AppManager.I.ScoreHelper.GetAllPlaySessionInfo();
            for (int i = 0; i < allPlaySessionInfos.Count; i++)
            {
                var jp = AppManager.I.JourneyHelper.PlaySessionIdToJourneyPosition(allPlaySessionInfos[i].data.Id);
                var packs = UnlockAllRewardPacksForJourneyPosition(jp, false);
                if (packs != null)
                    Debug.LogFormat("Unlocked rewards for playsession {0} : {1}", jp, packs.Count);
            }

            Debug.LogFormat("Unlocking also all extra rewards!");
            UnlockAllMissingExtraPacks(false);
            SaveRewardsUnlockDataChanges();
        }

        public List<RewardPack> UnlockAllRewardPacksForJourneyPosition(JourneyPosition journeyPosition, bool save = true)
        {
            var packs = GetOrGenerateAllRewardPacksForJourneyPosition(journeyPosition);

            if (AreAllJourneyPositionRewardsAlreadyUnlocked(journeyPosition))
            {
                Debug.LogError("We already unlocked all rewards for JP " + journeyPosition);
                return null;
            }

            UnlockPacks(packs, save);
            return packs;
        }

        #endregion

        #region Pack Generation

        /// <summary>
        /// Generates all Reward Packs for a given journey position
        /// </summary>
        /// <returns></returns>
        public List<RewardPack> GetOrGenerateAllRewardPacksForJourneyPosition(JourneyPosition journeyPosition)
        {
            // First check whether we already generated them
            var rewardPacks = GetRewardPacksForJourneyPosition(journeyPosition);
            if (rewardPacks.Any())
            {
                return rewardPacks.ToList();
            }

            // If not, we need to generate them from scratch
            var jpPacks = new List<RewardPack>();

            if (USE_UNLOCK_CONFIG)
            {
                GeneratePacksFromUnlockConfig(journeyPosition, jpPacks);
            }
            else
            {
                GeneratePacksFromUnlockFunction(journeyPosition, jpPacks);
            }

            // We register the generated packs as locked
            RegisterLockedPacks(jpPacks, journeyPosition);

            return jpPacks;
        }

        private void GeneratePacksFromUnlockFunction(JourneyPosition journeyPosition, List<RewardPack> jpPacks)
        {
            // Non-assessment PS do not generate any reward
            if (!journeyPosition.IsAssessment())
                return;

            // Force to unlock a prop and all its colors at the first JP
            if (journeyPosition.Equals(new JourneyPosition(1, 1, 100)))
            {
                jpPacks.AddRange(GenerateNewRewardPacks(RewardBaseType.Prop, RewardUnlockMethod.NewBaseAndAllColors));
                return;
            }

            // Else, randomly choose between locked props or textures
            // TODO!!
            jpPacks.AddRange(GenerateNewRewardPacks(RewardBaseType.Texture, RewardUnlockMethod.Any));
        }

        private void GeneratePacksFromUnlockConfig(JourneyPosition journeyPosition, List<RewardPack> jpPacks)
        {
            // What rewards are unlocked at this JP?
            JourneyPositionRewardUnlock unlocksAtJP = unlocksConfig.JourneyPositionsUnlocks.Find(r => r.JourneyPositionID == journeyPosition.Id);
            if (unlocksAtJP == null)
            {
                Debug.LogErrorFormat("Unable to find reward unlocks for JourneyPositions {0}", journeyPosition);
                return;
            }

            // Check numbers and base types
            for (int i = 0; i < unlocksAtJP.NewPropBase; i++)
                jpPacks.AddRange(GenerateNewRewardPacks(RewardBaseType.Prop, RewardUnlockMethod.NewBase));
            //TODO: if (OnNewRewardUnlocked != null)  OnNewRewardUnlocked(newItemReward);

            for (int i = 0; i < unlocksAtJP.NewPropColor; i++)
                jpPacks.AddRange(GenerateNewRewardPacks(RewardBaseType.Prop, RewardUnlockMethod.NewColor));

            if (unlocksAtJP.NewTexture > 0)
                jpPacks.AddRange(GenerateNewRewardPacks(RewardBaseType.Texture, RewardUnlockMethod.Any));

            if (unlocksAtJP.NewDecal > 0)
                jpPacks.AddRange(GenerateNewRewardPacks(RewardBaseType.Decal, RewardUnlockMethod.Any));
        }

        #endregion

        /// <summary>
        /// Generate a new list of reward packs to be unlocked.
        /// </summary>
        /// <param name="baseType">Type of the reward.</param>
        private List<RewardPack> GenerateNewRewardPacks(RewardBaseType baseType, RewardUnlockMethod unlockMethod)
        {
            // TODO: also force category for RewardBase
            List<RewardPack> newRewardPacks = new List<RewardPack>();
            switch (unlockMethod)
            {
                case RewardUnlockMethod.NewBase:
                {
                    // We force a NEW base
                    var lockedBases = GetLockedRewardBases(baseType);
                    if (lockedBases.Count == 0)
                        throw new NullReferenceException(
                            "We do not have enough rewards to get a new base of type " + baseType);

                    var newBase = lockedBases.RandomSelectOne();
                    var lockedPacks = GetLockedRewardPacksOfBase(baseType);
                    var lockedPacksOfNewBase = lockedPacks.Where(x => x.BaseId == newBase.ID).ToList();
                    newRewardPacks.Add(lockedPacksOfNewBase.RandomSelectOne()); 
                }
                    break;
                case RewardUnlockMethod.NewBaseAndAllColors:
                    {
                        // We force a NEW base
                        var lockedBases = GetLockedRewardBases(baseType);
                        if (lockedBases.Count == 0)
                            throw new NullReferenceException(
                                "We do not have enough rewards to get a new base of type " + baseType);

                        var newBase = lockedBases.RandomSelectOne();
                        var lockedPacks = GetLockedRewardPacksOfBase(baseType);
                        var lockedPacksOfNewBase = lockedPacks.Where(x => x.BaseId == newBase.ID).ToList();

                        // We add all locked packs of that base
                        newRewardPacks.AddRange(lockedPacksOfNewBase);
                    }
                    break;
                case RewardUnlockMethod.NewColor:
                {
                    // We force an OLD base
                    var unlockedBases = GetUnlockedRewardBases(baseType);
                    var unlockedBasesWithColorsLeft = unlockedBases.Where(b => GetLockedRewardPacksOfBase(baseType).Count(p => p.BaseId == b.ID) > 0).ToList();

                    if (unlockedBasesWithColorsLeft.Count == 0)
                        throw new NullReferenceException(
                            "We do not have unlocked bases that still have colors to be unlocked for base type " + baseType);

                    var oldBase = unlockedBasesWithColorsLeft.RandomSelectOne();
                    var lockedPacks = GetLockedRewardPacksOfBase(baseType);
                    var lockedPacksOfOldBase = lockedPacks.Where(x => x.BaseId == oldBase.ID).ToList();
                    if (lockedPacksOfOldBase.Count == 0)
                        throw new NullReferenceException(
                            "We do not have enough rewards to get a new color for an old base of type " + baseType);

                    newRewardPacks.Add(lockedPacksOfOldBase.RandomSelectOne()); 
                }
                    break;
                case RewardUnlockMethod.Any:
                {
                    // We get any reward pack
                    var lockedPacks = GetLockedRewardPacksOfBase(baseType);

                    if (lockedPacks.Count == 0)
                        throw new NullReferenceException(
                            "We do not have enough rewards left of type " + baseType);

                    newRewardPacks.Add(lockedPacks.RandomSelectOne());
                }
                    break;
            }
            return newRewardPacks;
        }

        /// <summary>
        /// Unlocks the first set of rewards for current player.
        /// </summary>
        public void UnlockFirstSetOfRewards()
        {
            var _player = AppManager.I.Player;
            if (_player == null)
            {
                Debug.LogError("No current player available!");
                return;
            }

            var zeroJP = new JourneyPosition(0, 0, 0);

            if (AreAllJourneyPositionRewardsAlreadyUnlocked(zeroJP))
            {
                Debug.LogError("We already unlocked the first set of rewards!");
                return;
            }

          
            var propPacks = GenerateFirstRewards(RewardBaseType.Prop);   // 1 prop and colors
            var texturePacks = GenerateFirstRewards(RewardBaseType.Texture);  // 1 texture
            var decalPacks = GenerateFirstRewards(RewardBaseType.Decal);   // 1 decal

            List<RewardPack> packs = new List<RewardPack>();
            packs.AddRange(propPacks);
            packs.AddRange(texturePacks);
            packs.AddRange(decalPacks);

            RegisterLockedPacks(packs, zeroJP, false);
            UnlockPacks(packs);

            // Force as already seen
            foreach (var pack in packs)
            {
                pack.SetNew(false);
            }

            // force to to wear decal
            _player.CurrentAnturaCustomizations.DecalPack = decalPacks[0];
            _player.CurrentAnturaCustomizations.DecalPackId = decalPacks[0].UniqueId;

            // force to to wear texture
            _player.CurrentAnturaCustomizations.TexturePack = texturePacks[0];
            _player.CurrentAnturaCustomizations.TexturePackId = texturePacks[0].UniqueId;

            // Save initial packs and customization
            _player.SaveRewardPackUnlockDataList();
            SaveRewardsUnlockDataChanges();
        }

        /// <summary>
        /// Gets the first RewardPacks that are unlocked when the game starts.
        /// </summary>
        /// <param name="baseType">Base type of the rewards to generate.</param>
        private List<RewardPack> GenerateFirstRewards(RewardBaseType baseType)
        {
            List<RewardPack> list = new List<RewardPack>();
            switch (baseType)
            {
                case RewardBaseType.Prop:
                    list = GenerateNewRewardPacks(baseType, RewardUnlockMethod.NewBaseAndAllColors);
                    break;
                case RewardBaseType.Texture:
                    list.Add(GetRewardPackByPartsIds("Antura_wool_tilemat", "color1"));
                    break;
                case RewardBaseType.Decal:
                    list.Add(GetRewardPackByPartsIds("Antura_decalmap01", "color1"));
                    break;
            }
            return list;
        }

        #endregion

        #region Customization (i.e. UI, selection, view)

        // used during customization
        // TODO: instead of creating a new reward pack, get just the correct ID and move that around
        private RewardPack CurrentSelectedReward = null;

        // TODO:
        /// <summary>
        /// Gets the reward items by rewardType (always 9 items, if not presente item in the return list is null).
        /// </summary>
        /// <param name="baseType">Type of the reward.</param>
        /// <param name="_parentsTransForModels">The parents trans for models.</param>
        /// <param name="_category">The category reward identifier.</param>
        /// <returns></returns>
        public List<RewardBaseItem> GetRewardItemsByRewardType(RewardBaseType baseType, List<Transform> _parentsTransForModels,
            string _category = "")
        {
            List<RewardBaseItem> returnList = new List<RewardBaseItem>();

            /// TODO: logic
            /// - Load returnList by type and category checking unlocked and if exist active one
            /*
            switch (baseType) {
                case RewardBaseType.Prop:
                    // Filter from unlocked elements (only items with this category and only one for itemID)
                    List<RewardProp> rewards = ItemsConfig.GetClone().PropBases;
                    foreach (var item in rewards.FindAll(r => r.Category == _category))
                    {
                        //var rewardPack = GetRewardPackByUniqueId(r.ItemId);
                        var unlockedPacks = GetUnlockedRewardPacks(baseType);

                        if (unlockedPacks.FindAll(pack => pack.Category == _category)
                            .Exists(pack => pack.UniqueId == item.ID)) {
                            returnList.Add(new RewardItem() {
                                ID = item.ID,
                                IsNew = RewardItemIsNew(item.ID),
                                IsSelected = AppManager.I.Player.CurrentAnturaCustomizations.PropPacks.Exists(f => f.BaseId == item.ID)
                            });
                        } else {
                            returnList.Add(null);
                        }
                    }
                    /// - Charge models
                    for (int i = 0; i < returnList.Count; i++) {
                        if (returnList[i] != null) {
                            ModelsManager.MountModel(returnList[i].ID, _parentsTransForModels[i]);
                        }
                    }
                    break;
                case RewardBaseType.Texture:
                    // Filter from unlocked elements (only one for itemID)
                    foreach (var item in ItemsConfig.TextureBases)
                    {
                        var unlockedPacks = GetUnlockedRewardPacks(baseType);
                        if (unlockedPacks.Any(ur => ur.BaseId == item.ID)) {
                            returnList.Add(new RewardItem() {
                                ID = item.ID,
                                IsNew = RewardItemIsNew(item.ID),
                                IsSelected = AppManager.I.Player.CurrentAnturaCustomizations.TexturePack.BaseId == item.ID
                            });
                        } else {
                            returnList.Add(null);
                        }
                    }
                    /// - Charge texture
                    for (int i = 0; i < returnList.Count; i++) {
                        if (returnList[i] != null) {
                            string texturePath = "AnturaStuff/Textures_and_Materials/";
                            Texture2D inputTexture = Resources.Load<Texture2D>(texturePath + returnList[i].ID);
                            _parentsTransForModels[i].GetComponent<RawImage>().texture = inputTexture;
                        }
                    }
                    break;
                case RewardBaseType.Decal:
                    // Filter from unlocked elements (only one for itemID)
                    foreach (var item in ItemsConfig.DecalBases)
                    {
                        var unlockedPacks = GetUnlockedRewardPacks(baseType);
                        if (unlockedPacks.Any(ur => ur.BaseId == item.ID))
                        {
                            returnList.Add(new RewardItem() {
                                ID = item.ID,
                                IsNew = RewardItemIsNew(item.ID),
                                IsSelected = AppManager.I.Player.CurrentAnturaCustomizations.DecalPack.BaseId == item.ID
                            });
                        } else {
                            returnList.Add(null);
                        }
                    }
                    /// - Charge texture
                    for (int i = 0; i < returnList.Count; i++) {
                        if (returnList[i] != null) {
                            string texturePath = "AnturaStuff/Textures_and_Materials/";
                            Texture2D inputTexture = Resources.Load<Texture2D>(texturePath + returnList[i].ID);
                            _parentsTransForModels[i].GetComponent<RawImage>().texture = inputTexture;
                        }
                    }
                    break;
                default:
                    Debug.LogWarningFormat("Reward typology requested {0} not found", baseType);
                    break;
            }
            */
            //// add empty results
            //int emptyItemsCount = _parentsTransForModels.Count - returnList.Count;
            //for (int i = 0; i < emptyItemsCount; i++) {
            //    returnList.Add(null);
            //}
            return returnList;
        }

        // OK
        /// <summary>
        /// Selects the reward item (in the UI)
        /// </summary>
        /// <param name="_baseId">The reward base identifier.</param>
        /// <returns></returns>
        public List<RewardColorItem> SelectRewardBase(string _baseId, RewardBaseType baseType)
        {
            List<RewardColorItem> returnList = new List<RewardColorItem>();

            /// logic
            /// - Trigger selected reward event.
            /// - Load returnList of color for reward checking unlocked and if exist active one

            /*
            var unlockedPacks = GetUnlockedRewardPacks(baseType);
            foreach (var unlockedPack in unlockedPacks)
            {
                RewardColorItem rci = new RewardColorItem(unlockedPack.ColorId);
                rci.IsNew = AppManager.I.Player.RewardPackUnlockDataList.Exists(ur =>
                    ur.ItemId == _baseId && ur.ColorId == color.ID && ur.IsNew == true);
                returnList.Add(rci);
                returnList.Add(rci);
            }

            var colors = ItemsConfig.GetColorsForType(baseType);
            foreach (RewardColor color in colors)
            {
                returnList.Add(rci);
            }

            switch (baseType) {
            case RewardBaseType.Prop:

                foreach (RewardColor color in ItemsConfig.PropColors)
                    {
                        if (AppManager.I.Player.RewardPackUnlockDataList.Exists(ur => ur.base == _baseId && ur.ColorId == color.ID))
                        {
                            RewardColorItem rci = new RewardColorItem(color);
                            rci.IsNew = AppManager.I.Player.RewardPackUnlockDataList.Exists(ur =>
                                ur.ItemId == _baseId && ur.ColorId == color.ID && ur.IsNew == true);
                            returnList.Add(rci);
                        } else {
                            returnList.Add(null);
                        }
                    }
                    // set current reward in modification
                    CurrentSelectedReward = new RewardPackUnlockData() { ItemId = _baseId, BaseType = RewardBaseType.Prop };
                    break;
                case RewardBaseType.Texture:
                    foreach (RewardColor color in ItemsConfig.TextureColors) {
                        if (AppManager.I.Player.RewardPackUnlockDataList.Exists(ur => ur.ItemId == _baseId && ur.ColorId == color.ID)) {
                            RewardColorItem rci = new RewardColorItem(color);
                            rci.IsNew = AppManager.I.Player.RewardPackUnlockDataList.Exists(ur =>
                                ur.ItemId == _baseId && ur.ColorId == color.ID && ur.IsNew == true);
                            rci.Color2RGB = rci.Color1RGB; // to avoid exadecimal conversion error on ui rgb code conversion.
                            returnList.Add(rci);
                        } else {
                            returnList.Add(null);
                        }
                    }
                    // set current reward in modification
                    CurrentSelectedReward = new RewardPackUnlockData() { ItemId = _baseId, BaseType = RewardBaseType.Texture };
                    break;
                case RewardBaseType.Decal:
                    foreach (RewardColor color in ItemsConfig.DecalColors) {
                        if (AppManager.I.Player.RewardPackUnlockDataList.Exists(ur => ur.ItemId == _baseId && ur.ColorId == color.ID)) {
                            RewardColorItem rci = new RewardColorItem(color);
                            rci.IsNew = AppManager.I.Player.RewardPackUnlockDataList.Exists(ur =>
                                ur.ItemId == _baseId && ur.ColorId == color.ID && ur.IsNew == true);
                            rci.Color2RGB = rci.Color1RGB; // to avoid exadecimal conversion error on ui rgb code conversion.
                            returnList.Add(rci);
                        } else {
                            returnList.Add(null);
                        }
                    }
                    //foreach (RewardColor color in config.DecalColors) {
                    //    RewardColorItem rci = new RewardColorItem(color);
                    //    rci.Color2RGB = rci.Color1RGB; // to avoid exadecimal conversion error on ui rgb code conversion.
                    //    returnList.Add(rci);
                    //}
                    // set current reward in modification
                    CurrentSelectedReward = new RewardPack() { ItemId = _baseId, BaseType = RewardBaseType.Decal };
                    break;
                default:
                    Debug.LogWarningFormat("Reward typology requested {0} not found", baseType);
                    break;
            }

            // Color selection
            RewardPackUnlockData alreadySelectedReward = null;
            switch (baseType) {
                case RewardBaseType.Prop:
                    List<RewardPackUnlockData> fornitures = AppManager.I.Player.CurrentAnturaCustomizations.PropPacks;
                    alreadySelectedReward = fornitures.Find(r => r.ItemId == _baseId && r.BaseType == baseType);
                    break;
                case RewardBaseType.Texture:
                    if (AppManager.I.Player.CurrentAnturaCustomizations.TexturePack.ItemId == _baseId)
                        alreadySelectedReward = AppManager.I.Player.CurrentAnturaCustomizations.TexturePack;
                    break;
                case RewardBaseType.Decal:
                    if (AppManager.I.Player.CurrentAnturaCustomizations.DecalPack.ItemId == _baseId)
                        alreadySelectedReward = AppManager.I.Player.CurrentAnturaCustomizations.DecalPack;
                    break;
                default:
                    Debug.LogErrorFormat("Reward type {0} not found!", baseType);
                    return returnList;
            }

            if (alreadySelectedReward != null) {
                // if previous selected this reward use previous color...
                returnList.Find(color => color != null && color.ID == alreadySelectedReward.ColorId).IsSelected = true;
            } else {
                // ...else selecting first available color
                foreach (var firstItem in returnList) {
                    if (firstItem != null) {
                        firstItem.IsSelected = true;
                        return returnList;
                    }
                }
            }
            */
            return returnList;
        }

        // OK
        /// <summary>
        /// Selects the reward color item.
        /// </summary>
        /// <param name="_rewardColorItemId">The reward color item identifier.</param>
        /// <param name="rewardBaseType">Type of the reward.</param>
        public void SelectRewardColorItem(string _rewardColorItemId)
        {
            // TODO: 
            // CurrentSelectedReward.ColorId = _rewardColorItemId;
            if (OnRewardChanged != null)
                OnRewardChanged(CurrentSelectedReward);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_categoryRewardId"></param>
        /*public static void DeselectAllRewardItemsForCategory(string _categoryRewardId = "")
        {
            AnturaModelManager.I.ClearLoadedRewardInCategory(_categoryRewardId);
        }*/

        /// <summary>
        /// TODO: public or private?
        /// Gets the reward colors by identifier.
        /// </summary>
        /// <param name="_rewardItemId">The reward item identifier.</param>
        /// <returns></returns>
        /*static List<RewardColorItem> GetRewardColorsById(string _rewardItemId, RewardTypes _rewardType)
        {
            List<RewardColorItem> returnList = new List<RewardColorItem>();
            // TODO: logic
            return returnList;
        }*/

        /// <summary>
        /// Gets the antura rotation angle view for reward category.
        /// </summary>
        /// <param name="_categoryId">The category identifier.</param>
        /// <returns></returns>
        public float GetAnturaRotationAngleViewForRewardCategory(string _categoryId)
        {
            switch (_categoryId)
            {
                case "HEAD":
                    return 20;
                case "NOSE":
                    return -20;
                case "BACK":
                    return 200;
                case "NECK":
                    return 80;
                case "JAW":
                    return 30;
                case "TAIL":
                    return 160;
                case "EAR_R":
                    return -40;
                case "EAR_L":
                    return 40;
                default:
                    return 0;
            }
        }

        #endregion
        
    }

}