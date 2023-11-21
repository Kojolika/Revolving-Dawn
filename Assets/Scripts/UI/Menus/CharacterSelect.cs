using System.Collections.Generic;
using Data.Definitions.Player;
using UI.Menus.Common;
using Utils.Attributes;
using UnityEngine;
using UI.Common.DisplayElements;
using Tooling.Logging;

namespace UI.Menus
{
    public class CharacterSelect : Menu<CharacterSelect.Data>
    {
        public class Data
        {
            public List<ClassDefinition> Classes;

            public Data(List<ClassDefinition> classes)
            {
                Classes = classes;
            }
        }

        [ResourcePath]
        public string ResourcePath => nameof(CharacterSelect);

        [SerializeField] ClassDisplayElement classDisplayElementPrefab;
        [SerializeField] Transform classDisplayListRoot;

        List<ClassDisplayElement> classDisplayElements = new ();

        public override void Populate(Data data)
        {
            classDisplayElements.Clear();
            foreach (var classDef in data.Classes)
            {
                var newClassDisplayElement = Instantiate(classDisplayElementPrefab, classDisplayListRoot);
                classDisplayElements.Add(newClassDisplayElement);
                newClassDisplayElement.Populate(classDef);
                newClassDisplayElement.gameObject.SetActive(true);
            }
        }
    }
}