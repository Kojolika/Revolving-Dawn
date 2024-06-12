using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Models.Buffs
{
    public interface IBuffable
    {
        BuffList Buffs { get; }
    }

    [System.Serializable]
    public class BuffList : IList<Buff>
    {
        public readonly List<Buff> buffs;

        public BuffList(List<Buff> buffs = null)
        {
            this.buffs = buffs ?? new List<Buff>();
        }

        public Buff this[int index] { get => buffs[index]; set => buffs[index] = value; }

        public int Count => buffs.Count;

        public bool IsReadOnly => false;

        public void Add(Buff buff)
        {
            var alreadyAppliedBuff = buffs.Where(b => b.GetType() == buff.GetType()).FirstOrDefault();
            if (alreadyAppliedBuff == null)
            {
                buffs.Add(buff);
            }
            else
            {
                alreadyAppliedBuff.StackSize += buff.StackSize;
            }
        }

        public void AddBuffs(List<Buff> buffs) => buffs.ForEach(buff => Add(buff));
        public void Clear() => buffs.Clear();
        public bool Contains(Buff buff) => buffs.Where(b => b.GetType() == buff.GetType()).Count() > 0;
        public void CopyTo(Buff[] buff, int arrayIndex) => buffs.CopyTo(buff, arrayIndex);
        public IEnumerator<Buff> GetEnumerator() => buffs.GetEnumerator();
        public int IndexOf(Buff item) => buffs.IndexOf(item);
        public void Insert(int index, Buff item) => buffs.Insert(index, item);
        public bool Remove(Buff item) => buffs.Remove(item);
        public void RemoveAt(int index) => buffs.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => buffs.GetEnumerator();
    }
}
