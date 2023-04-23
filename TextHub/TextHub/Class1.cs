using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TextHub
{
    public interface IMessageService
    {
        void ShowInformation(string message, string subMessage);
        void ShowWarning(string message);
        void ShowError(string message);
    }
    public class MessageService : IMessageService
    {
        public void ShowInformation(string message, string subMessage)
        {
            MessageBox.Show(message, subMessage, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
