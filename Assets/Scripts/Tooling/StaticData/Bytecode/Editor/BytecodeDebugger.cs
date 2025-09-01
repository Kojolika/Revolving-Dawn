using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Tooling.Logging;
using UnityEditor;
using UnityEngine.UIElements;
using ILogger = Tooling.Logging.ILogger;

namespace Tooling.StaticData.EditorUI.Bytecode.Editor
{
    public class BytecodeDebugger
    {
        [MenuItem("KoJy/Bytecode Debugger")]
        public static void OpenByteCodeTest()
        {
            UnityEditor.EditorWindow.GetWindow<EditorWindow>("Bytecode Debugger");
        }

        private class DebugBytecodeLogger : ILogger
        {
            private readonly Label label;

            public DebugBytecodeLogger(Label label)
            {
                this.label = label;
            }

            public void Log(LogLevel level, string message, string filePath = "", params object[] args)
            {
                switch (level)
                {
                    case LogLevel.Info:
                        label.text += $"{message}\n";
                        break;
                    case LogLevel.Warning:
                        label.text += $"<color=yellow>{message}</color>\n";
                        break;
                    case LogLevel.Error:
                        label.text += $"<color=red>{message}</color>\n";
                        break;
                }
            }
        }

        private class EditorWindow : UnityEditor.EditorWindow
        {
            private CancellationTokenSource cts = new();

            public void CreateGUI()
            {
                var scrollview = new ScrollView();
                rootVisualElement.Add(scrollview);

                var textField         = new TextField { multiline = true };
                var label             = new Label();
                var logger            = new DebugBytecodeLogger(label);
                var compilerContainer = new VisualElement();
                textField.RegisterValueChangedCallback(evt =>
                {
                    cts.Cancel();
                    cts.Dispose();
                    cts = new CancellationTokenSource();

                    _ = UniTask.Delay(TimeSpan.FromSeconds(1), true, cancellationToken: cts.Token).ContinueWith(() =>
                    {
                        label.text = string.Empty;
                        compilerContainer.Clear();
                        if (cts.IsCancellationRequested)
                        {
                            return;
                        }

                        Scanner.Scan(textField.value, out var tokens, logger);

                        var compilerLabel  = new Label();
                        var compilerLogger = new DebugBytecodeLogger(compilerLabel);
                        var compiler       = new Compiler();
                        if (textField.text.Trim() != string.Empty)
                        {
                            compilerContainer.Add(
                                new Button(() =>
                                {
                                    compilerLabel.text = string.Empty;
                                    compiler.Interpret(tokens, out var instructionSet, compilerLogger);

                                    compilerLogger.Log(LogLevel.Info, "\n==== Bytecode ====");
                                    bool wasPevConstant = false;
                                    foreach (var code in instructionSet.Instructions)
                                    {
                                        compilerLogger.Log(LogLevel.Info, wasPevConstant ? code.ToString() : ((Bytecode)code).ToString());
                                        wasPevConstant = (Bytecode)code == Bytecode.Constant && !wasPevConstant;
                                    }
                                }) { text = "Compile" });
                            compilerContainer.Add(compilerLabel);
                        }
                    });
                });

                scrollview.Add(textField);
                scrollview.Add(label);
                scrollview.Add(compilerContainer);
            }
        }
    }
}