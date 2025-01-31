﻿using Common.Messaging;
using GameInterface.Services.MobileParties.Messages.Lifetime;
using GameInterface.Services.ObjectManager.Extensions;
using GameInterface.Services.Registry;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace GameInterface.Services.MobileParties;

/// <summary>
/// Registry for <see cref="MobileParty"/> objects
/// </summary>
internal class MobilePartyRegistry : RegistryBase<MobileParty>
{
    private const string PartyStringIdPrefix = "CoopParty";
    private readonly IMessageBroker messageBroker;

    public MobilePartyRegistry(IRegistryCollection collection, IMessageBroker messageBroker) : base(collection)
    {
        this.messageBroker = messageBroker;

        messageBroker.Subscribe<PartyDestroyed>(Handle_PartyDestroyed);
    }

    public override void RegisterAll()
    {
        var objectManager = Campaign.Current?.CampaignObjectManager;

        if (objectManager == null)
        {
            Logger.Error("Unable to register objects when CampaignObjectManager is null");
            return;
        }

        foreach (var party in objectManager.MobileParties)
        {
            base.RegisterExistingObject(party.StringId, party);
        }
    }

    public override bool RegisterExistingObject(string id, object obj)
    {
        var result = base.RegisterExistingObject(id, obj);

        AddToCampaignObjectManager(obj);

        return result;
    }

    private void AddToCampaignObjectManager(object obj)
    {
        if (TryCast(obj, out var castedObj) == false) return;

        var objectManager = Campaign.Current?.CampaignObjectManager;

        if (objectManager == null) return;

        objectManager.AddMobileParty(castedObj);
    }

    protected override string GetNewId(MobileParty party)
    {
        party.StringId = Campaign.Current.CampaignObjectManager.FindNextUniqueStringId<MobileParty>(PartyStringIdPrefix);
        return party.StringId;
    }

    private void Handle_PartyDestroyed(MessagePayload<PartyDestroyed> payload)
    {
        var stringId = payload.What.Data.StringId;

        Remove(stringId);
    }
}
