﻿//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;
using System.Text;

namespace nanoFramework.Json.Resolvers
{
    public interface IMemberResolver
    {
        MemberSet Get(string memberName, Type objectType);
    }
}