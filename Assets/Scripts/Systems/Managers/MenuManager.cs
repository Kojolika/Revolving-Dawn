using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Data;
using Systems.Managers.Base;
using UI.Menus.Common;
using Utils.Attributes;

namespace Systems.Managers
{
    public class MenuManager : AbstractSOManager
    {
        public static List<MenuInfo> MenuStack { get; private set; } = new List<MenuInfo>();

        [SerializeField] private Canvas menuCanvasPrefab;

        private Canvas menuCanvas;
        public override UniTask Startup()
        {
            menuCanvas = Instantiate(menuCanvasPrefab);

            return base.Startup();
        }

        public override UniTask AfterStart()
        {
            Base.Managers.GetManagerOfType<MySceneManager>().AddObjectToNotDestroyOnLoad(menuCanvas);

            return base.AfterStart();
        }

        private void Push<M, D>(M menu)
            where M : Menu<D>
            where D : class?
        {
            MenuStack.Add(new MenuInfo(typeof(M)));
        }

        public async UniTask OpenMenu<M, D>(D data)
            where M : Menu<D>
            where D : class?
        {
            string resourcePath = default;

            foreach (var property in typeof(M).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                var attributes = property.GetCustomAttributes(typeof(ResourcePathAttribute), true);
                if (attributes.Length > 0)
                {
                    // Since the property is static, their is no instance for the class, set obj value to null
                    resourcePath = (string)property.GetValue(null);
                }
            }

            Debug.Assert(resourcePath != default, "Menu doesn't have an addressable resource path!");

            menu.Populate(data);

            await menu.PopulateAsync(data);

            Push<M, D>(menu);
        }


        public class MenuInfo
        {
            public MenuInfo(System.Type type)
            {
                this.type = type;
            }

            public System.Type type;
            public int SortingOrder;
        }
    }
}