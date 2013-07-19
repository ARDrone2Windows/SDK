#ifndef _ARDRONE_TCP_CLIENT_
#define _ARDRONE_TCP_CLIENT_

#include <memory>
#include <string>
#include <functional>

#include "VideoPacket.h"
#include <mfidl.h>
#include <ppltasks.h>


class ARDroneTcpClient
{
public:
    ARDroneTcpClient(const std::wstring&);
    ~ARDroneTcpClient();

    void Start(std::function<void(bool)>);
    void Stop();

    //bool PopVideoPacket( VideoPacket* pOutVideoPacket);
	Concurrency::task<std::shared_ptr<VideoPacket>> PopVideoPacketAsync();
private:
    class Impl;
    std::shared_ptr<VideoPacket> lastVideoPacket;
    std::unique_ptr<Impl> impl;
};

#endif