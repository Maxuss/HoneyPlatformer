using UnityEngine;

namespace Program
{
    public class TestProgrammable: MonoBehaviour, IActionContainer
    {
        public string Name => "Тестовый объект";
        public string Description => "Это просто тест исполнителя ааааа";

        public ActionInfo[] SupportedActions { get; } = new[]
        {
            new ActionInfo
            {
                ActionName = "Действие 1",
                ActionDescription = "Тестовое действие 1 без параметра",
            },
            new ActionInfo
            {
                ActionName = "Действие 2",
                ActionDescription = "Тестовое действие 2 с численным параметром",
                ParameterName = "Параметр 1",
                MaxFloatValue = 14,
                ValueType = ActionValueType.Float
            },
            new ActionInfo
            {
                ActionName = "Действие 3",
                ActionDescription = "Тестовое действие 2 с параметром перечисления",
                ParameterName = "Параметр 2",
                EnumType = typeof(TestEnum),
                ValueType = ActionValueType.Enum
            }
        };

        public ProgrammableType Type { get; } = ProgrammableType.Executor;
        public ActionData SelectedAction { get; set; }
        
        public void Begin(ActionData action)
        {
            Debug.Log($"SELECTED: {action.ActionIndex} {action.StoredValue}");
        }
    }

    public enum TestEnum
    {
        ValueA,
        ValueB,
        ValueC
    }
}