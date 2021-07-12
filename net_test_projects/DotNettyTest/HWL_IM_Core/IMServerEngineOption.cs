using HWL_IM_Core.Extra;
using HWL_IM_Core.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core
{
    public class IMServerEngineOption
    {
        private IServerUserAction _UserAction;
        public IServerUserAction UserAction
        {
            get { return _UserAction; }
            set
            {
                _UserAction = value;
                Init();
            }
        }

        private IServerSessionAction _SessionAction;
        public IServerSessionAction SessionAction
        {
            get { return _SessionAction; }
            set
            {
                _SessionAction = value;
                Init();
            }
        }

        private IOfflineMessageAction _OfflineMessageAction;
        public IOfflineMessageAction OfflineMessageAction
        {
            get { return _OfflineMessageAction; }
            set
            {
                _OfflineMessageAction = value;
                Init();
            }
        }

        //public IServerUserAction UserAction { get; set; }
        //public IServerSessionAction SessionAction { get; set; }
        //public IOfflineMessageAction OfflineMessageAction { get; set; }
        public OnlineChannelManager ChannelManager { get; private set; }


        //private Dictionary<ImMessageType, Func<ImMessageContext, IServerMessageReceiver>> Receivers;

        public IMServerEngineOption()
        {
            this.UserAction = new DefaultServerUserAction();
            this.OfflineMessageAction = new DefaultOfflineMessageManager();
            this.SessionAction = new ServerSessionManager();
            //this.MessageOperator = new ServerMessageManager();

            //this.Receivers = new Dictionary<ImMessageType, Func<ImMessageContext, IServerMessageReceiver>>();
            //this.Receivers.Add(ImMessageType.Validate, c => new ChannelValidateMessageReceiver(c.ValidateRequest));
            //this.Receivers.Add(ImMessageType.Basic, c => new BasicMessageReceiver(c.ChatRequest));

            this.Init();
        }

        private void Init()
        {
            if (this.ChannelManager == null)
            {
                this.ChannelManager = new OnlineChannelManager();
            }
            this.ChannelManager.SetSessionAction(this.SessionAction);
        }

        //public Dictionary<ImMessageType, Func<ImMessageContext, IServerMessageReceiver>> GetReceivers()
        //{
        //    return this.Receivers;
        //}

        //public void AddReceiver(ImMessageType type, Func<ImMessageContext, IServerMessageReceiver> receiver)
        //{
        //    this.Receivers.Add(type, receiver);
        //}

        //public void AddOrReplaceReceiver(ImMessageType type, Func<ImMessageContext, IServerMessageReceiver> receiver)
        //{
        //    if (this.Receivers.ContainsKey(type))
        //    {
        //        this.Receivers.Remove(type);
        //    }
        //    this.Receivers.Add(type, receiver);
        //}
    }
}
