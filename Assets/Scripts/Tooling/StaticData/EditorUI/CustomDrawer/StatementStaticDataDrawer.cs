using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tooling.Logging;
using Tooling.StaticData.Bytecode;
using UnityEngine.UIElements;
using Utils.Extensions;
using Type = System.Type;
using LiteralType = Tooling.StaticData.Bytecode.Type;

namespace Tooling.StaticData.EditorUI
{
    [UsedImplicitly]
    public class StatementStaticDataDrawer : CustomStaticDataDrawer<Statement>
    {
        protected override VisualElement Draw(Statement statement)
        {
            var root = new VisualElement();

            var nameField = new GeneralField<string>(new FieldValueProvider(typeof(Statement).GetField(nameof(Statement.Name)), statement));
            nameField.OnValueChanged += _ => InvokeValueChanged();
            root.Add(nameField);

            var inputsField = new ListView
            {
                itemsSource = statement.Inputs,
                makeItem = () => new InputView(),
                bindItem = (item, index) =>
                {
                    var inputView = (InputView)item;
                    if (index >= 0 && index < statement.Inputs?.Count)
                    {
                        inputView.SetValueWithoutNotify(statement.Inputs[index]);
                    }
                    else
                    {
                        MyLogger.LogError(
                            $"View data mismatch, statement input count is: {statement.Inputs?.Count} but index is: {index}!");
                    }
                },
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                reorderable = true,
                showBorder = true,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                reorderMode = ListViewReorderMode.Animated,
                showFoldoutHeader = true,
                headerTitle = nameof(Statement.Inputs),
                showAddRemoveFooter = true,
                showBoundCollectionSize = false,
                horizontalScrollingEnabled = true
            };
            inputsField.itemsAdded += _ => InvokeValueChanged();
            inputsField.itemsRemoved += _ => InvokeValueChanged();
            inputsField.itemsSourceChanged += InvokeValueChanged;
            inputsField.itemIndexChanged += (_, _) => InvokeValueChanged();
            root.Add(inputsField);

            statement.Instructions ??= new List<InstructionModel>();
            var listView = new ListView
            {
                itemsSource = statement.Instructions,
                makeItem = () => new InstructionView(statement),
                bindItem = (item, index) =>
                {
                    var instructionView = (InstructionView)item;
                    instructionView.RefreshView(index,
                        InstructionView.GetInstructionTypeOf(statement.Instructions[index]?.GetType() ?? typeof(InstructionModel)));
                    instructionView.OnValueChanged += InvokeValueChanged;
                },
                unbindItem = (item, _) =>
                {
                    var instructionView = (InstructionView)item;
                    instructionView.RefreshView(-1, InstructionView.InstructionType.Unknown);
                    instructionView.OnValueChanged -= InvokeValueChanged;
                },
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

        private class InputView : VisualElement, INotifyValueChanged<Variable>
        {
            public Variable value { get; set; }

            public void SetValueWithoutNotify(Variable value)
            {
                this.value = value;
                RefreshView();
            }

            private void RefreshView()
            {
                Clear();
                value ??= new Variable();

                var nameField = new TextField("Name")
                {
                    value = value.Name
                };
                nameField.RegisterValueChangedCallback(evt => value.Name = evt.newValue);
                Add(nameField);

                var instructionSelection = new EnumField("Instruction Type", value.Type);
                instructionSelection.RegisterValueChangedCallback(evt => { value.Type = (Tooling.StaticData.Bytecode.Type)evt.newValue; });
                Add(instructionSelection);
            }
        }

        public class InstructionView : VisualElement
        {
            public event Action OnValueChanged;
            private readonly Statement statement;

            public enum InstructionType
            {
                Unknown,
                ReadVariable,
                AssignVariable,
            }

            public InstructionView(Statement statement)
            {
                this.statement = statement;
                int index = this.statement.Instructions.Count - 1;
                RefreshView(index, GetInstructionTypeOf(statement.Instructions[index]?.GetType() ?? typeof(InstructionModel)));
            }

            public void RefreshView(int index, InstructionType instructionType)
            {
                Clear();

                if (statement.Instructions.IsNullOrEmpty())
                {
                    return;
                }

                var instructionSelection = new EnumField("Instruction Type", instructionType);
                instructionSelection.RegisterValueChangedCallback(evt =>
                {
                    var newInstruction = (InstructionType)evt.newValue;
                    statement.Instructions[index] = Activator.CreateInstance(GetTypeOf(newInstruction)) as InstructionModel;
                    RefreshView(index, newInstruction);
                    OnValueChanged?.Invoke();
                });
                Add(instructionSelection);

                switch (instructionType)
                {
                    case InstructionType.Unknown:
                    {
                        Add(new Label("Select an instruction"));
                        break;
                    }

                    case InstructionType.ReadVariable:
                    {
                        var definedVariables = new List<Variable>();
                        foreach (var input in statement.Inputs)
                        {
                            definedVariables.Add(input);
                        }

                        for (int i = 0; i < index; i++)
                        {
                            if (statement.Instructions[i] is AssignVariableModel assignVariableModel &&
                                string.IsNullOrEmpty(assignVariableModel.Name))
                            {
                                definedVariables.Add(new Variable
                                {
                                    Name = assignVariableModel.Name,
                                    Type = assignVariableModel.Value.Type
                                });
                            }
                        }

                        var variableSelection = new PopupField<Variable>("Select Variable",
                            definedVariables,
                            definedVariables.FirstOrDefault(),
                            variable => $"{variable.Name} ({variable.Type})",
                            variable => $"{variable.Name} ({variable.Type})");
                        Add(variableSelection);

                        break;
                    }

                    case InstructionType.AssignVariable:
                    {
                        if (statement.Instructions[index] is not AssignVariableModel assignVariable)
                        {
                            MyLogger.LogError($"Index mismatch, statement instruction at index {index}. " +
                                              $"Instruction is not a {typeof(AssignVariableModel)}!");
                            return;
                        }

                        var nameField = new TextField("Variable Name") { value = assignVariable.Name };
                        nameField.RegisterValueChangedCallback(evt =>
                        {
                            assignVariable.Name = evt.newValue;
                            OnValueChanged?.Invoke();
                        });
                        Add(nameField);
                        var valueField = new ValueField(assignVariable.Value, true);
                        valueField.OnValueChanged += OnValueChanged;
                        Add(valueField);
                        break;
                    }
                    default:
                        MyLogger.LogError("Unknown instruction type!");
                        break;
                }
            }


            private static readonly Dictionary<Type, InstructionType> ValidInstructionTypeMap = new()
            {
                { typeof(Unknown), InstructionType.Unknown },
                { typeof(AssignVariableModel), InstructionType.AssignVariable },
                { typeof(ReadVariableModel), InstructionType.ReadVariable }
            };

            public static InstructionType GetInstructionTypeOf(Type type)
            {
                return ValidInstructionTypeMap.GetValueOrDefault(type, InstructionType.Unknown);
            }

            private static Type GetTypeOf(InstructionType instructionType)
            {
                return ValidInstructionTypeMap
                    .ToDictionary(x => x.Value, y => y.Key)
                    .GetValueOrDefault(instructionType, typeof(InstructionModel));
            }

            private static LiteralType GetTypeOf(GameFunction gameFunction)
            {
                return gameFunction switch
                {
                    GameFunction.GetTargetedCombatParticipant => LiteralType.CombatParticipant,
                    GameFunction.GetSelf => LiteralType.CombatParticipant,
                    GameFunction.GetRandom => LiteralType.CombatParticipant,
                    GameFunction.GetAllCombatParticipants => LiteralType.List,
                    GameFunction.GetStat => LiteralType.Long,
                    GameFunction.GetBuff => LiteralType.Long,
                    _ => LiteralType.Null
                };
            }

            private class ValueField : VisualElement
            {
                public ValueModel Value { get; private set; }
                public event Action OnValueChanged;

                /// <summary>
                /// If true this allows the <see cref="Value"/> to have its type changed.
                /// This is false for lists, as all values in a list have the same type.
                /// </summary>
                private readonly bool allowTypeChange;

                public ValueField(ValueModel value, bool allowTypeChange)
                {
                    this.allowTypeChange = allowTypeChange;
                    RefreshView(value);
                }

                public void RefreshView(ValueModel value)
                {
                    Value = value;
                    Clear();

                    var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };
                    Add(row);
                    var valueTextField = new TextField("Value")
                    {
                        value = Value?.ToString() ?? "<null>",
                        isReadOnly = true
                    };
                    row.Add(valueTextField);
                    var editButton = new ButtonIcon(() => ManualValueFieldEditor.Open(Value, allowTypeChange, newValue =>
                    {
                        Value = newValue;
                        OnValueChanged?.Invoke();
                        valueTextField.value = Value.ToString();
                    }), IconPaths.Edit);
                    row.Add(editButton);
                }
            }

            private class ManualValueFieldEditor : UnityEditor.EditorWindow
            {
                private bool isInitialized;

                /// <summary>
                /// The original value before any edits have occured
                /// </summary>
                private ValueModel originalValue;

                /// <summary>
                /// The value that edits are being applied to
                /// </summary>
                private ValueModel value;

                private Action<ValueModel> onValueChanged;

                private bool allowTypeChange;

                public static void Open(ValueModel value, bool allowTypeChange, Action<ValueModel> onValueChanged)
                {
                    var window = GetWindow<ManualValueFieldEditor>();
                    window.value = value;
                    window.allowTypeChange = allowTypeChange;
                    window.onValueChanged = onValueChanged;
                    window.isInitialized = true;

                    window.CreateGUI();
                    window.ShowModalUtility();
                    window.Focus();
                }

                public void CreateGUI()
                {
                    if (!isInitialized)
                    {
                        return;
                    }

                    rootVisualElement.Clear();

                    value ??= new ValueModel { Type = LiteralType.Null };
                    originalValue = value.Clone();

                    var manualValueField = new ManualValueField(value);
                    manualValueField.OnValueChanged += () => { hasUnsavedChanges = true; };

                    var valueTypeField = new EnumField(value.Type);
                    valueTypeField.RegisterValueChangedCallback(evt =>
                    {
                        var newType = (LiteralType)evt.newValue;
                        value.Type = newType;
                        if (value.Source == Source.Manual)
                        {
                            manualValueField.RefreshView(newType);
                        }

                        hasUnsavedChanges = true;
                    });

                    var gameFunctionField = new EnumField("Game Function", value.GameFunction);
                    gameFunctionField.RegisterValueChangedCallback(evt =>
                    {
                        value.GameFunction = (GameFunction)evt.newValue;
                        hasUnsavedChanges = true;
                    });

                    var varSourceField = new EnumField("Source", value.Source)
                    {
                        tooltip = "Where this value is retrieved from; game function values are determined at runtime while " +
                                  "manually assigned values are available immediately."
                    };
                    varSourceField.RegisterValueChangedCallback(evt =>
                    {
                        value.Source = (Source)evt.newValue;
                        SetViewStateBasedOnSource(valueTypeField, allowTypeChange, gameFunctionField, manualValueField, value.Source);
                        hasUnsavedChanges = true;
                    });

                    SetViewStateBasedOnSource(valueTypeField, allowTypeChange, gameFunctionField, manualValueField, value.Source);
                    rootVisualElement.Add(valueTypeField);
                    rootVisualElement.Add(manualValueField);
                    rootVisualElement.Add(varSourceField);
                    rootVisualElement.Add(gameFunctionField);
                    rootVisualElement.Add(new Button(() =>
                    {
                        SaveChanges();
                        Close();
                    })
                    {
                        text = "Save"
                    });
                }

                private static void SetViewStateBasedOnSource(
                    EnumField varTypeField,
                    bool allowTypeChange,
                    EnumField gameFunctionField,
                    ManualValueField manualValueField,
                    Source source)
                {
                    varTypeField.SetEnabled(source == Source.Manual && allowTypeChange);
                    manualValueField.SetEnabled(source == Source.Manual);
                    manualValueField.visible = source == Source.Manual;
                    gameFunctionField.SetEnabled(source == Source.GameFunction);
                    gameFunctionField.visible = source == Source.GameFunction;
                }

                public override void SaveChanges()
                {
                    // apply the edits to the original
                    originalValue = value;
                    onValueChanged?.Invoke(originalValue);
                    base.SaveChanges();
                }
            }

            private class ManualValueField : VisualElement
            {
                private readonly ValueModel value;
                public event Action OnValueChanged;

                public ManualValueField(ValueModel value)
                {
                    this.value = value;
                    RefreshView(this.value.Type);
                }

                // TODO: On save, set all other values to their default values
                public void RefreshView(LiteralType type)
                {
                    Clear();

                    switch (type)
                    {
                        case LiteralType.Null:
                        {
                            Add(new Label("Null Value"));
                            break;
                        }

                        case LiteralType.Bool:
                        {
                            var boolField = new Toggle("Value") { value = value.Bool };
                            boolField.RegisterValueChangedCallback(evt =>
                            {
                                value.Bool = evt.newValue;
                                OnValueChanged?.Invoke();
                            });
                            Add(boolField);
                            break;
                        }

                        case LiteralType.String:
                        {
                            var textField = new TextField("Value") { value = value.String };
                            textField.RegisterValueChangedCallback(evt =>
                            {
                                value.String = evt.newValue;
                                OnValueChanged?.Invoke();
                            });
                            Add(textField);
                            break;
                        }

                        case LiteralType.Int:
                        {
                            var intField = new IntegerField("Value") { value = (int)value.Long };
                            intField.RegisterValueChangedCallback(evt =>
                            {
                                value.Long = evt.newValue;
                                OnValueChanged?.Invoke();
                            });
                            Add(intField);
                            break;
                        }

                        case LiteralType.Long:
                        {
                            var longField = new LongField("Value") { value = value.Long };
                            longField.RegisterValueChangedCallback(evt =>
                            {
                                value.Long = evt.newValue;
                                OnValueChanged?.Invoke();
                            });
                            Add(longField);
                            break;
                        }

                        case LiteralType.Float:
                        {
                            var floatField = new FloatField("Value") { value = (float)value.Double };
                            floatField.RegisterValueChangedCallback(evt =>
                            {
                                value.Double = evt.newValue;
                                OnValueChanged?.Invoke();
                            });
                            Add(floatField);
                            break;
                        }

                        case LiteralType.Double:
                        {
                            var doubleField = new DoubleField("Value") { value = value.Double };
                            doubleField.RegisterValueChangedCallback(evt =>
                            {
                                value.Double = evt.newValue;
                                OnValueChanged?.Invoke();
                            });
                            Add(doubleField);
                            break;
                        }

                        case LiteralType.List:
                        {
                            value.List ??= new ListValueModel();

                            var valueTypeField = new EnumField(value.Type);
                            var listField = new ListView(value.List)
                            {
                                makeItem = () => new ValueField((ValueModel)value.List[^1], false),
                                bindItem = (item, index) =>
                                {
                                    var valueField = (ValueField)item;
                                    var newValue = (ValueModel)value.List[index];
                                    newValue.Type = value.List.Type;
                                    valueField.RefreshView(newValue);
                                },
                                unbindItem = (item, _) =>
                                {
                                    var valueField = (ValueField)item;
                                    valueField.RefreshView(null);
                                },
                                showAddRemoveFooter = true,
                                showFoldoutHeader = true,
                                showBoundCollectionSize = true,
                                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
                                headerTitle = "List"
                            };
                            listField.itemsAdded += indices =>
                            {
                                foreach (var index in indices)
                                {
                                    value.List[index] = new ValueModel { Type = value.List.Type };
                                }

                                OnValueChanged?.Invoke();
                            };
                            listField.itemsRemoved += _ => OnValueChanged?.Invoke();
                            listField.itemsSourceChanged += () => OnValueChanged?.Invoke();
                            listField.itemIndexChanged += (_, _) => OnValueChanged?.Invoke();

                            valueTypeField.RegisterValueChangedCallback(evt =>
                            {
                                value.List.Type = (LiteralType)evt.newValue;
                                listField.Rebuild();
                                OnValueChanged?.Invoke();
                            });

                            Add(valueTypeField);
                            Add(listField);
                            break;
                        }

                        case LiteralType.CombatParticipant:
                        {
                            Add(new Label("TODO: What do we want to display for a CombatParticipant"));
                            break;
                        }

                        default:
                        {
                            Add(new Label("Invalid type"));
                            break;
                        }
                    }
                }
            }
        }
    }
}