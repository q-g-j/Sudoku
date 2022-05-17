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
            OnMessageTransmitted?.Invoke(type, message);
        }

        public static Action<string, string> OnMessageTransmitted;
    }
}
