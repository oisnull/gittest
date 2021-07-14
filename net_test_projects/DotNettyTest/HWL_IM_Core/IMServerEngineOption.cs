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
        //private IServerUserAction _UserAction;
        //public IServerUserAction UserAction
        //{
        //    get { return _UserAction; }
        //    set
        //    {
        //        _UserAction = value;
        //        Init();
        //    }
        //}

        //private IOfflineMessageAction _OfflineMessageAction;
        //public IOfflineMessageAction OfflineMessageAction
        //{
        //    get { return _OfflineMessageAction; }
        //    set
        //    {
        //        _OfflineMessageAction = value;
        //        Init();
        //    }
        //}

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

        //public IServerSessionAction SessionAction { get; set; }
        public IServerUserAction UserAction { get; set; }
        public IOfflineMessageAction OfflineMessageAction { get; set; }
        public OnlineChannelManager ChannelManager { get; private set; }

        public IMServerEngineOption()
        {
            this.UserAction = new DefaultServerUserAction();
            this.OfflineMessageAction = new DefaultOfflineMessageManager();
            this.SessionAction = new ServerSessionManager();

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
    }
}
