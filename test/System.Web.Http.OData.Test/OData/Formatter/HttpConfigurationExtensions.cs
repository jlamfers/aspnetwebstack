﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;
using Moq;

namespace System.Web.Http.OData.Formatter
{
    internal static class HttpConfigurationExtensions
    {
        public static void AddFakeODataRoute(this HttpConfiguration configuration)
        {
            Mock<IHttpRoute> mockRoute = new Mock<IHttpRoute>();
            Mock<IHttpVirtualPathData> mockVirtualPath = new Mock<IHttpVirtualPathData>();
            mockVirtualPath.Setup(p => p.Route).Returns(mockRoute.Object);
            mockRoute.Setup(r => r.RouteTemplate).Returns("http://localhost");
            mockRoute.Setup(v => v.GetVirtualPath(
                It.IsAny<HttpRequestMessage>(), It.IsAny<Dictionary<string, object>>())).Returns(
                mockVirtualPath.Object);
            configuration.Routes.Add(ODataRouteConstants.RouteName, mockRoute.Object);
        }
    }
}
