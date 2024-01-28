﻿using Common.Messaging;
using ProtoBuf;

namespace Coop.Core.Server.Services.Settlements.Messages;


/// <summary>
/// Sends the client information on enemies changed.
/// </summary>
[ProtoContract(SkipConstructor = true)]
public record NetworkChangeSettlementEnemiesSpotted : IEvent
{
    [ProtoMember(1)]
    public string SettlementId { get; }
    [ProtoMember(2)]
    public float NumberOfEnemiesSpottedAround {  get; }

    public NetworkChangeSettlementEnemiesSpotted(string settlementID, float numberOfEnemiesSpottedAround)
    {
        SettlementId = settlementID;
        NumberOfEnemiesSpottedAround = numberOfEnemiesSpottedAround;
    }
}
