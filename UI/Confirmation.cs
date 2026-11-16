using PopupLib.UI.Windows;

namespace ChartManager.UI
{
    internal sealed class Confirmation
    {
        internal PromptWindow Base;
        internal Action OnConfirm;
        internal Action? OnCancel;

        internal void Show() => Base.Show();

        internal void Close() => Base.ForceClose();

        private void OnResult(PopupLib.UI.Windows.Abstract.BaseWindow _)
        {
            if (Base.Result == true)
                OnConfirm.Invoke();
            else if (OnCancel != null)
                OnCancel.Invoke();
        }

        internal Confirmation(string title, string text, Action onConfirm, Action? onCancel = null)
        {
            Base = new(new(text), new(title));
            Base.AutoReset = true;
            Base.OnCompletion += OnResult;
        }
    }
}
