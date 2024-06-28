/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.WebServer;

using System.IO;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

public class ResourceFileServeModule
{
    public class StaticResourceModule(string urlPath, string staticFile) : WebApiController
    {
        [Route(HttpVerbs.Get, "/")]
        public async Task GetResource()
        {

            HttpContext.Response.ContentType = "text/html";
            using var writer = new StreamWriter(HttpContext.Response.OutputStream);
            await writer.WriteAsync(staticFile);
        }


    }
}
