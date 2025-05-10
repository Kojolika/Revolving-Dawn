using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Tooling.Logging;
using Tooling.StaticData.Bytecode;
using UnityEngine.UIElements;
using Utils.Extensions;
using Double = Tooling.StaticData.Bytecode.Double;
using Type = System.Type;
using LiteralType = Tooling.StaticData.Bytecode.Type;
using String = Tooling.StaticData.Bytecode.String;

namespace Tooling.StaticData.EditorUI
{
    [UsedImplicitly]
    public class StatementStaticDataDrawer : CustomStaticDataDrawer<Statement>
    {
        protected override VisualElement Draw(Statement data)
        {
            var root = new VisualElement();

            var nameField = new GeneralField<string>(new FieldValueProvider(typeof(Statement).GetField(nameof(Statement.Name)), data));
            nameField.OnValueChanged += _ => InvokeValueChanged();
            root.Add(nameField);

            var inputsField = new GeneralField<List<Variable>>(
                new FieldValueProvider(typeof(Statement).GetField(nameof(Statement.Inputs)), data)
            );
            inputsField.OnValueChanged += _ => InvokeValueChanged();
            root.Add(inputsField);

            data.Instructions ??= new List<IInstructionModel>();
            var listView = new ListView
            {
                itemsSource = data.Instructions,
                makeItem = () => new InstructionView(data),
                bindItem = (item, index) =>
                {
                    var instructionView = (InstructionView)item;
                    instructionView.RefreshView(index,
                        InstructionView.GetInstructionTypeOf(data.Instructions[index]?.GetType() ?? typeof(IInstructionModel)));
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
                RefreshView(index, GetInstructionTypeOf(statement.Instructions[index]?.GetType() ?? typeof(IInstructionModel)));
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
                    statement.Instructions[index] = Activator.CreateInstance(GetTypeOf(newInstruction)) as IInstructionModel;
                    RefreshView(index, newInstruction);
                    OnValueChanged?.Invoke();
                });
                Add(instructionSelection);

                switch (instructionType)
                {
                    case InstructionType.Unknown:
                    {
                        Add(new Label("Unknown Instruction"));
                        break;
                    }

                    case InstructionType.ReadVariable:
                    {
                        var definedVariables = new List<Variable>();
                        for (int i = 0; i < index; i++)
                        {
                        }

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
                        var valueField = new ValueField(assignVariable.Value);
                        valueField.OnValueChanged += OnValueChanged;
                        Add(valueField);
                        break;
                    }
                    default:
                        MyLogger.LogError("Unknown instruction type!");
                        break;
                }
            }

            private static void SetViewStateBasedOnSource(
                ValueTypeField varTypeField,
                EnumField gameFunctionField,
                ManualValueField manualValueField,
                Source source)
            {
                varTypeField.SetEnabled(source == Source.Manual);
                manualValueField.SetEnabled(source == Source.Manual);
                manualValueField.visible = source == Source.Manual;
                gameFunctionField.SetEnabled(source == Source.GameFunction);
                gameFunctionField.visible = source == Source.GameFunction;
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
                    .GetValueOrDefault(instructionType, typeof(IInstructionModel));
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

                public ValueField(ValueModel value)
                {
                    RefreshView(value);
                }

                public void RefreshView(ValueModel value)
                {
                    Value = value;
                    Clear();

                    if (value == null)
                    {
                        Add(new Label("Null value!"));
                        return;
                    }

                    var manualValueField = new ManualValueField(value, value.Type);
                    manualValueField.OnValueChanged += OnValueChanged;

                    var valueTypeField = new ValueTypeField(value);
                    valueTypeField.RegisterCallback<ChangeEvent<Type>>(evt =>
                    {
                        var newType = evt.newValue;
                        value.Type = newType;
                        if (value.Source == Source.Manual)
                        {
                            manualValueField.RefreshView(newType);
                        }

                        OnValueChanged?.Invoke();
                    });

                    var gameFunctionField = new EnumField("Game Function", value.GameFunction);
                    gameFunctionField.RegisterValueChangedCallback(evt =>
                    {
                        value.GameFunction = (GameFunction)evt.newValue;
                        OnValueChanged?.Invoke();
                    });

                    var varSourceField = new EnumField("Source", value.Source)
                    {
                        tooltip = "Where this value is retrieved from; game function values are determined at runtime while " +
                                  "manually assigned values are available immediately."
                    };
                    varSourceField.RegisterValueChangedCallback(evt =>
                    {
                        value.Source = (Source)evt.newValue;
                        SetViewStateBasedOnSource(valueTypeField, gameFunctionField, manualValueField, value.Source);
                        OnValueChanged?.Invoke();
                    });

                    SetViewStateBasedOnSource(valueTypeField, gameFunctionField, manualValueField, value.Source);
                    Add(valueTypeField);
                    Add(manualValueField);
                    Add(varSourceField);
                    Add(gameFunctionField);
                }
            }

            private class ManualValueField : VisualElement
            {
                private readonly ValueModel value;
                public event Action OnValueChanged;

                public ManualValueField(ValueModel value, Type type)
                {
                    this.value = value;
                    RefreshView(type);
                }

                private static readonly Dictionary<Type, Action<ManualValueField>> RefreshViewAction = new()
                {
                    {
                        typeof(Null), field => { field.Add(new Label("Null Value")); }
                    },
                    {
                        typeof(Bool), field =>
                        {
                            var boolField = new Toggle("Value") { value = field.value.Bool };
                            boolField.RegisterValueChangedCallback(evt =>
                            {
                                field.value.Bool = evt.newValue;
                                field.OnValueChanged?.Invoke();
                            });
                            field.Add(boolField);
                        }
                    },
                    {
                        typeof(String), field =>
                        {
                            var textField = new TextField("Value") { value = field.value.String };
                            textField.RegisterValueChangedCallback(evt =>
                            {
                                field.value.String = evt.newValue;
                                field.OnValueChanged?.Invoke();
                            });
                            field.Add(textField);
                        }
                    },
                    {
                        typeof(Int), field =>
                        {
                            var intField = new IntegerField("Value") { value = (int)field.value.Long };
                            intField.RegisterValueChangedCallback(evt =>
                            {
                                field.value.Long = evt.newValue;
                                field.OnValueChanged?.Invoke();
                            });
                            field.Add(intField);
                        }
                    },
                    {
                        typeof(Long), field =>
                        {
                            var longField = new LongField("Value") { value = field.value.Long };
                            longField.RegisterValueChangedCallback(evt =>
                            {
                                field.value.Long = evt.newValue;
                                field.OnValueChanged?.Invoke();
                            });
                            field.Add(longField);
                        }
                    },
                    {
                        typeof(Float), field =>
                        {
                            var floatField = new FloatField("Value") { value = (float)field.value.Double };
                            floatField.RegisterValueChangedCallback(evt =>
                            {
                                field.value.Double = evt.newValue;
                                field.OnValueChanged?.Invoke();
                            });
                            field.Add(floatField);
                        }
                    },
                    {
                        typeof(Double), field =>
                        {
                            var doubleField = new DoubleField("Value") { value = field.value.Double };
                            doubleField.RegisterValueChangedCallback(evt =>
                            {
                                field.value.Double = evt.newValue;
                                field.OnValueChanged?.Invoke();
                            });
                            field.Add(doubleField);
                        }
                    },
                    {
                        typeof(ValueList<>), field =>
                        {
                            field.value.List ??= new ListValueModel
                            {
                                Type = typeof(Null)
                            };

                            var valueTypeField = new ValueTypeField(field.value);
                            var listField = new ListView(field.value.List)
                            {
                                makeItem = () => new ValueField(null),
                                bindItem = (item, index) =>
                                {
                                    var valueField = (ValueField)item;
                                    valueField.RefreshView((ValueModel)field.value.List[index]);
                                    valueField.OnValueChanged += field.OnValueChanged;
                                },
                                unbindItem = (item, _) =>
                                {
                                    var valueField = (ValueField)item;
                                    valueField.RefreshView(null);
                                    valueField.OnValueChanged -= field.OnValueChanged;
                                },
                                showAddRemoveFooter = true,
                                showFoldoutHeader = true,
                                showBoundCollectionSize = true,
                                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
                                headerTitle = "List"
                            };
                            listField.itemsAdded += _ => field.OnValueChanged?.Invoke();
                            listField.itemsRemoved += _ => field.OnValueChanged?.Invoke();
                            listField.itemsSourceChanged += () => field.OnValueChanged?.Invoke();
                            listField.itemIndexChanged += (_, _) => field.OnValueChanged?.Invoke();

                            valueTypeField.RegisterValueChangedCallback(evt =>
                            {
                                var newType = evt.newValue;
                                field.value.List.Type = newType;
                                foreach (ValueModel value in field.value.List)
                                {
                                    value.Type = newType;
                                }

                                listField.Rebuild();
                                field.OnValueChanged?.Invoke();
                            });

                            field.Add(valueTypeField);
                            field.Add(listField);
                        }
                    }
                };

                // TODO: On save, set all other values to their default values
                public void RefreshView(Type type)
                {
                    Clear();

                    if (type == null)
                    {
                        type = typeof(Null);
                        value.Type = type;
                    }

                    if (RefreshViewAction.TryGetValue(type, out var refreshViewAction))
                    {
                        refreshViewAction.Invoke(this);
                    }
                    else
                    {
                        MyLogger.LogError($"Invalid type passed in, expected a class that inherits {typeof(IValueType)}, but got {type}!");
                    }
                }
            }

            private class ValueTypeField : VisualElement, INotifyValueChanged<Type>
            {
                public Type value { get; set; }

                public ValueTypeField(ValueModel value)
                {
                    RefreshView(value);
                }

                public void RefreshView(ValueModel value)
                {
                    this.value = value.Type;
                    Clear();

                    var types = Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => typeof(IValueType).IsAssignableFrom(t)
                                    && t.IsValueType
                                    && !t.IsAbstract
                                    && !t.IsInterface)
                        .ToList();
                    var popupTypeField = new PopupField<Type>(
                        "Type",
                        types,
                        value.Type,
                        t => t.IsGenericType // Only ValueList is generic, we want to display that name without the `
                            ? t.Name.Split('`')[0]
                            : t.Name,
                        t => t.IsGenericType
                            ? t.Name.Split('`')[0]
                            : t.Name);
                    popupTypeField.RegisterValueChangedCallback(SendEvent);
                    Add(popupTypeField);
                }

                public void SetValueWithoutNotify(Type newValue)
                {
                    value = newValue;
                }
            }
        }
    }
}