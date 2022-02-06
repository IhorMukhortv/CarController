using System;
using System.Threading;

namespace CarController.Infrastructure.Background
{
	public class DataSender
    {
        public DataSender(Action action)
        {
            _action = action;
        }

        private Thread _sender;
        private Action _action;
        private bool issending = false;

        public void StartSending()
        {
			issending = true;
            _sender = new Thread(SendData);
            _sender.IsBackground = true;
            _sender.Start();
        }

        public void StopSending()
        {
            issending = false;
            if (_sender.Join(200) == false)
            {
                _sender.Abort();
            }
            _sender = null;
        }

        public void ReStart()
        {
			if (issending)
			{
                StopSending();
			}

            StartSending();
        }

        private void SendData()
        {
            _action.Invoke();
        }
    }
}
