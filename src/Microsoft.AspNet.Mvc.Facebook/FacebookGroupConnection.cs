﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.AspNet.Mvc.Facebook
{
    public class FacebookGroupConnection<T>
    {
        public IList<T> Data { get; set; }
    }
}