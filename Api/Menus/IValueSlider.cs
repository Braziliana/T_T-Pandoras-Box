namespace Api.Menus;

public interface IValueSlider : IMenuElement
{
    float Value { get; set; }
    int IntValue => (int)Math.Round(Value);
}