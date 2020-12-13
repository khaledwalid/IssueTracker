// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IssueTracker.Presentation.Controllers.IdentityControllers.Consent;

namespace IssueTracker.Presentation.Controllers.IdentityControllers.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}