using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Services
{
    public static class MessengerService
    {
        public static void BroadCast(string type, string message)
        {
            OnMessageTransmittedTwoParams?.Invoke(type, message);
        }

        public static void BroadCastWithButton(string type, string message, string button)
        {
            OnMessageTransmittedThreeParams?.Invoke(type, message, button);
        }

        public static Action<string, string> OnMessageTransmittedTwoParams;
        public static Action<string, string, string> OnMessageTransmittedThreeParams;
    }
}
