/*
 * SPDX-License-Identifier: GPL-3.0
 * Another Crab's Treasure Twitch Integration
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace AnotherCrabTwitchIntegration.Modules.WebServer;

using System;
using System.IO;
using System.Threading.Tasks;
using EmbedIO;

/// <summary>
/// Silly module to serve file content to specific routes
/// </summary>
/// <param name="getContentString">Function to request file contents as a string.</param>
/// <param name="endpointPath">Path on which to serve the file string.</param>
/// <param name="mimeType">Content type of the file contents.</param>
public class FileStringModule(Func<string> getContentString, string endpointPath, string mimeType)
    : WebModuleBase(endpointPath)
{
    private string EndpointPath { get; } = endpointPath;

    /// <inheritdoc cref="WebModuleBase.OnRequestAsync"/>
    protected override async Task OnRequestAsync(IHttpContext context)
    {
        if (context.Request.HttpMethod.Equals(HttpVerbs.Get.ToString(), StringComparison.OrdinalIgnoreCase) && context.Request.Url.LocalPath == EndpointPath)
        {
            await GetContent(context);
        }
    }

    /// <inheritdoc cref="WebModuleBase.IsFinalHandler"/>
    public override bool IsFinalHandler => true;

    private async Task GetContent(IHttpContext context)
    {
        context.Response.ContentType = mimeType;
        using var writer = new StreamWriter(context.Response.OutputStream);
        await writer.WriteAsync(getContentString());
    }
}