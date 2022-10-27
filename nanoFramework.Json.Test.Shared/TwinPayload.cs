//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;

namespace nanoFramework.Json.Test.Shared
{
    public class TwinPayload
    {
        public string deviceId { get; set; }
        public string etag { get; set; }
        public string status { get; set; }
        public DateTime statusUpdateTime { get; set; }
        public string connectionState { get; set; }
        public DateTime lastActivityTime { get; set; }
        public int cloudToDeviceMessageCount { get; set; }
        public string authenticationType { get; set; }
        public Hashtable x509Thumbprint { get; set; }
        public string modelId { get; set; }
        public int version { get; set; }
    }
}
