using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Tooling.Logging;
using Tooling.StaticData.Bytecode;
using UnityEngine.UIElements;
using Utils.Extensions;
using Type = System.Type;
using ByteValueType = Tooling.StaticData.Bytecode.Type;

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
                    instructionView.RefreshView(index, statement.Instructions[index]);
                    instructionView.OnValueChanged += InvokeValueChanged;
                },
                unbindItem = (item, _) => ((InstructionView)item).OnValueChanged -= InvokeValueChanged,
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

        /// <summary>
        /// The view for a variable input for a statement.
        /// </summary>
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

                var availableTypes = new List<string>();
                var enumTypes = Enum.GetValues(typeof(ByteValueType))
                    .Cast<ByteValueType>()
                    .Select(type => type.ToString())
                    .ToList();
                availableTypes.AddRange(enumTypes);

                var objectTypes = Assembly.GetCallingAssembly()
                    .GetTypes()
                    .Where(type => type.GetCustomAttribute<ByteObject>() != null)
                    .Select(type => type.FullName)
                    .ToList();
                availableTypes.AddRange(objectTypes);

                Func<string, string> popupFieldFormatting = typeString =>
                {
                    if (enumTypes.Contains(typeString))
                    {
                        return typeString;
                    }

                    if (objectTypes.Contains(typeString))
                    {
                        string typeWithoutNameSpace = typeString.Split('.').Last();
                        return $"Object/{typeWithoutNameSpace}";
                    }

                    return typeString;
                };

                var typeSelection = new PopupField<string>(
                    "Variable Type",
                    availableTypes,
                    value.Type.ToString(),
                    popupFieldFormatting,
                    popupFieldFormatting)
                {
                    value = value.Type != ByteValueType.Object || value.ObjectType == null
                        ? value.Type.ToString()
                        : popupFieldFormatting(value.ObjectType.FullName)
                };

                var objectTypeContainer = new VisualElement();
                RefreshVariableTypeView(
                    value,
                    objectTypeContainer,
                    value.Type != ByteValueType.Object || value.ObjectType == null
                        ? value.Type.ToString()
                        : value.ObjectType.FullName,
                    enumTypes,
                    objectTypes);

                typeSelection.RegisterValueChangedCallback(evt => RefreshVariableTypeView(value, objectTypeContainer, evt.newValue, enumTypes, objectTypes));

                Add(typeSelection);
                Add(objectTypeContainer);
            }

            private static void RefreshVariableTypeView(
                Variable variable,
                VisualElement objectTypeContainer,
                string selectedType,
                List<string> enumTypes,
                List<string> objectTypes)
            {
                objectTypeContainer.Clear();

                if (enumTypes.Contains(selectedType))
                {
                    variable.Type = (ByteValueType)Enum.Parse(typeof(ByteValueType), selectedType);
                }
                else if (objectTypes.Contains(selectedType))
                {
                    var newType = Type.GetType(selectedType);
                    if (newType == null)
                    {
                        variable.Type = default;
                        MyLogger.LogError("Error: Variable object type could not be found!");

                        return;
                    }

                    variable.Type = ByteValueType.Object;
                    variable.ObjectType = newType;
                    var objectTypeTextField = new TextField("Object Type")
                    {
                        value = selectedType
                    };
                    objectTypeTextField.SetEnabled(false);
                    objectTypeContainer.Add(objectTypeTextField);
                }
            }
        }

        private class InstructionView : VisualElement
        {
            public event Action OnValueChanged;
            private readonly Statement statement;

            public InstructionView(Statement statement)
            {
                this.statement = statement;
                int index = this.statement.Instructions.Count - 1;
                RefreshView(index, statement.Instructions[index]);
            }

            public void RefreshView(int index, InstructionModel instruction)
            {
                Clear();

                if (statement.Instructions.IsNullOrEmpty())
                {
                    return;
                }

                var instructionSelection = new TypeSelect<InstructionModel>("Instruction Type", statement.Instructions[index]);
                instructionSelection.RegisterValueChangedCallback(evt =>
                {
                    statement.Instructions[index] = evt.newValue;
                    RefreshView(index, evt.newValue);
                    OnValueChanged?.Invoke();
                });
                Add(instructionSelection);

                switch (instruction)
                {
                    case null:
                    case Unknown:
                    {
                        Add(new Label("Select an instruction"));
                        break;
                    }

                    case ReadVariableModel readVariableModel:
                    {
                        // Provides a default selection that we won't save if no variable is selected
                        var nullVariable = new Variable { Name = "(none)", Type = ByteValueType.Null };
                        var definedVariables = new List<Variable> { nullVariable };

                        if (!statement.Inputs.IsNullOrEmpty())
                        {
                            foreach (var input in statement.Inputs)
                            {
                                definedVariables.Add(input);
                            }
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

                        if (definedVariables.Count > 0)
                        {
                            for (int i = 0; i < definedVariables.Count; i++)
                            {
                                var variable = definedVariables[i];
                                string variableNamePrefix =
                                    $"{variable.Name} ({(variable.Type == ByteValueType.Object && variable.ObjectType != null ? variable.ObjectType.Name : variable.Name)}) ";
                                if (variable.Type == ByteValueType.Object && variable.ObjectType != null)
                                {
                                    var fieldVariables = variable.ObjectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                        .Where(p => p.GetCustomAttribute<ByteProperty>() != null)
                                        .Select(p => new Variable
                                        {
                                            Name = $"{variableNamePrefix}/ {p.Name}",
                                            Type = p.GetCustomAttribute<ByteProperty>().Type
                                        });

                                    var propertyVariables = variable.ObjectType
                                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                        .Where(p => p.GetCustomAttribute<ByteProperty>() != null)
                                        .Select(p => new Variable
                                        {
                                            Name = $"{variableNamePrefix}/ {p.Name}",
                                            Type = p.GetCustomAttribute<ByteProperty>().Type
                                        });

                                    var methodVariables = variable.ObjectType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                        .Where(m => m.GetCustomAttribute<ByteFunction>() != null)
                                        .Select(m => new Variable
                                        {
                                            Name = $"{variableNamePrefix}/ {m.Name}",
                                            Type = m.GetCustomAttribute<ByteFunction>().Output
                                        });

                                    definedVariables.AddRange(propertyVariables);
                                    definedVariables.AddRange(fieldVariables);
                                    definedVariables.AddRange(methodVariables);
                                }
                            }

                            definedVariables = definedVariables.OrderBy(x => x.Name).ToList();
                        }

                        var variableSelection = new PopupField<Variable>("Select Variable",
                            definedVariables,
                            definedVariables.FirstOrDefault(),
                            variable => $"{variable.Name} ({variable.Type})",
                            variable => $"{variable.Name} ({variable.Type})");
                        variableSelection.RegisterValueChangedCallback(evt =>
                        {
                            // Set the selected variable, if it's our designated null variable, we set the value to null
                            readVariableModel.Name = evt.newValue == nullVariable
                                ? null
                                : evt.newValue.Name;
                        });
                        Add(variableSelection);

                        break;
                    }

                    case AssignVariableModel assignVariable:
                    {
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
                        Add(new Label($"Instruction has no UI. type={instruction.GetType()}"));
                        break;
                }
            }

            private class ValueField : VisualElement, INotifyValueChanged<ValueModel>
            {
                /// <summary>
                /// The original value before any edits have occured
                /// </summary>
                private ValueModel originalValue;

                public ValueModel value { get; set; }

                /// <summary>
                /// If true this allows the <see cref="value"/> to have its type changed.
                /// This is false for lists, as all values in a list have the same type.
                /// </summary>
                private readonly bool allowTypeChange;

                private readonly bool allowEditing;

                public ValueField(ValueModel value, bool allowTypeChange = true, bool allowEditing = true)
                {
                    this.allowTypeChange = allowTypeChange;
                    this.allowEditing = allowEditing;

                    originalValue = value ??= new ValueModel();
                    this.value = originalValue.Clone();

                    AddToClassList(Styles.Container);
                    AddToClassList(Styles.Border);

                    RefreshView(value);
                }

                public void SetValueWithoutNotify(ValueModel newValue)
                {
                    originalValue = newValue;
                }

                public void RefreshView(ValueModel value)
                {
                    this.value = value;
                    Clear();

                    var foldout = new Foldout
                    {
                        value = false,
                        text = GetFoldoutText(this.value)
                    };

                    var manualValueField = new ManualValueField(value);
                    manualValueField.RegisterValueChangedCallback(evt =>
                    {
                        value = evt.newValue;
                        foldout.text = GetFoldoutText(this.value);
                    });

                    var valueTypeField = new EnumField(value.Type)
                    {
                        label = "Type"
                    };
                    valueTypeField.RegisterValueChangedCallback(evt =>
                    {
                        var newType = (ByteValueType)evt.newValue;
                        value.Type = newType;
                        if (value.Source == Source.Manual)
                        {
                            manualValueField.RefreshView(newType);
                        }

                        foldout.text = GetFoldoutText(this.value);
                    });

                    var gameFunctionField = new EnumField("Game Function", value.GameFunction);
                    gameFunctionField.RegisterValueChangedCallback(evt =>
                    {
                        value.GameFunction = (GameFunction)evt.newValue;
                        foldout.text = GetFoldoutText(this.value);
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
                        foldout.text = GetFoldoutText(this.value);
                    });

                    SetViewStateBasedOnSource(valueTypeField, allowTypeChange, gameFunctionField, manualValueField, value.Source);

                    Add(foldout);
                    foldout.Add(valueTypeField);
                    foldout.Add(manualValueField);
                    foldout.Add(varSourceField);
                    foldout.Add(gameFunctionField);
                    foldout.Add(new Button(() =>
                    {
                        // apply the edits to the original
                        var changeEvent = ChangeEvent<ValueModel>.GetPooled(originalValue, value);
                        changeEvent.target = this;
                        originalValue = value;
                        SendEvent(changeEvent);
                    })
                    {
                        text = "Save"
                    });
                }

                /// <summary>
                /// Formats as 'value : type' or 'Null' if the value is null
                /// </summary>
                private static string GetFoldoutText(ValueModel value)
                {
                    return $"{value?.ToString() ?? "Null"}{(value != null ? $" : {value.Type.ToString()}" : string.Empty)}";
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
                public void RefreshView(ByteValueType type)
                {
                    Clear();

                    var valueTextField = new TextField("Value");
                    valueTextField.RegisterValueChangedCallback(evt =>
                    {
                        value.String = evt.newValue;

                        cts?.Cancel();
                        cts?.Dispose();
                        cts = new CancellationTokenSource();
                        _ = UniTask.Delay(TimeSpan.FromSeconds(1), true, cancellationToken: cts.Token).ContinueWith(() =>
                        {
                            if (cts.IsCancellationRequested)
                            {
                                return;
                            }
                        });
                    });


                    switch (type)
                    {
                        case ByteValueType.Null:
                        {
                            var nullValueField = new TextField("Value") { value = "null" };
                            nullValueField.SetEnabled(false);
                            Add(nullValueField);
                            break;
                        }

                        case ByteValueType.Bool:
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
                                errorMessageContainer.AddToClassList(Styles.Column);
                                var errorReport = new ExpressionErrorReport(validationLabel, expressionField, errorMessageContainer);
                                var variablesContainer = new ListView
                                {
                                    itemsSource = value.BooleanModel.ExpressionValues,
                                    showFoldoutHeader = false,
                                    showAddRemoveFooter = false,
                                    showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
                                    reorderable = true,
                                    makeItem = () => new ValueField(null),
                                    bindItem = (item, index) =>
                                    {
                                        var valueField = (ValueField)item;
                                        valueField.RefreshView(value.BooleanModel.ExpressionValues[index]);
                                        valueField.userData =
                                            new EventCallback<ChangeEvent<ValueModel>>(evt => value.BooleanModel.ExpressionValues[index] = evt.newValue);
                                        valueField.RegisterValueChangedCallback(evt => value.BooleanModel.ExpressionValues[index] = evt.newValue);
                                    },
                                    unbindItem = (item, _) =>
                                    {
                                        var valueField = (ValueField)item;
                                        valueField.UnregisterValueChangedCallback(valueField.userData as EventCallback<ChangeEvent<ValueModel>>);
                                    },
                                    virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
                                };

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

                                    MyLogger.Log("Scanning...");
                                    Scanner.Scan(value.BooleanModel.Expression, out var tokens, errorReport);

                                    int highestVar = 0;
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
                                            errorReport.Report("Variable numbers must be sequential, i.e start with {0} then {1}", token.Start, token.Length);
                                            continue;
                                        }

                                        MyLogger.Log($"# expr values: {value.BooleanModel.ExpressionValues?.Count}, varNumber: {varNumber}");

                                        /*
                                        if ((value.BooleanModel.ExpressionValues?.Count ?? 0) - 1 < varNumber)
                                        {
                                            var newVariable = new ValueModel();
                                            value.BooleanModel.ExpressionValues ??= new List<ValueModel>();
                                            value.BooleanModel.ExpressionValues.Add(newVariable);
                                            variablesContainer.Rebuild();
                                        }
                                        */

                                        highestVar++;
                                    }
                                }
                            }
                        }

                        case ByteValueType.String:
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

                        case ByteValueType.Int:
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

                        case ByteValueType.Long:
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

                        case ByteValueType.Float:
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

                        case ByteValueType.Double:
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

                        case ByteValueType.List:
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
                                value.List.Type = (ByteValueType)evt.newValue;
                                listField.Rebuild();
                                SendEvent(ChangeEvent<ValueModel>.GetPooled(oldValue, value));
                            });

                            Add(valueTypeField);
                            Add(listField);
                            break;
                        }

                        case ByteValueType.Object:
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