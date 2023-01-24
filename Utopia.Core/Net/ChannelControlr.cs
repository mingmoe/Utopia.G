//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core.Net;

/// <summary>
/// 通道控制器，负责控制通道，检测通道和套接字状态，根据套接字状态发送各种通道调用链。
/// </summary>
public class ChannelControlr
{
    static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    readonly IChannel _channel;
    readonly ISocket _socket;

    public ChannelControlr(IChannel channel, ISocket handle)
    {
        ArgumentNullException.ThrowIfNull(channel, nameof(channel));
        ArgumentNullException.ThrowIfNull(handle, nameof(handle));

        this._socket = handle;
        this._channel = channel;
    }

    /// <summary>
    /// 这个指标指示套接字是否处在链接中
    /// </summary>
    public bool IsConnected { get; private set; } = true;

    /// <summary>
    /// 这个指标只是是否还需要调用Fire函数。
    /// 在套接字关闭之后需要触发Disconnect调用链，
    /// 因此不能单纯地检查套接字是否关闭来检查是否需要继续调用Fire。
    /// </summary>
    public bool HaveDone { get; private set; } = false;

    private bool _firedConnected = false;

    private bool _firedDisconnected = false;

    public async Task Fire()
    {
        if (!_socket.Connected)
        {
            this.IsConnected = false;
            _socket.Close();

            if (!_firedDisconnected)
            {
                this.HaveDone = true;
                _firedDisconnected = true;
                await _channel.FireDisconnect();
            }

            return;
        }

        if (!_firedConnected)
        {
            _firedConnected = true;
            await _channel.FireConnect();
        }

        await _channel.FireRead();
    }

    /// <summary>
    /// 停止任务循环
    /// </summary>
    public void StopFireLoopAndDisconnect()
    {
        _socket.Close();
    }

    /// <summary>
    /// 设置一个任务循环
    /// </summary>
    public void FireLoop()
    {
        var t = async () =>
        {
            while (!this.HaveDone)
            {
                try
                {
                    await this.Fire();
                }
                catch (Exception e)
                {
                    _logger.Error(e, "failed to fire the channel");
                }
                // 事不过三
                await Task.Delay(3);
            }

        };
        Task.Factory.StartNew(t);
    }
}
