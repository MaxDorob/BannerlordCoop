﻿using Coop.Core.Client.Services.Armies.Messages;
using Coop.IntegrationTests.Environment;
using Coop.IntegrationTests.Environment.Instance;
using GameInterface.Services.Armies.Messages;


namespace Coop.IntegrationTests.Armies
{
    public class RemoveMobilePartyArmyTest
    {
        // Creates a test environment with 1 server and 2 clients by default
        internal TestEnvironment TestEnvironment { get; } = new TestEnvironment();

        /// <summary>
        /// Used to Test that client receives RemoveMobilePartyInArmy messages.
        /// </summary>
        [Fact]
        public void ServerMobilePartyInArmyRemoved_Publishes_AllClients()
        {
            // Arrange
            string mobilePartyId = "vassal_v2";
            string armyId = "CoopArmy_2";
            var triggerMessage = new MobilePartyInArmyRemoved(mobilePartyId, armyId);

            var server = TestEnvironment.Server;

            // Act
            server.SimulateMessage(this, triggerMessage);

            // Assert
            // Verify the server sends a single message to its game interface
            Assert.Equal(1, server.NetworkSentMessages.GetMessageCount<NetworkRemoveMobilePartyInArmy>());

            //Verify if the server is sending the same mobilePartyId and leaderMobilePartyId value
            Assert.Equal(mobilePartyId, server.NetworkSentMessages.GetMessages<NetworkRemoveMobilePartyInArmy>().First().MobilePartyId);
            Assert.Equal(armyId, server.NetworkSentMessages.GetMessages<NetworkRemoveMobilePartyInArmy>().First().ArmyId);


            // Verify all clients receive a single message to their game interfaces
            foreach (EnvironmentInstance client in TestEnvironment.Clients)
            {
                Assert.Equal(1, client.InternalMessages.GetMessageCount<RemoveMobilePartyInArmy>());
            }

            // Verify all clients receive the same mobilePartyId and leaderMobilePartyId value
            foreach (EnvironmentInstance client in TestEnvironment.Clients)
            {
                Assert.Equal(mobilePartyId, client.InternalMessages.GetMessages<RemoveMobilePartyInArmy>().First().MobilePartyId);
                Assert.Equal(armyId, client.InternalMessages.GetMessages<RemoveMobilePartyInArmy>().First().ArmyId);
            }


        }
    }
}
