using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGF.Photon.Implementation.Codes {
    public enum ReturnCode : short {
        InvalidUserPass = -4,
        OperationDenied = -3,
        OperationInvalid = -2,
        InternalServerError = -1,

        Ok = 0,
        
    }
}