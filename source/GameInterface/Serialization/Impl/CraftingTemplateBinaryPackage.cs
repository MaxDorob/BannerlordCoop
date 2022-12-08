﻿using Common.Extensions;
using System;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace GameInterface.Serialization.Impl
{
    [Serializable]
    public class CraftingTemplateBinaryPackage : BinaryPackageBase<CraftingTemplate>
    {
        public string templateId;

        public CraftingTemplateBinaryPackage(CraftingTemplate obj, BinaryPackageFactory binaryPackageFactory) : base(obj, binaryPackageFactory)
        {

        }
        public override void Pack()
        {
            //templateId = Object.StringId;
            foreach (FieldInfo field in ObjectType.GetAllInstanceFields())
            {
                object obj = field.GetValue(Object);
                StoredFields.Add(field, BinaryPackageFactory.GetBinaryPackage(obj));
            }
        }
        protected override void UnpackInternal()
        {
            //Object = MBObjectManager.Instance.GetObject<CraftingTemplate>(templateId);
            TypedReference reference = __makeref(Object);
            foreach (FieldInfo field in StoredFields.Keys)
            {
                field.SetValueDirect(reference, StoredFields[field].Unpack());
            }
        }
    }
}

