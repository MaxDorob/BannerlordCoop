﻿using Common.Messaging;
using Coop.Mod.LogicStates.Client;
using System;

namespace Coop.Mod.Client.States
{
    internal class CampaignState : ClientStateBase
    {
        public CampaignState(IClientLogic logic, IMessageBroker messageBroker) : base(logic, messageBroker)
        {
        }

        public override void Connect()
        {
            throw new NotImplementedException();
        }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override void EnterMainMenu()
        {
            throw new NotImplementedException();
        }

        public override void ExitGame()
        {
            throw new NotImplementedException();
        }

        public override void LoadSavedData()
        {
            throw new NotImplementedException();
        }

        public override void StartCharacterCreation()
        {
            throw new NotImplementedException();
        }
    }
}
