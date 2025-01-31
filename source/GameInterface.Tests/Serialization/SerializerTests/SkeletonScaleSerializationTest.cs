﻿using Autofac;
using GameInterface.Serialization;
using GameInterface.Serialization.External;
using GameInterface.Tests.Bootstrap.Modules;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using Xunit;
using Common.Serialization;

namespace GameInterface.Tests.Serialization.SerializerTests
{
    public class SkeletonScaleSerializationTest
    {
        IContainer container;
        public SkeletonScaleSerializationTest()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterModule<SerializationTestModule>();

            container = builder.Build();
        }

        [Fact]
        public void SkeletonScale_Serialize()
        {
            SkeletonScale SkeletonScale = new SkeletonScale();

            var factory = container.Resolve<IBinaryPackageFactory>();
            SkeletonScaleBinaryPackage package = new SkeletonScaleBinaryPackage(SkeletonScale, factory);

            package.Pack();

            byte[] bytes = BinaryFormatterSerializer.Serialize(package);

            Assert.NotEmpty(bytes);
        }


        private static PropertyInfo SkeletonModel = typeof(SkeletonScale).GetProperty(nameof(SkeletonScale.SkeletonModel));
        private static PropertyInfo MountSitBoneScale = typeof(SkeletonScale).GetProperty(nameof(SkeletonScale.MountSitBoneScale));
        private static PropertyInfo MountRadiusAdder = typeof(SkeletonScale).GetProperty(nameof(SkeletonScale.MountRadiusAdder));
        private static PropertyInfo Scales = typeof(SkeletonScale).GetProperty(nameof(SkeletonScale.Scales));
        private static PropertyInfo BoneNames = typeof(SkeletonScale).GetProperty(nameof(SkeletonScale.BoneNames));
        private static PropertyInfo BoneIndices = typeof(SkeletonScale).GetProperty(nameof(SkeletonScale.BoneIndices));
        [Fact]
        public void SkeletonScale_Full_Serialization()
        {
            SkeletonScale SkeletonScale = new SkeletonScale();
            SkeletonModel.SetValue(SkeletonScale, "my model");
            MountSitBoneScale.SetValue(SkeletonScale, new Vec3(1,2,3));
            MountRadiusAdder.SetValue(SkeletonScale, 0.4f);
            Scales.SetValue(SkeletonScale, new[] { new Vec3(3, 2, 3), new Vec3(1, 2, 1) });
            BoneNames.SetValue(SkeletonScale, new List<string> { "str1", "str2" });
            BoneIndices.SetValue(SkeletonScale, new sbyte[] { 1, 2, 3 });

            var factory = container.Resolve<IBinaryPackageFactory>();
            SkeletonScaleBinaryPackage package = new SkeletonScaleBinaryPackage(SkeletonScale, factory);

            package.Pack();

            byte[] bytes = BinaryFormatterSerializer.Serialize(package);

            Assert.NotEmpty(bytes);

            object obj = BinaryFormatterSerializer.Deserialize(bytes);

            Assert.IsType<SkeletonScaleBinaryPackage>(obj);

            SkeletonScaleBinaryPackage returnedPackage = (SkeletonScaleBinaryPackage)obj;

            var deserializeFactory = container.Resolve<IBinaryPackageFactory>();
            SkeletonScale newSkeletonScale = returnedPackage.Unpack<SkeletonScale>(deserializeFactory);

            Assert.Equal(SkeletonScale.SkeletonModel, newSkeletonScale.SkeletonModel);
            Assert.Equal(SkeletonScale.MountSitBoneScale, newSkeletonScale.MountSitBoneScale);
            Assert.Equal(SkeletonScale.MountRadiusAdder, newSkeletonScale.MountRadiusAdder);
            Assert.Equal(SkeletonScale.Scales, newSkeletonScale.Scales);
            Assert.Equal(SkeletonScale.BoneNames, newSkeletonScale.BoneNames);
            Assert.Equal(SkeletonScale.BoneIndices, newSkeletonScale.BoneIndices);
        }
    }
}
