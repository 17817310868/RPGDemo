using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;

[AttributeUsage(AttributeTargets.All, Inherited = false)]
internal sealed class __DynamicallyInvokableAttribute : Attribute
{
}

namespace GDNet
{
    // Token: 0x02000492 RID: 1170
    //[DebuggerTypeProxy(typeof(Mscorlib_DictionaryDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    [ComVisible(false)]
    [__DynamicallyInvokable]
    [Serializable]
    public class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, ISerializable, IDeserializationCallback
    {
        // Token: 0x06003914 RID: 14612 RVA: 0x000D9474 File Offset: 0x000D7674
        [__DynamicallyInvokable]
        public Dictionary() : this(0, null)
        {
        }

        // Token: 0x06003915 RID: 14613 RVA: 0x000D947E File Offset: 0x000D767E
        [__DynamicallyInvokable]
        public Dictionary(int capacity) : this(capacity, null)
        {
        }

        // Token: 0x06003916 RID: 14614 RVA: 0x000D9488 File Offset: 0x000D7688
        [__DynamicallyInvokable]
        public Dictionary(IEqualityComparer<TKey> comparer) : this(0, comparer)
        {
        }

        // Token: 0x06003917 RID: 14615 RVA: 0x000D9492 File Offset: 0x000D7692
        [__DynamicallyInvokable]
        public Dictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0) {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity);
            }
            if (capacity > 0) {
                Initialize(capacity);
            }
            Comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        // Token: 0x06003918 RID: 14616 RVA: 0x000D94C0 File Offset: 0x000D76C0
        [__DynamicallyInvokable]
        public Dictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null)
        {
        }

        // Token: 0x06003919 RID: 14617 RVA: 0x000D94CC File Offset: 0x000D76CC
        [__DynamicallyInvokable]
        public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : this((dictionary != null) ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null) {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
            }
            foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary) {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        // Token: 0x0600391A RID: 14618 RVA: 0x000D9540 File Offset: 0x000D7740
        protected Dictionary(SerializationInfo info, StreamingContext context)
        {
            HashHelpers.SerializationInfoTable.Add(this, info);
        }

        // Token: 0x170008B6 RID: 2230
        // (get) Token: 0x0600391B RID: 14619 RVA: 0x000D9554 File Offset: 0x000D7754
        [__DynamicallyInvokable]
        public IEqualityComparer<TKey> Comparer { [__DynamicallyInvokable]
            get; private set;
        }

        // Token: 0x170008B7 RID: 2231
        // (get) Token: 0x0600391C RID: 14620 RVA: 0x000D955C File Offset: 0x000D775C
        [__DynamicallyInvokable]
        public int Count {
            [__DynamicallyInvokable]
            get {
                return count - freeCount;
            }
        }

        // Token: 0x170008B8 RID: 2232
        // (get) Token: 0x0600391D RID: 14621 RVA: 0x000D956B File Offset: 0x000D776B
        [__DynamicallyInvokable]
        public KeyCollection Keys {
            [__DynamicallyInvokable]
            get {
                if (keys == null) {
                    keys = new KeyCollection(this);
                }
                return keys;
            }
        }

        // Token: 0x170008B9 RID: 2233
        // (get) Token: 0x0600391E RID: 14622 RVA: 0x000D9587 File Offset: 0x000D7787
        [__DynamicallyInvokable]
        ICollection<TKey> IDictionary<TKey, TValue>.Keys {
            [__DynamicallyInvokable]
            get {
                if (keys == null) {
                    keys = new KeyCollection(this);
                }
                return keys;
            }
        }

        // Token: 0x170008BA RID: 2234
        // (get) Token: 0x0600391F RID: 14623 RVA: 0x000D95A3 File Offset: 0x000D77A3
        [__DynamicallyInvokable]
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys {
            [__DynamicallyInvokable]
            get {
                if (keys == null) {
                    keys = new KeyCollection(this);
                }
                return keys;
            }
        }

        // Token: 0x170008BB RID: 2235
        // (get) Token: 0x06003920 RID: 14624 RVA: 0x000D95BF File Offset: 0x000D77BF
        [__DynamicallyInvokable]
        public ValueCollection Values {
            [__DynamicallyInvokable]
            get {
                if (values == null) {
                    values = new ValueCollection(this);
                }
                return values;
            }
        }

        // Token: 0x170008BC RID: 2236
        // (get) Token: 0x06003921 RID: 14625 RVA: 0x000D95DB File Offset: 0x000D77DB
        [__DynamicallyInvokable]
        ICollection<TValue> IDictionary<TKey, TValue>.Values {
            [__DynamicallyInvokable]
            get {
                if (values == null) {
                    values = new ValueCollection(this);
                }
                return values;
            }
        }

        // Token: 0x170008BD RID: 2237
        // (get) Token: 0x06003922 RID: 14626 RVA: 0x000D95F7 File Offset: 0x000D77F7
        [__DynamicallyInvokable]
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values {
            [__DynamicallyInvokable]
            get {
                if (values == null) {
                    values = new ValueCollection(this);
                }
                return values;
            }
        }

        /// <summary>
        /// 字典索引, 当你索引字典时Index值就是你所取出的值索引
        /// </summary>
        public int Index{ get; private set; }

        // Token: 0x170008BE RID: 2238
        [__DynamicallyInvokable]
        public TValue this[TKey key] {
            [__DynamicallyInvokable]
            get {
                int Index = FindEntry(key);
                if (Index >= 0) {
                    return entries[Index].value;
                }
                ThrowHelper.ThrowKeyNotFoundException();
                return default;
            }
            [__DynamicallyInvokable]
            set {
                Insert(key, value, false);
            }
        }

        // Token: 0x06003925 RID: 14629 RVA: 0x000D9658 File Offset: 0x000D7858
        [__DynamicallyInvokable]
        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        // Token: 0x06003926 RID: 14630 RVA: 0x000D9663 File Offset: 0x000D7863
        [__DynamicallyInvokable]
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        // Token: 0x06003927 RID: 14631 RVA: 0x000D967C File Offset: 0x000D787C
        [__DynamicallyInvokable]
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int num = FindEntry(keyValuePair.Key);
            return num >= 0 && EqualityComparer<TValue>.Default.Equals(entries[num].value, keyValuePair.Value);
        }

        // Token: 0x06003928 RID: 14632 RVA: 0x000D96C4 File Offset: 0x000D78C4
        [__DynamicallyInvokable]
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int num = FindEntry(keyValuePair.Key);
            if (num >= 0 && EqualityComparer<TValue>.Default.Equals(entries[num].value, keyValuePair.Value)) {
                Remove(keyValuePair.Key);
                return true;
            }
            return false;
        }

        // Token: 0x06003929 RID: 14633 RVA: 0x000D9718 File Offset: 0x000D7918
        [__DynamicallyInvokable]
        public void Clear()
        {
            if (count > 0) {
                for (int i = 0; i < buckets.Length; i++) {
                    buckets[i] = -1;
                }
                Array.Clear(entries, 0, count);
                freeList = -1;
                count = 0;
                freeCount = 0;
                version++;
            }
        }

        // Token: 0x0600392A RID: 14634 RVA: 0x000D977F File Offset: 0x000D797F
        [__DynamicallyInvokable]
        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) >= 0;
        }

        // Token: 0x0600392B RID: 14635 RVA: 0x000D9790 File Offset: 0x000D7990
        [__DynamicallyInvokable]
        public bool ContainsValue(TValue value)
        {
            if (value == null) {
                for (int i = 0; i < count; i++) {
                    if (entries[i].hashCode >= 0 && entries[i].value == null) {
                        return true;
                    }
                }
            } else {
                EqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
                for (int j = 0; j < count; j++) {
                    if (entries[j].hashCode >= 0 && @default.Equals(entries[j].value, value)) {
                        return true;
                    }
                }
            }
            return false;
        }

        // Token: 0x0600392C RID: 14636 RVA: 0x000D9830 File Offset: 0x000D7A30
        private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null) {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }
            if (index < 0 || index > array.Length) {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (array.Length - index < Count) {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }
            int num = count;
            Entry[] array2 = entries;
            for (int i = 0; i < num; i++) {
                if (array2[i].hashCode >= 0) {
                    array[index++] = new KeyValuePair<TKey, TValue>(array2[i].key, array2[i].value);
                }
            }
        }

        // Token: 0x0600392D RID: 14637 RVA: 0x000D98BD File Offset: 0x000D7ABD
        [__DynamicallyInvokable]
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this, 2);
        }

        // Token: 0x0600392E RID: 14638 RVA: 0x000D98C6 File Offset: 0x000D7AC6
        [__DynamicallyInvokable]
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this, 2);
        }

        // Token: 0x0600392F RID: 14639 RVA: 0x000D98D4 File Offset: 0x000D7AD4
        [SecurityCritical]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.info);
            }
            info.AddValue("Version", version);
            info.AddValue("Comparer", HashHelpers.GetEqualityComparerForSerialization(Comparer), typeof(IEqualityComparer<TKey>));
            info.AddValue("HashSize", (buckets == null) ? 0 : buckets.Length);
            if (buckets != null) {
                KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[Count];
                CopyTo(array, 0);
                info.AddValue("KeyValuePairs", array, typeof(KeyValuePair<TKey, TValue>[]));
            }
        }

        // Token: 0x06003930 RID: 14640 RVA: 0x000D996C File Offset: 0x000D7B6C
        private int FindEntry(TKey key)
        {
            if (key == null) {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            if (buckets != null) {
                int num = Comparer.GetHashCode(key) & int.MaxValue;
                for (int i = buckets[num % buckets.Length]; i >= 0; i = entries[i].next) {
                    if (entries[i].hashCode == num && Comparer.Equals(entries[i].key, key)) {
                        return i;
                    }
                }
            }
            return -1;
        }

        // Token: 0x06003931 RID: 14641 RVA: 0x000D9A04 File Offset: 0x000D7C04
        private void Initialize(int capacity)
        {
            int prime = HashHelpers.GetPrime(capacity);
            buckets = new int[prime];
            for (int i = 0; i < buckets.Length; i++) {
                buckets[i] = -1;
            }
            entries = new Entry[prime];
            freeList = -1;
        }

        // Token: 0x06003932 RID: 14642 RVA: 0x000D9A54 File Offset: 0x000D7C54
        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null) {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            if (buckets == null) {
                Initialize(0);
            }
            int num = Comparer.GetHashCode(key) & int.MaxValue;
            int num2 = num % buckets.Length;
            int num3 = 0;
            for (int i = buckets[num2]; i >= 0; i = entries[i].next) {
                if (entries[i].hashCode == num && Comparer.Equals(entries[i].key, key)) {
                    if (add) {
                        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_AddingDuplicate);
                    }
                    entries[i].value = value;
                    version++;
                    Index = i;
                    return;
                }
                num3++;
            }
            int num4;
            if (freeCount > 0) {
                num4 = freeList;
                freeList = entries[num4].next;
                freeCount--;
            } else {
                if (count == entries.Length) {
                    Resize();
                    num2 = num % buckets.Length;
                }
                num4 = count;
                count++;
            }
            entries[num4].hashCode = num;
            entries[num4].next = buckets[num2];
            entries[num4].key = key;
            entries[num4].value = value;
            Index = num4;
            buckets[num2] = num4;
            version++;
            if (num3 > 100 && HashHelpers.IsWellKnownEqualityComparer(Comparer)) {
                Comparer = (IEqualityComparer<TKey>)HashHelpers.GetRandomizedEqualityComparer(Comparer);
                Resize(entries.Length, true);
            }
        }

        // Token: 0x06003933 RID: 14643 RVA: 0x000D9C34 File Offset: 0x000D7E34
        public virtual void OnDeserialization(object sender)
        {
            HashHelpers.SerializationInfoTable.TryGetValue(this, out SerializationInfo serializationInfo);
            if (serializationInfo == null) {
                return;
            }
            int @int = serializationInfo.GetInt32("Version");
            int int2 = serializationInfo.GetInt32("HashSize");
            Comparer = (IEqualityComparer<TKey>)serializationInfo.GetValue("Comparer", typeof(IEqualityComparer<TKey>));
            if (int2 != 0) {
                buckets = new int[int2];
                for (int i = 0; i < buckets.Length; i++) {
                    buckets[i] = -1;
                }
                entries = new Entry[int2];
                freeList = -1;
                KeyValuePair<TKey, TValue>[] array = (KeyValuePair<TKey, TValue>[])serializationInfo.GetValue("KeyValuePairs", typeof(KeyValuePair<TKey, TValue>[]));
                if (array == null) {
                    ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_MissingKeys);
                }
                for (int j = 0; j < array.Length; j++) {
                    if (array[j].Key == null) {
                        ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_NullKey);
                    }
                    Insert(array[j].Key, array[j].Value, true);
                }
            } else {
                buckets = null;
            }
            version = @int;
            HashHelpers.SerializationInfoTable.Remove(this);
        }

        // Token: 0x06003934 RID: 14644 RVA: 0x000D9D60 File Offset: 0x000D7F60
        private void Resize()
        {
            Resize(HashHelpers.ExpandPrime(count), false);
        }

        // Token: 0x06003935 RID: 14645 RVA: 0x000D9D74 File Offset: 0x000D7F74
        private void Resize(int newSize, bool forceNewHashCodes)
        {
            int[] array = new int[newSize];
            for (int i = 0; i < array.Length; i++) {
                array[i] = -1;
            }
            Entry[] array2 = new Entry[newSize];
            Array.Copy(entries, 0, array2, 0, count);
            if (forceNewHashCodes) {
                for (int j = 0; j < count; j++) {
                    if (array2[j].hashCode != -1) {
                        array2[j].hashCode = (Comparer.GetHashCode(array2[j].key) & int.MaxValue);
                    }
                }
            }
            for (int k = 0; k < count; k++) {
                if (array2[k].hashCode >= 0) {
                    int num = array2[k].hashCode % newSize;
                    array2[k].next = array[num];
                    array[num] = k;
                }
            }
            buckets = array;
            entries = array2;
        }

        // Token: 0x06003936 RID: 14646 RVA: 0x000D9E5C File Offset: 0x000D805C
        [__DynamicallyInvokable]
        public bool Remove(TKey key)
        {
            if (key == null) {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            if (buckets != null) {
                int num = Comparer.GetHashCode(key) & int.MaxValue;
                int num2 = num % buckets.Length;
                int num3 = -1;
                for (int i = buckets[num2]; i >= 0; i = entries[i].next) {
                    if (entries[i].hashCode == num && Comparer.Equals(entries[i].key, key)) {
                        if (num3 < 0) {
                            buckets[num2] = entries[i].next;
                        } else {
                            entries[num3].next = entries[i].next;
                        }
                        entries[i].hashCode = -1;
                        entries[i].next = freeList;
                        entries[i].key = default;
                        entries[i].value = default;
                        freeList = i;
                        freeCount++;
                        version++;
                        Index = i;
                        return true;
                    }
                    num3 = i;
                }
            }
            return false;
        }

        // Token: 0x06003937 RID: 14647 RVA: 0x000D9FC4 File Offset: 0x000D81C4
        [__DynamicallyInvokable]
        public bool TryGetValue(TKey key, out TValue value)
        {
            int num = FindEntry(key);
            if (num >= 0) {
                value = entries[num].value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        // Token: 0x06003938 RID: 14648 RVA: 0x000DA000 File Offset: 0x000D8200
        internal TValue GetValueOrDefault(TKey key)
        {
            int num = FindEntry(key);
            if (num >= 0) {
                return entries[num].value;
            }
            return default(TValue);
        }

        // Token: 0x170008BF RID: 2239
        // (get) Token: 0x06003939 RID: 14649 RVA: 0x000DA034 File Offset: 0x000D8234
        [__DynamicallyInvokable]
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
            [__DynamicallyInvokable]
            get {
                return false;
            }
        }

        // Token: 0x0600393A RID: 14650 RVA: 0x000DA037 File Offset: 0x000D8237
        [__DynamicallyInvokable]
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            CopyTo(array, index);
        }

        // Token: 0x0600393B RID: 14651 RVA: 0x000DA044 File Offset: 0x000D8244
        [__DynamicallyInvokable]
        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null) {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }
            if (array.Rank != 1) {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            }
            if (array.GetLowerBound(0) != 0) {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
            }
            if (index < 0 || index > array.Length) {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (array.Length - index < Count) {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }
            KeyValuePair<TKey, TValue>[] array2 = array as KeyValuePair<TKey, TValue>[];
            if (array2 != null) {
                CopyTo(array2, index);
                return;
            }
            if (array is DictionaryEntry[]) {
                DictionaryEntry[] array3 = array as DictionaryEntry[];
                Entry[] array4 = entries;
                for (int i = 0; i < count; i++) {
                    if (array4[i].hashCode >= 0) {
                        array3[index++] = new DictionaryEntry(array4[i].key, array4[i].value);
                    }
                }
                return;
            }
            object[] array5 = array as object[];
            if (array5 == null) {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
            }
            try {
                int num = count;
                Entry[] array6 = entries;
                for (int j = 0; j < num; j++) {
                    if (array6[j].hashCode >= 0) {
                        array5[index++] = new KeyValuePair<TKey, TValue>(array6[j].key, array6[j].value);
                    }
                }
            } catch (ArrayTypeMismatchException) {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
            }
        }

        // Token: 0x0600393C RID: 14652 RVA: 0x000DA1B4 File Offset: 0x000D83B4
        [__DynamicallyInvokable]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, 2);
        }

        // Token: 0x170008C0 RID: 2240
        // (get) Token: 0x0600393D RID: 14653 RVA: 0x000DA1C2 File Offset: 0x000D83C2
        [__DynamicallyInvokable]
        bool ICollection.IsSynchronized {
            [__DynamicallyInvokable]
            get {
                return false;
            }
        }

        // Token: 0x170008C1 RID: 2241
        // (get) Token: 0x0600393E RID: 14654 RVA: 0x000DA1C5 File Offset: 0x000D83C5
        [__DynamicallyInvokable]
        object ICollection.SyncRoot {
            [__DynamicallyInvokable]
            get {
                if (_syncRoot == null) {
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        // Token: 0x170008C2 RID: 2242
        // (get) Token: 0x0600393F RID: 14655 RVA: 0x000DA1E7 File Offset: 0x000D83E7
        [__DynamicallyInvokable]
        bool IDictionary.IsFixedSize {
            [__DynamicallyInvokable]
            get {
                return false;
            }
        }

        // Token: 0x170008C3 RID: 2243
        // (get) Token: 0x06003940 RID: 14656 RVA: 0x000DA1EA File Offset: 0x000D83EA
        [__DynamicallyInvokable]
        bool IDictionary.IsReadOnly {
            [__DynamicallyInvokable]
            get {
                return false;
            }
        }

        // Token: 0x170008C4 RID: 2244
        // (get) Token: 0x06003941 RID: 14657 RVA: 0x000DA1ED File Offset: 0x000D83ED
        [__DynamicallyInvokable]
        ICollection IDictionary.Keys {
            [__DynamicallyInvokable]
            get {
                return Keys;
            }
        }

        // Token: 0x170008C5 RID: 2245
        // (get) Token: 0x06003942 RID: 14658 RVA: 0x000DA1F5 File Offset: 0x000D83F5
        [__DynamicallyInvokable]
        ICollection IDictionary.Values {
            [__DynamicallyInvokable]
            get {
                return Values;
            }
        }

        // Token: 0x170008C6 RID: 2246
        [__DynamicallyInvokable]
        object IDictionary.this[object key] {
            [__DynamicallyInvokable]
            get {
                if (Dictionary<TKey, TValue>.IsCompatibleKey(key)) {
                    int num = FindEntry((TKey)(key));
                    if (num >= 0) {
                        return entries[num].value;
                    }
                }
                return null;
            }
            [__DynamicallyInvokable]
            set {
                if (key == null) {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
                }
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, ExceptionArgument.value);
                try {
                    TKey key2 = (TKey)(key);
                    try {
                        this[key2] = (TValue)(value);
                    } catch (InvalidCastException) {
                        ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                    }
                } catch (InvalidCastException) {
                    ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
                }
            }
        }

        // Token: 0x06003945 RID: 14661 RVA: 0x000DA2B8 File Offset: 0x000D84B8
        private static bool IsCompatibleKey(object key)
        {
            if (key == null) {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            return key is TKey;
        }

        // Token: 0x06003946 RID: 14662 RVA: 0x000DA2CC File Offset: 0x000D84CC
        [__DynamicallyInvokable]
        void IDictionary.Add(object key, object value)
        {
            if (key == null) {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, ExceptionArgument.value);
            try {
                TKey key2 = (TKey)(key);
                try {
                    Add(key2, (TValue)(value));
                } catch (InvalidCastException) {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                }
            } catch (InvalidCastException) {
                ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
            }
        }

        // Token: 0x06003947 RID: 14663 RVA: 0x000DA344 File Offset: 0x000D8544
        [__DynamicallyInvokable]
        bool IDictionary.Contains(object key)
        {
            return IsCompatibleKey(key) && ContainsKey((TKey)(key));
        }

        // Token: 0x06003948 RID: 14664 RVA: 0x000DA35C File Offset: 0x000D855C
        [__DynamicallyInvokable]
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(this, 1);
        }

        // Token: 0x06003949 RID: 14665 RVA: 0x000DA36A File Offset: 0x000D856A
        [__DynamicallyInvokable]
        void IDictionary.Remove(object key)
        {
            if (IsCompatibleKey(key)) {
                Remove((TKey)(key));
            }
        }

        // Token: 0x04001885 RID: 6277
        private int[] buckets;

        // Token: 0x04001886 RID: 6278
        private Entry[] entries;

        // Token: 0x04001887 RID: 6279
        private int count;

        // Token: 0x04001888 RID: 6280
        private int version;

        // Token: 0x04001889 RID: 6281
        private int freeList;

        // Token: 0x0400188A RID: 6282
        private int freeCount;

        // Token: 0x0400188C RID: 6284
        private KeyCollection keys;

        // Token: 0x0400188D RID: 6285
        private ValueCollection values;

        // Token: 0x0400188E RID: 6286
        private object _syncRoot;

        // Token: 0x0400188F RID: 6287
        private const string VersionName = "Version";

        // Token: 0x04001890 RID: 6288
        private const string HashSizeName = "HashSize";

        // Token: 0x04001891 RID: 6289
        private const string KeyValuePairsName = "KeyValuePairs";

        // Token: 0x04001892 RID: 6290
        private const string ComparerName = "Comparer";

        // Token: 0x02000BA8 RID: 2984
        private struct Entry
        {
            // Token: 0x040034FE RID: 13566
            public int hashCode;

            // Token: 0x040034FF RID: 13567
            public int next;

            // Token: 0x04003500 RID: 13568
            public TKey key;

            // Token: 0x04003501 RID: 13569
            public TValue value;
        }

        // Token: 0x02000BA9 RID: 2985
        [__DynamicallyInvokable]
        [Serializable]
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator, IDictionaryEnumerator
        {
            // Token: 0x06006DCD RID: 28109 RVA: 0x0017981A File Offset: 0x00177A1A
            internal Enumerator(Dictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                this.dictionary = dictionary;
                version = dictionary.version;
                index = 0;
                this.getEnumeratorRetType = getEnumeratorRetType;
                Current = default(KeyValuePair<TKey, TValue>);
            }

            // Token: 0x06006DCE RID: 28110 RVA: 0x0017984C File Offset: 0x00177A4C
            [__DynamicallyInvokable]
            public bool MoveNext()
            {
                if (version != dictionary.version) {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                while (index < dictionary.count) {
                    if (dictionary.entries[index].hashCode >= 0) {
                        Current = new KeyValuePair<TKey, TValue>(dictionary.entries[index].key, dictionary.entries[index].value);
                        index++;
                        return true;
                    }
                    index++;
                }
                index = dictionary.count + 1;
                Current = default(KeyValuePair<TKey, TValue>);
                return false;
            }

            // Token: 0x170012EC RID: 4844
            // (get) Token: 0x06006DCF RID: 28111 RVA: 0x0017992B File Offset: 0x00177B2B
            [__DynamicallyInvokable]
            public KeyValuePair<TKey, TValue> Current { [__DynamicallyInvokable]
                get; private set;
            }

            // Token: 0x06006DD0 RID: 28112 RVA: 0x00179933 File Offset: 0x00177B33
            [__DynamicallyInvokable]
            public void Dispose()
            {
            }

            // Token: 0x170012ED RID: 4845
            // (get) Token: 0x06006DD1 RID: 28113 RVA: 0x00179938 File Offset: 0x00177B38
            [__DynamicallyInvokable]
            object IEnumerator.Current {
                [__DynamicallyInvokable]
                get {
                    if (index == 0 || index == dictionary.count + 1) {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    if (getEnumeratorRetType == 1) {
                        return new DictionaryEntry(Current.Key, Current.Value);
                    }
                    return new KeyValuePair<TKey, TValue>(Current.Key, Current.Value);
                }
            }

            // Token: 0x06006DD2 RID: 28114 RVA: 0x001799BD File Offset: 0x00177BBD
            [__DynamicallyInvokable]
            void IEnumerator.Reset()
            {
                if (version != dictionary.version) {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                index = 0;
                Current = default(KeyValuePair<TKey, TValue>);
            }

            // Token: 0x170012EE RID: 4846
            // (get) Token: 0x06006DD3 RID: 28115 RVA: 0x001799EC File Offset: 0x00177BEC
            [__DynamicallyInvokable]
            DictionaryEntry IDictionaryEnumerator.Entry {
                [__DynamicallyInvokable]
                get {
                    if (index == 0 || index == dictionary.count + 1) {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return new DictionaryEntry(Current.Key, Current.Value);
                }
            }

            // Token: 0x170012EF RID: 4847
            // (get) Token: 0x06006DD4 RID: 28116 RVA: 0x00179A42 File Offset: 0x00177C42
            [__DynamicallyInvokable]
            object IDictionaryEnumerator.Key {
                [__DynamicallyInvokable]
                get {
                    if (index == 0 || index == dictionary.count + 1) {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return Current.Key;
                }
            }

            // Token: 0x170012F0 RID: 4848
            // (get) Token: 0x06006DD5 RID: 28117 RVA: 0x00179A78 File Offset: 0x00177C78
            [__DynamicallyInvokable]
            object IDictionaryEnumerator.Value {
                [__DynamicallyInvokable]
                get {
                    if (index == 0 || index == dictionary.count + 1) {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return Current.Value;
                }
            }

            // Token: 0x04003502 RID: 13570
            private Dictionary<TKey, TValue> dictionary;

            // Token: 0x04003503 RID: 13571
            private int version;

            // Token: 0x04003504 RID: 13572
            private int index;

            // Token: 0x04003506 RID: 13574
            private int getEnumeratorRetType;

            // Token: 0x04003507 RID: 13575
            internal const int DictEntry = 1;

            // Token: 0x04003508 RID: 13576
            internal const int KeyValuePair = 2;
        }

        // Token: 0x02000BAA RID: 2986
        //[DebuggerTypeProxy(typeof(Mscorlib_DictionaryKeyCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        [__DynamicallyInvokable]
        [Serializable]
        public sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable, ICollection, IReadOnlyCollection<TKey>
        {
            // Token: 0x06006DD6 RID: 28118 RVA: 0x00179AAE File Offset: 0x00177CAE
            [__DynamicallyInvokable]
            public KeyCollection(Dictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null) {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
                }
                this.dictionary = dictionary;
            }

            // Token: 0x06006DD7 RID: 28119 RVA: 0x00179AC6 File Offset: 0x00177CC6
            [__DynamicallyInvokable]
            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            // Token: 0x06006DD8 RID: 28120 RVA: 0x00179AD4 File Offset: 0x00177CD4
            [__DynamicallyInvokable]
            public void CopyTo(TKey[] array, int index)
            {
                if (array == null) {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (index < 0 || index > array.Length) {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (array.Length - index < dictionary.Count) {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                for (int i = 0; i < count; i++) {
                    if (entries[i].hashCode >= 0) {
                        array[index++] = entries[i].key;
                    }
                }
            }

            // Token: 0x170012F1 RID: 4849
            // (get) Token: 0x06006DD9 RID: 28121 RVA: 0x00179B5F File Offset: 0x00177D5F
            [__DynamicallyInvokable]
            public int Count {
                [__DynamicallyInvokable]
                get {
                    return dictionary.Count;
                }
            }

            // Token: 0x170012F2 RID: 4850
            // (get) Token: 0x06006DDA RID: 28122 RVA: 0x00179B6C File Offset: 0x00177D6C
            [__DynamicallyInvokable]
            bool ICollection<TKey>.IsReadOnly {
                [__DynamicallyInvokable]
                get {
                    return true;
                }
            }

            // Token: 0x06006DDB RID: 28123 RVA: 0x00179B6F File Offset: 0x00177D6F
            [__DynamicallyInvokable]
            void ICollection<TKey>.Add(TKey item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
            }

            // Token: 0x06006DDC RID: 28124 RVA: 0x00179B78 File Offset: 0x00177D78
            [__DynamicallyInvokable]
            void ICollection<TKey>.Clear()
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
            }

            // Token: 0x06006DDD RID: 28125 RVA: 0x00179B81 File Offset: 0x00177D81
            [__DynamicallyInvokable]
            bool ICollection<TKey>.Contains(TKey item)
            {
                return dictionary.ContainsKey(item);
            }

            // Token: 0x06006DDE RID: 28126 RVA: 0x00179B8F File Offset: 0x00177D8F
            [__DynamicallyInvokable]
            bool ICollection<TKey>.Remove(TKey item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
                return false;
            }

            // Token: 0x06006DDF RID: 28127 RVA: 0x00179B99 File Offset: 0x00177D99
            [__DynamicallyInvokable]
            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            // Token: 0x06006DE0 RID: 28128 RVA: 0x00179BAB File Offset: 0x00177DAB
            [__DynamicallyInvokable]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            // Token: 0x06006DE1 RID: 28129 RVA: 0x00179BC0 File Offset: 0x00177DC0
            [__DynamicallyInvokable]
            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null) {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (array.Rank != 1) {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                }
                if (array.GetLowerBound(0) != 0) {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                }
                if (index < 0 || index > array.Length) {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (array.Length - index < dictionary.Count) {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                TKey[] array2 = array as TKey[];
                if (array2 != null) {
                    CopyTo(array2, index);
                    return;
                }
                object[] array3 = array as object[];
                if (array3 == null) {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                }
                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                try {
                    for (int i = 0; i < count; i++) {
                        if (entries[i].hashCode >= 0) {
                            array3[index++] = entries[i].key;
                        }
                    }
                } catch (ArrayTypeMismatchException) {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                }
            }

            // Token: 0x170012F3 RID: 4851
            // (get) Token: 0x06006DE2 RID: 28130 RVA: 0x00179CB8 File Offset: 0x00177EB8
            [__DynamicallyInvokable]
            bool ICollection.IsSynchronized {
                [__DynamicallyInvokable]
                get {
                    return false;
                }
            }

            // Token: 0x170012F4 RID: 4852
            // (get) Token: 0x06006DE3 RID: 28131 RVA: 0x00179CBB File Offset: 0x00177EBB
            [__DynamicallyInvokable]
            object ICollection.SyncRoot {
                [__DynamicallyInvokable]
                get {
                    return ((ICollection)dictionary).SyncRoot;
                }
            }

            // Token: 0x04003509 RID: 13577
            private Dictionary<TKey, TValue> dictionary;

            // Token: 0x02000CD2 RID: 3282
            [__DynamicallyInvokable]
            [Serializable]
            public struct Enumerator : IEnumerator<TKey>, IDisposable, IEnumerator
            {
                // Token: 0x060070DA RID: 28890 RVA: 0x001845CE File Offset: 0x001827CE
                internal Enumerator(Dictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    Current = default(TKey);
                }

                // Token: 0x060070DB RID: 28891 RVA: 0x001845F6 File Offset: 0x001827F6
                [__DynamicallyInvokable]
                public void Dispose()
                {
                }

                // Token: 0x060070DC RID: 28892 RVA: 0x001845F8 File Offset: 0x001827F8
                [__DynamicallyInvokable]
                public bool MoveNext()
                {
                    if (version != dictionary.version) {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                    }
                    while (index < dictionary.count) {
                        if (dictionary.entries[index].hashCode >= 0) {
                            Current = dictionary.entries[index].key;
                            index++;
                            return true;
                        }
                        index++;
                    }
                    index = dictionary.count + 1;
                    Current = default(TKey);
                    return false;
                }

                // Token: 0x1700136F RID: 4975
                // (get) Token: 0x060070DD RID: 28893 RVA: 0x001846B1 File Offset: 0x001828B1
                [__DynamicallyInvokable]
                public TKey Current { [__DynamicallyInvokable]
                    get; private set;
                }

                // Token: 0x17001370 RID: 4976
                // (get) Token: 0x060070DE RID: 28894 RVA: 0x001846B9 File Offset: 0x001828B9
                [__DynamicallyInvokable]
                object IEnumerator.Current {
                    [__DynamicallyInvokable]
                    get {
                        if (index == 0 || index == dictionary.count + 1) {
                            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                        }
                        return Current;
                    }
                }

                // Token: 0x060070DF RID: 28895 RVA: 0x001846EA File Offset: 0x001828EA
                [__DynamicallyInvokable]
                void IEnumerator.Reset()
                {
                    if (version != dictionary.version) {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                    }
                    index = 0;
                    Current = default(TKey);
                }

                // Token: 0x0400386A RID: 14442
                private Dictionary<TKey, TValue> dictionary;

                // Token: 0x0400386B RID: 14443
                private int index;

                // Token: 0x0400386C RID: 14444
                private int version;
            }
        }

        // Token: 0x02000BAB RID: 2987
        //[DebuggerTypeProxy(typeof(Mscorlib_DictionaryValueCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        [__DynamicallyInvokable]
        [Serializable]
        public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable, ICollection, IReadOnlyCollection<TValue>
        {
            // Token: 0x06006DE4 RID: 28132 RVA: 0x00179CC8 File Offset: 0x00177EC8
            [__DynamicallyInvokable]
            public ValueCollection(Dictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null) {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
                }
                this.dictionary = dictionary;
            }

            // Token: 0x06006DE5 RID: 28133 RVA: 0x00179CE0 File Offset: 0x00177EE0
            [__DynamicallyInvokable]
            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            // Token: 0x06006DE6 RID: 28134 RVA: 0x00179CF0 File Offset: 0x00177EF0
            [__DynamicallyInvokable]
            public void CopyTo(TValue[] array, int index)
            {
                if (array == null) {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (index < 0 || index > array.Length) {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (array.Length - index < dictionary.Count) {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                for (int i = 0; i < count; i++) {
                    if (entries[i].hashCode >= 0) {
                        array[index++] = entries[i].value;
                    }
                }
            }

            // Token: 0x170012F5 RID: 4853
            // (get) Token: 0x06006DE7 RID: 28135 RVA: 0x00179D7B File Offset: 0x00177F7B
            [__DynamicallyInvokable]
            public int Count {
                [__DynamicallyInvokable]
                get {
                    return dictionary.Count;
                }
            }

            // Token: 0x170012F6 RID: 4854
            // (get) Token: 0x06006DE8 RID: 28136 RVA: 0x00179D88 File Offset: 0x00177F88
            [__DynamicallyInvokable]
            bool ICollection<TValue>.IsReadOnly {
                [__DynamicallyInvokable]
                get {
                    return true;
                }
            }

            // Token: 0x06006DE9 RID: 28137 RVA: 0x00179D8B File Offset: 0x00177F8B
            [__DynamicallyInvokable]
            void ICollection<TValue>.Add(TValue item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
            }

            // Token: 0x06006DEA RID: 28138 RVA: 0x00179D94 File Offset: 0x00177F94
            [__DynamicallyInvokable]
            bool ICollection<TValue>.Remove(TValue item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
                return false;
            }

            // Token: 0x06006DEB RID: 28139 RVA: 0x00179D9E File Offset: 0x00177F9E
            [__DynamicallyInvokable]
            void ICollection<TValue>.Clear()
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
            }

            // Token: 0x06006DEC RID: 28140 RVA: 0x00179DA7 File Offset: 0x00177FA7
            [__DynamicallyInvokable]
            bool ICollection<TValue>.Contains(TValue item)
            {
                return dictionary.ContainsValue(item);
            }

            // Token: 0x06006DED RID: 28141 RVA: 0x00179DB5 File Offset: 0x00177FB5
            [__DynamicallyInvokable]
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            // Token: 0x06006DEE RID: 28142 RVA: 0x00179DC7 File Offset: 0x00177FC7
            [__DynamicallyInvokable]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            // Token: 0x06006DEF RID: 28143 RVA: 0x00179DDC File Offset: 0x00177FDC
            [__DynamicallyInvokable]
            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null) {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (array.Rank != 1) {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                }
                if (array.GetLowerBound(0) != 0) {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                }
                if (index < 0 || index > array.Length) {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (array.Length - index < dictionary.Count) {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                TValue[] array2 = array as TValue[];
                if (array2 != null) {
                    CopyTo(array2, index);
                    return;
                }
                object[] array3 = array as object[];
                if (array3 == null) {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                }
                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                try {
                    for (int i = 0; i < count; i++) {
                        if (entries[i].hashCode >= 0) {
                            array3[index++] = entries[i].value;
                        }
                    }
                } catch (ArrayTypeMismatchException) {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                }
            }

            // Token: 0x170012F7 RID: 4855
            // (get) Token: 0x06006DF0 RID: 28144 RVA: 0x00179ED4 File Offset: 0x001780D4
            [__DynamicallyInvokable]
            bool ICollection.IsSynchronized {
                [__DynamicallyInvokable]
                get {
                    return false;
                }
            }

            // Token: 0x170012F8 RID: 4856
            // (get) Token: 0x06006DF1 RID: 28145 RVA: 0x00179ED7 File Offset: 0x001780D7
            [__DynamicallyInvokable]
            object ICollection.SyncRoot {
                [__DynamicallyInvokable]
                get {
                    return ((ICollection)dictionary).SyncRoot;
                }
            }

            // Token: 0x0400350A RID: 13578
            private Dictionary<TKey, TValue> dictionary;

            // Token: 0x02000CD3 RID: 3283
            [__DynamicallyInvokable]
            [Serializable]
            public struct Enumerator : IEnumerator<TValue>, IDisposable, IEnumerator
            {
                // Token: 0x060070E0 RID: 28896 RVA: 0x00184719 File Offset: 0x00182919
                internal Enumerator(Dictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    Current = default(TValue);
                }

                // Token: 0x060070E1 RID: 28897 RVA: 0x00184741 File Offset: 0x00182941
                [__DynamicallyInvokable]
                public void Dispose()
                {
                }

                // Token: 0x060070E2 RID: 28898 RVA: 0x00184744 File Offset: 0x00182944
                [__DynamicallyInvokable]
                public bool MoveNext()
                {
                    if (version != dictionary.version) {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                    }
                    while (index < dictionary.count) {
                        if (dictionary.entries[index].hashCode >= 0) {
                            Current = dictionary.entries[index].value;
                            index++;
                            return true;
                        }
                        index++;
                    }
                    index = dictionary.count + 1;
                    Current = default(TValue);
                    return false;
                }

                // Token: 0x17001371 RID: 4977
                // (get) Token: 0x060070E3 RID: 28899 RVA: 0x001847FD File Offset: 0x001829FD
                [__DynamicallyInvokable]
                public TValue Current { [__DynamicallyInvokable]
                    get; private set;
                }

                // Token: 0x17001372 RID: 4978
                // (get) Token: 0x060070E4 RID: 28900 RVA: 0x00184805 File Offset: 0x00182A05
                [__DynamicallyInvokable]
                object IEnumerator.Current {
                    [__DynamicallyInvokable]
                    get {
                        if (index == 0 || index == dictionary.count + 1) {
                            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                        }
                        return Current;
                    }
                }

                // Token: 0x060070E5 RID: 28901 RVA: 0x00184836 File Offset: 0x00182A36
                [__DynamicallyInvokable]
                void IEnumerator.Reset()
                {
                    if (version != dictionary.version) {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                    }
                    index = 0;
                    Current = default(TValue);
                }

                // Token: 0x0400386E RID: 14446
                private Dictionary<TKey, TValue> dictionary;

                // Token: 0x0400386F RID: 14447
                private int index;

                // Token: 0x04003870 RID: 14448
                private int version;
            }
        }
    }
}