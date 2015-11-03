using System;
using GalaSoft.MvvmLight.Messaging;

namespace CasualMeter.Common.Conductors
{
    public sealed class CasualMessenger
    {
        private static readonly Lazy<CasualMessenger> Lazy = new Lazy<CasualMessenger>(() => new CasualMessenger());

        public static CasualMessenger Instance
        {
            get { return Lazy.Value; }
        }

        public IMessenger Messenger { get { return GalaSoft.MvvmLight.Messaging.Messenger.Default; } }

        private CasualMessenger()
        {
        }

        public void SendWindowVisibilityMessage(bool isVisible)
        {
            Messenger.Send(new WindowVisibilityMessage{ IsVisible = isVisible });
        }
    }
}
