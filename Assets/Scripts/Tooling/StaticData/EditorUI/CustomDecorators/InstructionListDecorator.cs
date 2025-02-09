using System.Collections.Generic;
using System.Linq;
using Fight.Engine.Bytecode;
using Tooling.Logging;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI
{
    // ReSharper disable once UnusedType.Global
    public class InstructionListDecorator : IDecorator<List<IInstruction>>
    {
        public void DecorateElement(GeneralField generalField, List<IInstruction> element)
        {
        }
    }

    // ReSharper disable once UnusedType.Global
    public class InstructionDecorator : IDecorator<IInstruction>
    {
        public void DecorateElement(GeneralField generalField, IInstruction element)
        {
            var elementType = element.GetType();
            var elementInterfaces = elementType.GetInterfaces();

            var pushInterface = elementInterfaces
                .FirstOrDefault(iType => iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IPush<>));
            if (pushInterface != null)
            {
                var popType = pushInterface.GetGenericArguments()[0];

                generalField.GetFieldDrawer().Insert(0, new Label($"Output: {popType.Name}"));
            }

            var popInterface = elementInterfaces
                .FirstOrDefault(iType => iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IPop<>));
            if (popInterface != null)
            {
                var popType = popInterface.GetGenericArguments()[0];

                generalField.GetFieldDrawer().Insert(0, new Label($"Input: {popType.Name}"));
            }

            popInterface = elementInterfaces
                .FirstOrDefault(iType => iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IPop<,>));
            if (popInterface != null)
            {
                var popTypes = popInterface.GetGenericArguments();

                generalField.GetFieldDrawer()
                    .Insert(0, new Label($"Inputs: {popTypes.Select(t => t.Name).Aggregate((a, b) => $"{a}, {b}")}"));
            }
        }
    }
}