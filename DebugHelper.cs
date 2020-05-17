//
// Copyright (c) 2020 The nanoFramework project contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

// Don't really want this on now that others will be using lib-nanoFramework.JSON
//#define NANOFRAMEWORK_DISPLAY_DEBUG

using System;

namespace nanoFramework.Json
{
    internal static class DebugHelper
    {
        // Used this a lot in the effort to get Array serialization & deserialization working
        public static void DisplayDebug(string displayString)
        {
#if NANOFRAMEWORK_DISPLAY_DEBUG
		Console.WriteLine(displayString);           // Show Serialize & Deserialize details - but only when NANOFRAMEWORK_DISPLAY_DEBUG is defined
#endif
        }
    }
}
