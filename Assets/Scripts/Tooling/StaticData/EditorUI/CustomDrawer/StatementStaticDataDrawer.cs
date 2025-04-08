/*using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI
{
    public class StatementStaticDataDrawer : CustomStaticDataDrawer<Statement>
    {
        protected override VisualElement Draw(Statement data)
        {
            var root = new VisualElement();

            var nameField = new GeneralField<string>(new FieldValueProvider(typeof(Statement).GetField(nameof(Statement.Name)), data));
            nameField.OnValueChanged += _ => InvokeValueChanged();
            root.Add(nameField);

            var inputsField =
                new GeneralField<List<Variable>>(new FieldValueProvider(typeof(Statement).GetField(nameof(Statement.Inputs)), data));
            inputsField.OnValueChanged += _ => InvokeValueChanged();
            root.Add(inputsField);

            var listView = new ListView
            {
                //itemsSource = itemsSource,
                //makeItem = () => new GeneralField(),
                //bindItem = (item, index) => ((GeneralField)item).BindArrayIndex(index),
                //unbindItem = (item, _) => ((GeneralField)item).BindArrayIndex(-1),
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                reorderable = true,
                showBorder = true,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                reorderMode = ListViewReorderMode.Animated,
                showFoldoutHeader = true,
                headerTitle = nameof(Statement.Instructions),
                showAddRemoveFooter = true,
                showBoundCollectionSize = false,
                horizontalScrollingEnabled = true
            };

            listView.itemsAdded += _ => InvokeValueChanged();
            listView.itemsRemoved += _ => InvokeValueChanged();
            listView.itemsSourceChanged += InvokeValueChanged;
            listView.itemIndexChanged += (_, _) => InvokeValueChanged();

            root.Add(listView);

            return root;
        }

        private class InstructionView : VisualElement
        {
            public int Index;
            InstructionType instructionType;

            public enum InstructionType
            {
                CreateVariable,
                AssignVariable,
            }

            public enum Source
            {
                Manual,
                GameFunction,
                StaticData
            }

            public InstructionView(int index)
            {
                RefreshView(index, InstructionType.AssignVariable);
            }

            private void RefreshView(int index, InstructionType instructionType)
            {
                Clear();

                Index = index;
                this.instructionType = instructionType;

                switch (instructionType)
                {
                    case InstructionType.CreateVariable:
                        Add(new CreateVariableView());
                        break;
                    case InstructionType.AssignVariable:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(instructionType), instructionType, null);
                }
            }


            private class CreateVariableView : VisualElement
            {
                private string varName;
                private readonly InstructionType instructionType = InstructionType.CreateVariable;

                public CreateVariableView()
                {
                    var nameField =
                        new GeneralField<string>(new FieldValueProvider(typeof(CreateVariableView).GetField(nameof(varName)), this));
                    Add(nameField);

                    nameField.OnValueChanged += newName => varName = newName.ToString();
                }
            }
        }

        public interface ISerializeable
        {
            public Dictionary<string, object> ToDictionary();
            public void FromDictionary(Dictionary<string, object> dictionary);
        }
    }
}*/