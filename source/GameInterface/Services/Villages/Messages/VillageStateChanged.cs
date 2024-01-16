﻿using Common.Messaging;

namespace GameInterface.Services.Villages.Messages;

/// <summary>
/// TODO update summary
/// A command changes the state of something
/// </summary>
public record VillageStateChanged : ICommand
{
    public string SettlementId { get; }
    public int State { get; }

    public VillageStateChanged(string settlementId, int state)
    {
        SettlementId = settlementId;
        State = state;
    }
}
