using System;
using Microsoft.Identity.Client;
using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class FailedGroupServiceException : Xeption
    {
        public FailedGroupServiceException(Exception innerException)
            :base(message : "Failed group service error occured, please contact support.",
                 innerException)
        {}
    }
}
