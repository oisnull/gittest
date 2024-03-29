﻿using HWL_IM_Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWL_IM_Core.Client
{
    public interface IClientMessageExecutor
    {
        ImMessageContext BuildContext();
        void Receive(ImMessageContext message);
    }
}
