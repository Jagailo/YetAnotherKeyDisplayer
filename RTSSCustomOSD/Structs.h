#pragma once

using namespace System;
using namespace System::Drawing;
using namespace System::Diagnostics;

namespace RTSSSharedMemoryNET {
    [Flags]
    public enum class AppFlags
    {
        None        = 0,
        OpenGL      = APPFLAG_OGL,
        DirectDraw  = APPFLAG_DD,
        Direct3D8   = APPFLAG_D3D8,
        Direct3D9   = APPFLAG_D3D9,
        Direct3D9Ex = APPFLAG_D3D9EX,
        Direct3D10  = APPFLAG_D3D10,
        Direct3D11  = APPFLAG_D3D11,

        ProfileUpdateRequested = APPFLAG_PROFILE_UPDATE_REQUESTED,
        MASK = (APPFLAG_DD | APPFLAG_D3D8 | APPFLAG_D3D9 | APPFLAG_D3D9EX | APPFLAG_OGL | APPFLAG_D3D10  | APPFLAG_D3D11),
    };

    [Flags]
    public enum class StatFlags
    {
        None   = 0,
        Record = STATFLAG_RECORD,
    };

    [Flags]
    public enum class ScreenshotFlags
    {
        None              = 0,
        RequestCapture    = SCREENCAPTUREFLAG_REQUEST_CAPTURE,
        RequestCaptureOSD = SCREENCAPTUREFLAG_REQUEST_CAPTURE_OSD,
    };

    [Flags]
    public enum class VideoCaptureFlags
    {
        None                   = 0,
        RequestCaptureStart    = VIDEOCAPTUREFLAG_REQUEST_CAPTURE_START,
        RequestCaptureProgress = VIDEOCAPTUREFLAG_REQUEST_CAPTURE_PROGRESS,
        RequestCaptureStop     = VIDEOCAPTUREFLAG_REQUEST_CAPTURE_STOP,
        RequestCaptureOSD      = VIDEOCAPTUREFLAG_REQUEST_CAPTURE_OSD,

        INTERNAL_RESIZE = VIDEOCAPTUREFLAG_INTERNAL_RESIZE,
        MASK = (VIDEOCAPTUREFLAG_REQUEST_CAPTURE_START | VIDEOCAPTUREFLAG_REQUEST_CAPTURE_PROGRESS | VIDEOCAPTUREFLAG_REQUEST_CAPTURE_STOP),
    };

    ///////////////////////////////////////////////////////////////////////////

    [DebuggerDisplay("{Owner}, {Text}")]
    public ref struct OSDEntry
    {
    public:
        String^ Owner;
        String^ Text;
    };

    [DebuggerDisplay("{ProcessId}:{Name}, {Flags}")]
    public ref struct AppEntry
    {
    public:
        int ProcessId;
        String^ Name;
        AppFlags Flags;
        
        //instantaneous framerate fields
        DateTime InstantaneousTimeStart;
        DateTime InstantaneousTimeEnd;
        DWORD InstantaneousFrames;
        TimeSpan InstantaneousFrameTime;

        //framerate stats fields
        StatFlags StatFlags;
        DateTime StatTimeStart;
        DateTime StatTimeEnd;
        DWORD StatFrames;
        DWORD StatCount;
        DWORD StatFramerateMin;
        DWORD StatFramerateAvg;
        DWORD StatFramerateMax;

        //framerate stats 2.5+
        DWORD StatFrameTimeMin;
        DWORD StatFrameTimeAvg;
        DWORD StatFrameTimeMax;
        DWORD StatFrameTimeCount;
        /* TODO
        DWORD StatFrameTimeBuf[1024];
        DWORD StatFrameTimeBufPos;
        DWORD StatFrameTimeBufFramerate;
        */

        //OSD fields
        int OSDCoordinateX;
        int OSDCoordinateY;
        DWORD OSDZoom;
        Color OSDColor;
        DWORD OSDFrameId;
        Color OSDBackgroundColor; //2.1+
        
        //screenshot fields
        ScreenshotFlags ScreenshotFlags;
        String^ ScreenshotPath;
        DWORD ScreenshotQuality; //2.2+
        DWORD ScreenshotThreads; //2.2+

        //video capture fields - 2.2+
        VideoCaptureFlags VideoCaptureFlags;
        String^ VideoCapturePath;
        DWORD VideoFramerate;
        DWORD VideoFramesize;
        DWORD VideoFormat;
        DWORD VideoQuality;
        DWORD VideoCaptureThreads;
        DWORD VideoCaptureFlagsEx; //2.4+

        //audio capture fields
        DWORD AudioCaptureFlags; //2.3+
        DWORD AudioCaptureFlags2; //2.5+
        Int64 AudioCapturePTTEventPush; //2.6+
        Int64 AudioCapturePTTEventRelease; //2.6+
        Int64 AudioCapturePTTEventPush2; //2.6+
        Int64 AudioCapturePTTEventRelease2; //2.6+
    };
}