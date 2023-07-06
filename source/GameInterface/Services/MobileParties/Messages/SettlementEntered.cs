﻿using Common.Messaging;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameInterface.Services.MobileParties.Messages
{
    /// <summary>
    /// Event fired when the local player enters a settlement.
    /// </summary>
    public record SettlementEntered : IEvent
    {
        public string SettlementId;
        public string PartyId;

        public SettlementEntered(string settlementId, string partyId)
        {
            SettlementId = settlementId;
            PartyId = partyId;
        }
    }
}
