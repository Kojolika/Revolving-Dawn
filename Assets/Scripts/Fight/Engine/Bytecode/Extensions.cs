using System.Collections.Generic;

namespace Fight.Engine.Bytecode
{
    public static class Extensions
    {
        public static StoreableList<T> ToStoreableList<T>(this List<T> list) where T : IStoreable
        {
            if (list == null)
            {
                return null;
            }

            var storeableList = new StoreableList<T>();
            foreach (var item in list)
            {
                storeableList.Add(item);
            }

            return storeableList;
        }
    }
}