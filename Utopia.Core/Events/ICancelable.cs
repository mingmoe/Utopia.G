namespace Utopia.Core.Events;
/// <summary>
/// If a event implement this interface,the event can be canceled.
/// </summary>
public interface ICancelable : IEvent
{
    void SetCancel(bool isCancel);
}
