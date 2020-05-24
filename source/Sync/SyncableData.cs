﻿using JetBrains.Annotations;

namespace Sync
{
    public class SyncableData
    {
        public SyncableData([NotNull] SyncValue syncable, object target, object value)
        {
            Syncable = syncable;
            Target = target;
            Value = value;
        }

        public SyncValue Syncable { get; }
        public object Target { get; }
        public object Value { get; }
    }
}
