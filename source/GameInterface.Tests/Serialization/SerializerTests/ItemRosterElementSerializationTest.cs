﻿using GameInterface.Serialization.External;
using GameInterface.Serialization;
using Xunit;
using TaleWorlds.Core;
using System.Reflection;
using Autofac;
using Common.Serialization;
using GameInterface.Tests.Bootstrap.Modules;

namespace GameInterface.Tests.Serialization.SerializerTests
{
    public class ItemRosterElementSerializationTest
    {
        IContainer container;
        public ItemRosterElementSerializationTest()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterModule<SerializationTestModule>();

            container = builder.Build();
        }

        [Fact]
        public void ItemRosterElement_Serialize()
        {
            ItemRosterElement itemRosterElement = new ItemRosterElement();

            var factory = container.Resolve<IBinaryPackageFactory>();
            ItemRosterElementBinaryPackage package = new ItemRosterElementBinaryPackage(itemRosterElement, factory);

            package.Pack();

            byte[] bytes = BinaryFormatterSerializer.Serialize(package);

            Assert.NotEmpty(bytes);
        }
        private static readonly FieldInfo _amount = typeof(ItemRosterElement).GetField("_amount", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        private static readonly PropertyInfo EquipmentElementProperty = typeof(ItemRosterElement).GetProperty("EquipmentElement");
        [Fact]
        public void ItemRosterElement_Full_Serialization()
        {
            ItemRosterElement itemRosterElement = new ItemRosterElement();
            _amount.SetValue(itemRosterElement, 5);
            EquipmentElementProperty.SetValue(itemRosterElement, new EquipmentElement());
            var factory = container.Resolve<IBinaryPackageFactory>();
            ItemRosterElementBinaryPackage package = new ItemRosterElementBinaryPackage(itemRosterElement, factory);

            package.Pack();

            byte[] bytes = BinaryFormatterSerializer.Serialize(package);

            Assert.NotEmpty(bytes);

            object obj = BinaryFormatterSerializer.Deserialize(bytes);

            Assert.IsType<ItemRosterElementBinaryPackage>(obj);

            ItemRosterElementBinaryPackage returnedPackage = (ItemRosterElementBinaryPackage)obj;

            var deserializeFactory = container.Resolve<IBinaryPackageFactory>();
            ItemRosterElement newRosterElement = returnedPackage.Unpack<ItemRosterElement>(deserializeFactory);

            //Equals is defined for ItemRosterElement
            Assert.Equal(itemRosterElement, newRosterElement);
        }
    }
}
