using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
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
                        MyLogger.LogError($"Error: statement input count is: {statement.Inputs?.Count} but index is: {index}!");
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
                horizontalScrollingEnabled = true,
                tooltip = "Options inputs for this statement. This is only used when you reference this statement in another statement. " +
                          "For example, if we created a statement that was for a DealDamage effect, we would want the inputs of the amount " +
                          "of damage, and the target of the affect. Then we could reuse the DealDamage effect in multiple other statements."
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
                        // Provides a default selection that we won't save if no variable is selected
                        var nullVariable = new Variable { Name = "(none)", Type = LiteralType.Null };
                        var definedVariables = new List<Variable> { nullVariable };
                        foreach (var input in statement.Inputs)
                        {
                            definedVariables.Add(input);
                        }

                        for (int i = 0; i < index; i++)
                        {
                            if (statement.Instructions[i] is AssignVariableModel assignVariableModel && string.IsNullOrEmpty(assignVariableModel.Name))
                            {
                                definedVariables.Add(new Variable
                                {
                                    Name = assignVariableModel.Name,
                                    Type = assignVariableModel.Value.Type
                                });
                            }
                        }

                        /*// TODO: Refactor if we have more cases like this?
                        // we want to be able to reference the stats/buffs of combat participants in the UI
                        // This seems like more a quick way to do it for the UI but this relationship between
                        // the LiteralType.CombatParticipant and stats/buffs isn't defined anywhere in the code
                        if (definedVariables.Count > 0)
                        {
                            for (int i = 0; i < definedVariables.Count; i++)
                            {
                                var variable = definedVariables[i];
                                string variableNamePrefix = $"{variable.Name} ({variable.Type})";
                                if (variable.Type == LiteralType.CombatParticipant)
                                {
                                    foreach (var stat in StaticDatabase.Instance.GetInstancesForType<Stat>())
                                    {
                                        // TODO: do we want to use double for stats?
                                        // We're assuming here that stats are only floats
                                        definedVariables.Add(new Variable { Name = $"{variableNamePrefix}/Stats/{stat.Name}", Type = LiteralType.Float });
                                    }

                                    foreach (var buff in StaticDatabase.Instance.GetInstancesForType<Buff>())
                                    {
                                        // TODO: do we want to use long for buffs?
                                        // We're assuming buffs are only ints and that getting a buff returns the number of buffs on the combat participant
                                        definedVariables.Add(new Variable { Name = $"{variableNamePrefix}/Buffs/{buff.Name}", Type = LiteralType.Int });
                                    }
                                }
                            }
                        }*/

                        definedVariables = definedVariables.OrderBy(x => x.Name).ToList();

                        var variableSelection = new PopupField<Variable>("Select Variable",
                            definedVariables,
                            definedVariables.FirstOrDefault(),
                            variable => $"{variable.Name} ({variable.Type})",
                            variable => $"{variable.Name} ({variable.Type})");
                        variableSelection.RegisterValueChangedCallback(evt =>
                        {
                            if (statement.Instructions[index] is ReadVariableModel readVariableModel)
                            {
                                // Set the selected variable, if it's our designated null variable, we set the value to null
                                readVariableModel.Name = evt.newValue == nullVariable
                                    ? null
                                    : evt.newValue.Name;
                            }
                            else
                            {
                                MyLogger.LogError($"Expected instruction {index} to be {typeof(ReadVariableModel)}!");
                            }
                        });
                        Add(variableSelection);

                        break;
                    }

                    case InstructionType.AssignVariable:
                    {
                        if (statement.Instructions[index] is not AssignVariableModel assignVariable)
                        {
                            MyLogger.LogError($"Error: statement instruction at index {index}. " +
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
                        valueField.RegisterValueChangedCallback(evt =>
                        {
                            assignVariable.Value = evt.newValue;
                            OnValueChanged?.Invoke();
                        });
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

            private class ValueField : VisualElement, INotifyValueChanged<ValueModel>
            {
                public ValueModel value { get; set; }

                /// <summary>
                /// If true this allows the <see cref="value"/> to have its type changed.
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
                    this.value = value;
                    Clear();

                    var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };
                    Add(row);
                    var valueTextField = new TextField("Value")
                    {
                        value = this.value?.ToString() ?? "<null>",
                        isReadOnly = true
                    };

                    row.Add(valueTextField);
                    var editButton = new ButtonIcon(() =>
                        ManualValueFieldEditor.Open(
                            this.value,
                            allowTypeChange,
                            newValue =>
                            {
                                var oldValue = this.value;
                                this.value = newValue;
                                valueTextField.value = this.value.ToString();

                                var changeEvent = ChangeEvent<ValueModel>.GetPooled(oldValue, this.value);
                                // Without setting the target as this element, the target is null and
                                // the InstructionView that registered to this value changed callback does not receive events
                                changeEvent.target = this;

                                SendEvent(changeEvent);
                            }), IconPaths.Edit);
                    row.Add(editButton);
                }

                public void SetValueWithoutNotify(ValueModel newValue)
                {
                    value = newValue;
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

                private event Action<ValueModel> onValueChanged;

                private bool allowTypeChange;

                public static void Open(ValueModel value, bool allowTypeChange, Action<ValueModel> onValueChanged)
                {
                    var window = GetWindow<ManualValueFieldEditor>();
                    window.value = value;
                    window.allowTypeChange = allowTypeChange;
                    window.onValueChanged = onValueChanged;
                    window.isInitialized = true;

                    window.CreateGUI();
                    window.Show();
                    window.Focus();
                }

                public void CreateGUI()
                {
                    if (!isInitialized)
                    {
                        return;
                    }

                    rootVisualElement.Clear();

                    value ??= new ValueModel();
                    originalValue = value.Clone();

                    var manualValueField = new ManualValueField(value);
                    manualValueField.RegisterValueChangedCallback(evt =>
                    {
                        value = evt.newValue;
                        hasUnsavedChanges = true;
                    });

                    var valueTypeField = new EnumField(value.Type)
                    {
                        label = "Type"
                    };
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

            private class ManualValueField : VisualElement, INotifyValueChanged<ValueModel>
            {
                public ValueModel value { get; set; }

                private CancellationTokenSource cts;

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
                            var nullValueField = new TextField("Value") { value = "null" };
                            nullValueField.SetEnabled(false);
                            Add(nullValueField);
                            break;
                        }

                        case LiteralType.Bool:
                        {
                            var boolField = new Toggle("Value")
                            {
                                value = value.BooleanModel.Value
                            };
                            boolField.RegisterValueChangedCallback(evt =>
                            {
                                var oldValue = value.Clone();
                                value.BooleanModel.Value = evt.newValue;
                                SendEvent(ChangeEvent<ValueModel>.GetPooled(oldValue, value));
                            });

                            var expressionContainer = new VisualElement();
                            var useExpressionToggle = new Toggle("Use Expression")
                            {
                                value = value.BooleanModel.UseExpression
                            };
                            useExpressionToggle.RegisterValueChangedCallback(evt => RefreshExpressionView(evt.newValue));
                            RefreshExpressionView(value.BooleanModel.UseExpression);

                            Add(boolField);
                            Add(useExpressionToggle);
                            Add(expressionContainer);

                            break;

                            void RefreshExpressionView(bool useExpression)
                            {
                                value.BooleanModel.UseExpression = useExpression;
                                boolField.SetEnabled(!value.BooleanModel.UseExpression);
                                expressionContainer.Clear();
                                if (!useExpression)
                                {
                                    return;
                                }

                                var validationLabel = new Label();
                                var expressionField = new TextField("Expression")
                                {
                                    value = value.BooleanModel.Expression
                                };

                                var errorMessageContainer = new VisualElement();
                                errorMessageContainer.AddToClassList(VisualElementClasses.Column);
                                var errorReport = new ExpressionErrorReport(validationLabel, expressionField, errorMessageContainer);
                                var variablesContainer = new VisualElement();

                                expressionContainer.Add(validationLabel);
                                expressionContainer.Add(expressionField);
                                expressionContainer.Add(errorMessageContainer);
                                expressionContainer.Add(variablesContainer);
                                ProcessExpression();

                                expressionField.RegisterValueChangedCallback(evt =>
                                {
                                    value.BooleanModel.Expression = evt.newValue;

                                    cts?.Cancel();
                                    cts?.Dispose();
                                    cts = new CancellationTokenSource();
                                    _ = UniTask.Delay(TimeSpan.FromSeconds(1), true, cancellationToken: cts.Token).ContinueWith(() =>
                                    {
                                        if (cts.IsCancellationRequested)
                                        {
                                            return;
                                        }

                                        ProcessExpression();
                                    });
                                });

                                return;

                                void ProcessExpression()
                                {
                                    errorReport.Reset();
                                    variablesContainer.Clear();

                                    Scanner.Scan(value.BooleanModel.Expression, out var tokens, errorReport);

                                    int highestVar = 0;
                                    var expressionVariables = value.BooleanModel.ExpressionValues?
                                        .Select(var => var.Clone())
                                        .ToList() ?? new List<ValueModel>();

                                    foreach (var token in tokens)
                                    {
                                        if (token.TokenType != Token.Type.ByteVar)
                                        {
                                            continue;
                                        }

                                        if (!int.TryParse(token.Lexeme[1..^1], out var varNumber))
                                        {
                                            MyLogger.LogError($"Cannot parse a variable number from the expression. Lexeme: {token.Lexeme}");
                                            continue;
                                        }

                                        if (varNumber < 0)
                                        {
                                            errorReport.Report("Variable numbers cannot be negative.", token.Start, token.Length);
                                            continue;
                                        }

                                        if (varNumber > highestVar)
                                        {
                                            errorReport.Report("Variable numbers must be sequential, i.e start with {0} then {1}", token.Start,
                                                token.Length);
                                            continue;
                                        }

                                        if (expressionVariables.Count - 1 < varNumber)
                                        {
                                            var newVariable = new ValueModel();
                                            expressionVariables.Add(newVariable);
                                        }

                                        variablesContainer.Add(new ManualValueField(expressionVariables[varNumber]));
                                        highestVar++;
                                    }

                                    value.BooleanModel.ExpressionValues = expressionVariables;
                                }
                            }
                        }

                        case LiteralType.String:
                        {
                            var textField = new TextField("Value") { value = value.String };
                            textField.RegisterValueChangedCallback(evt =>
                            {
                                var oldValue = value.Clone();
                                value.String = evt.newValue;
                                SendEvent(ChangeEvent<ValueModel>.GetPooled(oldValue, value));
                            });
                            Add(textField);
                            break;
                        }

                        case LiteralType.Int:
                        {
                            var intField = new IntegerField("Value") { value = (int)value.Long };
                            intField.RegisterValueChangedCallback(evt =>
                            {
                                var oldValue = value.Clone();
                                value.Long = evt.newValue;
                                SendEvent(ChangeEvent<ValueModel>.GetPooled(oldValue, value));
                            });
                            Add(intField);
                            break;
                        }

                        case LiteralType.Long:
                        {
                            var longField = new LongField("Value") { value = value.Long };
                            longField.RegisterValueChangedCallback(evt =>
                            {
                                var oldValue = value.Clone();
                                value.Long = evt.newValue;
                                SendEvent(ChangeEvent<ValueModel>.GetPooled(oldValue, value));
                            });
                            Add(longField);
                            break;
                        }

                        case LiteralType.Float:
                        {
                            var floatField = new FloatField("Value") { value = (float)value.Double };
                            floatField.RegisterValueChangedCallback(evt =>
                            {
                                var oldValue = value.Clone();
                                value.Double = evt.newValue;
                                SendEvent(ChangeEvent<ValueModel>.GetPooled(oldValue, value));
                            });
                            Add(floatField);
                            break;
                        }

                        case LiteralType.Double:
                        {
                            var doubleField = new DoubleField("Value") { value = value.Double };
                            doubleField.RegisterValueChangedCallback(evt =>
                            {
                                var oldValue = value.Clone();
                                value.Double = evt.newValue;
                                SendEvent(ChangeEvent<ValueModel>.GetPooled(oldValue, value));
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
                                var oldValue = value.Clone();
                                foreach (var index in indices)
                                {
                                    value.List[index] = new ValueModel { Type = value.List.Type };
                                }


                                SendEvent(ChangeEvent<ValueModel>.GetPooled(oldValue, value));
                            };
                            listField.itemsRemoved += _ => SendEvent(ChangeEvent<ValueModel>.GetPooled());
                            listField.itemsSourceChanged += () => SendEvent(ChangeEvent<ValueModel>.GetPooled());
                            listField.itemIndexChanged += (_, _) => SendEvent(ChangeEvent<ValueModel>.GetPooled());

                            valueTypeField.RegisterValueChangedCallback(evt =>
                            {
                                var oldValue = value.Clone();
                                value.List.Type = (LiteralType)evt.newValue;
                                listField.Rebuild();
                                SendEvent(ChangeEvent<ValueModel>.GetPooled(oldValue, value));
                            });

                            Add(valueTypeField);
                            Add(listField);
                            break;
                        }

                        case LiteralType.Object:
                        {
                            Add(new Label("TODO: What do we want to display for an Object"));
                            break;
                        }

                        default:
                        {
                            Add(new Label("Invalid type"));
                            break;
                        }
                    }
                }

                public void SetValueWithoutNotify(ValueModel newValue)
                {
                    value = newValue;
                }

                private class ExpressionErrorReport : IErrorReport
                {
                    private readonly Label label;
                    private readonly TextField rawExpressionField;
                    private readonly VisualElement errorMessageContainer;

                    private const string ColorStart = "<color=red>";
                    private const string ColorEnd = "</color>";
                    private int lengthOfColorStart => ColorStart.Length;
                    private int lengthOfColorEnd => ColorEnd.Length;

                    /// <summary>
                    /// Stores of the offsets of where the source string's characters are located in the current validation label.
                    /// This is so we can accurately insert values in the right places of the source string while editing the validation label.
                    /// </summary>
                    private int[] offsetsFromSourceString;

                    public ExpressionErrorReport(Label label, TextField rawExpressionField, VisualElement errorMessageContainer)
                    {
                        this.label = label;
                        this.rawExpressionField = rawExpressionField;
                        this.errorMessageContainer = errorMessageContainer;
                    }

                    public void Report(string message, int columnNumber, int length)
                    {
                        // Shouldn't be possible with how the tokens are created but will guard against just in case
                        if (columnNumber == 0 && length == 0)
                        {
                            return;
                        }

                        // If this is the first error reported for the expression, initialize the state
                        if (offsetsFromSourceString == null)
                        {
                            offsetsFromSourceString = new int[rawExpressionField.text.Length];
                            label.text = rawExpressionField.text;
                        }

                        string validationString = label.text;
                        int startIndex = columnNumber + offsetsFromSourceString[columnNumber];
                        validationString = validationString.Insert(startIndex, ColorStart);
                        for (int i = columnNumber; i < offsetsFromSourceString.Length; i++)
                        {
                            offsetsFromSourceString[i] += lengthOfColorStart;
                        }

                        int lexemeEndIndex = columnNumber + length - 1;
                        int endIndex = lexemeEndIndex + offsetsFromSourceString[lexemeEndIndex] + 1;
                        validationString = validationString.Insert(endIndex, ColorEnd);
                        for (int i = lexemeEndIndex; i < offsetsFromSourceString.Length; i++)
                        {
                            offsetsFromSourceString[i] += lengthOfColorEnd;
                        }

                        label.text = validationString;

                        errorMessageContainer.Add(new Label($"<color=red>{message}</color>"));
                    }

                    /// <summary>
                    /// Resets the state of the current validation string
                    /// </summary>
                    public void Reset()
                    {
                        label.text = string.Empty;
                        offsetsFromSourceString = null;
                        errorMessageContainer.Clear();
                    }
                }
            }
        }
    }
}