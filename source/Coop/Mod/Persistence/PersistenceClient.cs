﻿using System;
using System.Collections.Generic;
using Common;
using Coop.Mod.Persistence.RemoteAction;
using Coop.NetImpl.LiteNet;
using CoopFramework;
using JetBrains.Annotations;
using Network.Infrastructure;
using RailgunNet.Connection.Client;
using RemoteAction;
using Sync;
using Sync.Behaviour;

namespace Coop.Mod.Persistence
{
    /// <summary>
    ///     Manages the <see cref="FieldChangeStack" />, that is it calls the responsible handlers
    ///     for all requested changes once per <see cref="Update" />. As the server is currently
    ///     running in the host client, this class is also responsible for triggering the serverside
    ///     field updates!
    /// </summary>
    public class PersistenceClient : IUpdateable
    {
        [NotNull] private readonly RailClient m_RailClient;

        public PersistenceClient(IEnvironmentClient environment)
        {
            Environment = environment;
            m_RailClient = new RailClient(Registry.Client(Environment));
            Room = m_RailClient.StartRoom();
        }

        public IEnvironmentClient Environment { get; }
        [CanBeNull] public RailClientPeer Peer => m_RailClient.ServerPeer;

        [NotNull] public RailClientRoom Room { get; }

        [NotNull] public FieldChangeStack ChangeStack { get; } = new FieldChangeStack();

        public void Update(TimeSpan frameTime)
        {
            // TODO
            // List<object> toRemove = new List<object>();
            // foreach (KeyValuePair<ValueAccess, Dictionary<object, ValueChangeRequest>> buffer in
            //     ChangeStack.BufferedChanges)
            // {
            //     ValueAccess access = buffer.Key;
            //     foreach (KeyValuePair<object, ValueChangeRequest> instanceBuffer in buffer.Value)
            //     {
            //         object instance = instanceBuffer.Key;
            //         if (CheckShouldRemove(access, instance, instanceBuffer.Value))
            //         {
            //             toRemove.Add(instanceBuffer.Key);
            //         }
            //         else if (!instanceBuffer.Value.RequestProcessed)
            //         {
            //             access.Prefix.GetHandler(instance)?.Invoke(
            //                 EActionOrigin.Local, // we want to emulate a local "field setter"
            //                 new object[] { instanceBuffer.Value.RequestedValue });
            //             instanceBuffer.Value.RequestProcessed = true;
            //         }
            //     }
// 
            //     toRemove.ForEach(o => buffer.Value.Remove(o));
            //     toRemove.Clear();
            // }
// 
            m_RailClient.Update();
        }

        public void SetConnection([CanBeNull] ConnectionClient connection)
        {
            m_RailClient.SetPeer((RailNetPeerWrapper) connection?.GameStatePersistence);
        }

        private bool CheckShouldRemove(
            ValueAccess access,
            object instance,
            ValueChangeRequest buffer)
        {
            if (buffer.RequestProcessed && Equals(buffer.RequestedValue, buffer.LatestActualValue))
            {
                return true;
            }

            object currentValue = access.Get(instance);
            if (Equals(currentValue, buffer.LatestActualValue))
            {
                return false;
            }

            if (buffer.RequestProcessed)
            {
                return true;
            }

            buffer.LatestActualValue = currentValue;
            return false;
        }
    }
}
