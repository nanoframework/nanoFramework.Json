//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Json.Test.Shared
{
    public class JsonTestClassChild
    {
        public int one { get; set; }
        public int two { get; set; }
        public int three { get; set; }
#pragma warning disable S1104 // Fields should not have public accessibility
        public int four; //not a property on purpose!
#pragma warning restore S1104 // Fields should not have public accessibility
    }
}
