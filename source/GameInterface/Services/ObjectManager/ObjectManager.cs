﻿using GameInterface.Services.Clans;
using GameInterface.Services.MobileParties;
using GameInterface.Services.ObjectManager.Extensions;
using GameInterface.Services.Registry;
using GameInterface.Services.Settlements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.ObjectSystem;

namespace GameInterface.Services.ObjectManager;

public interface IObjectManager
{
    /// <summary>
    /// Determins if an object is stored in the object manager
    /// </summary>
    /// <param name="obj">Object to check if stored</param>
    /// <returns>True if stored, false if not</returns>
    bool Contains(object obj);

    /// <summary>
    /// Determins if an StringId is stored in the object manager
    /// </summary>
    /// <param name="id">StringId to check if stored</param>
    /// <returns>True if stored, false if not</returns>
    bool Contains(string id);

    /// <summary>
    /// Attempts to get an object using a StringId and object type
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="id">StringId used to lookup object</param>
    /// <param name="obj">Out parameter for the object</param>
    /// <returns>True if successful, false if failed</returns>
    bool TryGetObject<T>(string id, out T obj) where T : MBObjectBase;

    /// <summary>
    /// Add an object with already existing StringId
    /// </summary>
    /// <param name="id">Id to assosiate with object</param>
    /// <param name="obj">Object to assosiate with id</param>
    /// <returns>True if successful, false if failed</returns>
    bool AddExisting(string id, object obj);

    /// <summary>
    /// Adds an object without a registered StringId
    /// </summary>
    /// <param name="obj">Object to register</param>
    /// <param name="newId">Newly created StringId</param>
    /// <returns>True if successful, false if failed</returns>
    bool AddNewObject(object obj, out string newId);
}

/// <summary>
/// Ground truth for storing and retreiving object and ids
/// </summary>
internal class ObjectManager : IObjectManager
{
    private MBObjectManager objectManager => MBObjectManager.Instance;

    private readonly IHeroRegistry heroRegistry;
    private readonly IMobilePartyRegistry partyRegistry;
    private readonly IClanRegistry clanRegistry;

    public ObjectManager(
        IHeroRegistry heroRegistry,
        IMobilePartyRegistry partyRegistry, 
        IClanRegistry clanRegistry)
    {
        this.heroRegistry = heroRegistry;
        this.partyRegistry = partyRegistry;
        this.clanRegistry = clanRegistry;
    }

    public bool AddExisting(string id, object obj)
    {
        if (string.IsNullOrEmpty(id)) return false;
        if (objectManager == null) return false;
        if (obj is MBObjectBase mbObject == false) return false;

        return AddExistingInternal(id, mbObject);
    }

    private bool AddExistingInternal<T>(string id, T obj) where T : MBObjectBase
    {
        if (string.IsNullOrEmpty(id)) return false;

        obj.StringId = id;

        return obj switch
        {
            MobileParty party => partyRegistry.RegisterExistingObject(id, party),
            Hero hero => heroRegistry.RegisterExistingObject(id, hero),
            Clan clan => clanRegistry.RegisterExistingObject(id, clan),
            _ => objectManager.RegisterPresumedObject(obj) != null,
        };
    }

    public bool AddNewObject(object obj, out string newId)
    {
        newId = null;

        if (objectManager == null) return false;
        if (obj is MBObjectBase mbObject == false) return false;

        return obj switch
        {
            MobileParty party => partyRegistry.RegisterNewObject(party, out newId),
            Hero hero => heroRegistry.RegisterNewObject(hero, out newId),
            Clan clan => clanRegistry.RegisterNewObject(clan, out newId),
            _ => AddNewObjectInternal(mbObject, out newId),
        };
    }


    private static readonly MethodInfo RegisterObject = typeof(MBObjectManager)
        .GetMethod(nameof(MBObjectManager.RegisterObject));
    private bool AddNewObjectInternal(object obj, out string id)
    {
        id = null;

        if (objectManager == null) return false;
        if (obj is MBObjectBase mbObject == false) return false;

        RegisterObject.MakeGenericMethod(obj.GetType()).Invoke(objectManager, new object[] { mbObject });

        return true;
    }

    public bool Contains(object obj)
    {
        if (objectManager == null) return false;
        if (obj is MBObjectBase mbObject == false) return false;

        return obj switch
        {
            MobileParty party => partyRegistry.TryGetValue(party, out string _),
            Hero hero => heroRegistry.TryGetValue(hero, out string _),
            Clan clan => clanRegistry.TryGetValue(clan, out string _),
            _ => Contains(mbObject.StringId),
        };
    }

    
    public bool Contains(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        if (objectManager == null) return false;

        if(partyRegistry.TryGetValue(id, out MobileParty _))
        {
            return true;
        }

        if(heroRegistry.TryGetValue(id, out Hero _))
        {
            return true;
        }

        if (clanRegistry.TryGetValue(id, out Clan _))
        {
            return true;
        }

        return objectManager.Contains(id);
    }

    public bool TryGetId(object obj, out string id)
    {
        id = null;

        if (objectManager == null) return false;
        if (obj is MBObjectBase mbObject == false) return false;

        id = mbObject.StringId;

        return true;
    }

    // TODO refactor
    private readonly Dictionary<Type, Delegate> BranchLookup = new Dictionary<Type, Delegate>()
    {
        { typeof(Hideout), Delegate.CreateDelegate(typeof(GetObjectDelegate), null, typeof(ObjectManager).GetMethod(nameof(TryGetHideout))) }
    };

    private static readonly HashSet<Type> SettlementTypes = new HashSet<Type>()
    {
        typeof(Town),
        typeof(Village),
        typeof(Hideout)
    };

    private static readonly MethodInfo GetObject = typeof(MBObjectManager)
        .GetMethod(nameof(MBObjectManager.GetObject), new Type[] { typeof(string) });
    public bool TryGetObject<T>(string id, out T obj) where T : MBObjectBase
    {
        obj = default;

        if (string.IsNullOrEmpty(id)) return false;
        if (objectManager == null) return false;


        if (partyRegistry.TryGetValue(id, out MobileParty party))
        {
            obj = party as T;
            return obj != null;
        }

        if (heroRegistry.TryGetValue(id, out Hero hero))
        {
            obj = hero as T;
            return obj != null;
        }

        if (clanRegistry.TryGetValue(id, out Clan clan))
        {
            obj = clan as T;
            return obj != null;
        }

        if (BranchLookup.TryGetValue(typeof(T), out var @delegate))
        {
            return (bool)@delegate.DynamicInvoke(this, id, obj);
        }


        obj = (T)GetObject.MakeGenericMethod(typeof(T)).Invoke(objectManager, new object[] { id });

            return obj != null;
    }

    public delegate bool GetObjectDelegate(ObjectManager instance, string id, out object obj);

    private bool TryGetSettlement(string id, out Settlement settlement)
    {
        settlement = objectManager.GetObject<Settlement>(id);

        return settlement != null;
    }

    private bool TryGetHideout(string id, out object obj)
    {
        obj = null;
        if (TryGetSettlement(id, out var settlement) == false) return false;

        if (settlement.IsHideout == false) return false;

        obj = settlement.Hideout;

        return true;
    }

    private bool TryGetTown(string id, out object obj)
    {
        obj = null;
        if (TryGetSettlement(id, out var settlement) == false) return false;

        if (settlement.IsHideout == false) return false;

        obj = settlement.Hideout;

        return true;
    }

    private bool TryGetCastle(string id, out object obj)
    {
        obj = null;
        if (TryGetSettlement(id, out var settlement) == false) return false;

        if (settlement.IsHideout == false) return false;

        obj = settlement.Hideout;

        return true;
    }
}