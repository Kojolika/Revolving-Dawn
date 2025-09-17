/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Tooling.StaticData;

namespace Models.Buffs
{
    [System.Serializable]
    public class BuffList : IList<Buff>
    {
        [JsonProperty("buffs")]
        private readonly List<Buff> buffs;

        public event Action<Buff> BuffAmountUpdated;
        public event Action<Buff> BuffRemoved;
        public event Action<Buff> BuffAdded;

        [JsonConstructor]
        public BuffList(List<Buff> buffs = null)
        {
            this.buffs = buffs ?? new List<Buff>();
        }

        public Buff this[int index] { get => buffs[index]; set => buffs[index] = value; }

        public Buff this[Buff buff] { get => GetBuff(buff); }

        public int Count => buffs.Count;

        public bool IsReadOnly => false;

        public void Add(Buff buff)
        {
            var alreadyAppliedBuff = buffs.Where(b => b.GetType() == buff.GetType()).FirstOrDefault();
            if (alreadyAppliedBuff == null)
            {
                buffs.Add(buff);
                BuffAdded?.Invoke(buff);
            }
            else
            {
                alreadyAppliedBuff.StackSize += buff.StackSize;
                BuffAmountUpdated(alreadyAppliedBuff);
            }
        }

        public bool Remove(Buff buff)
        {
            var buffInList = GetBuff(buff);
            if (buffInList == null)
            {
                return false;
            }

            buffInList.StackSize -= buff.StackSize;

            if (buffInList.StackSize == 0)
            {
                buffs.Remove(buff);
            }

            BuffRemoved?.Invoke(buff);

            return true;
        }
        public void AddBuffs(List<Buff> buffs)
        {
            buffs.ForEach(buff =>
            {
                Add(buff);
                BuffAdded?.Invoke(buff);
            });
        }
        public void Clear() => buffs.Clear();
        public Buff GetBuff(Buff buff) => buffs.Where(b => b.GetType() == buff.GetType()).FirstOrDefault();
        public bool Contains(Buff buff) => GetBuff(buff) != null;
        public void CopyTo(Buff[] buff, int arrayIndex) => buffs.CopyTo(buff, arrayIndex);
        public IEnumerator<Buff> GetEnumerator() => buffs.GetEnumerator();
        public int IndexOf(Buff item) => buffs.IndexOf(item);
        public void Insert(int index, Buff item)
        {
            buffs.Insert(index, item);
            BuffAdded?.Invoke(item);
        }
        public void RemoveAt(int index)
        {
            var removedBuff = buffs[index];
            buffs.RemoveAt(index);
            BuffRemoved?.Invoke(removedBuff);
        }
        IEnumerator IEnumerable.GetEnumerator() => buffs.GetEnumerator();
    }
}*/