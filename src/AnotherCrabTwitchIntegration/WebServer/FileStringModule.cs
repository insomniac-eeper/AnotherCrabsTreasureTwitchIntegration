// /*
//  * SPDX-License-Identifier: GPL-3.0
//  * Another Crab's Treasure Twitch Integration
//  * Copyright (c) 2024 insomniac-eeper and contributors
//  */

namespace AnotherCrabTwitchIntegration.WebServer;

using System;
using EmbedIO;
using System.IO;
using System.Threading.Tasks;

public class FileStringModule : WebModuleBase
{
    private Func<string> _getContentString;
    private readonly string _mimeType;
    public string EndpointPath { get; }

    public FileStringModule(Func<string> getContentString, string endpointPath, string mimeType) : base(endpointPath)
    {
        _getContentString = getContentString;
        _mimeType = mimeType;
        EndpointPath = endpointPath;
    }

    protected override async Task OnRequestAsync(IHttpContext context)
    {
        if (context.Request.HttpMethod.Equals(HttpVerbs.Get.ToString(), StringComparison.OrdinalIgnoreCase) && context.Request.Url.LocalPath == EndpointPath)
        {
            await GetContent(context);
        }
    }

    public override bool IsFinalHandler => true;

    private async Task GetContent(IHttpContext context)
    {
        context.Response.ContentType = _mimeType;
        using var writer = new StreamWriter(context.Response.OutputStream);
        await writer.WriteAsync(_getContentString());
    }
}