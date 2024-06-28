/*
 * SPDX-License-Identifier: GPL-3.0
 * Insomniac-eeper's Common.TwitchLibrary
 * Copyright (c) 2024 insomniac-eeper and contributors
 */

namespace Common.TwitchLibrary.Utilities;

public class HttpClientWrapper
{
    private readonly HttpClient _httpClient = new();

    public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
    {
        return await _httpClient.PostAsync(url, content);
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage msg)
    {
        return await _httpClient.SendAsync(msg);
    }
}