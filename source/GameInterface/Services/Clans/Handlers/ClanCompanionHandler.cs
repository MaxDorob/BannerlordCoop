﻿using Common.Logging;
using Common.Messaging;
using Common.Network;
using GameInterface.Services.Clans.Messages;
using GameInterface.Services.Clans.Patches;
using GameInterface.Services.ObjectManager;
using Serilog;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Localization;

namespace GameInterface.Services.Clans.Handlers
{
    /// <summary>
    /// Handles all changes to clans on client.
    /// </summary>
    public class ClanCompanionHandler : IHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly IObjectManager objectManager;
        private readonly ILogger Logger = LogManager.GetLogger<ClanCompanionHandler>();

        public ClanCompanionHandler(IMessageBroker messageBroker, IObjectManager objectManager)
        {
            this.messageBroker = messageBroker;
            this.objectManager = objectManager;
            messageBroker.Subscribe<AddCompanion>(Handle);
        }

        public void Dispose()
        {
            messageBroker.Unsubscribe<AddCompanion>(Handle);
        }

        private void Handle(MessagePayload<AddCompanion> obj)
        {
            var payload = obj.What;

            objectManager.TryGetObject<Clan>(payload.ClanId, out var clan);

            objectManager.TryGetObject<Hero>(payload.CompanionId, out var companion);

            ClanAddCompanionPatch.RunOriginalAddCompanion(clan, companion);
        }
    }
}